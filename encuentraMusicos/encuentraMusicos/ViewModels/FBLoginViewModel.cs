using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.Views;
using Newtonsoft.Json.Linq;
using Plugin.FacebookClient;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace encuentraMusicos.ViewModels
{
    public class FBLoginViewModel : INotifyPropertyChanged
    {
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        bool isAdmin = false;
        string[] permisions = new string[] { "public_profile" };

        public bool IsBusy { get; set; } = false;
        public bool IsNotBusy { get { return !IsBusy; } }
        public FBProfileAndroid Profile { get; set; }

        public Command OnLoginCommand { get; set; }
        public Command OnShareDataCommand { get; set; }
        public Command OnLoadDataCommand { get; set; }
        public Command OnLogoutCommand { get; set; }
        public bool IsLoggedIn { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public FBLoginViewModel()
        {

            Profile = new FBProfileAndroid();

            OnLoginCommand = new Command(async () => await LoginAsync());
            OnShareDataCommand = new Command(async () => await ShareDataAsync());
            OnLoadDataCommand = new Command(async () => await LoadData());
            OnLogoutCommand = new Command(() =>
            {
                if (CrossFacebookClient.Current.IsLoggedIn)
                {
                    CrossFacebookClient.Current.Logout();
                    IsLoggedIn = false;
                }
            });

            //Posts = new ObservableCollection<FacebookPost>();
            //LoadDataCommand.Execute(null);
        }

        public async Task LoginAsync()
        {
            FacebookResponse<bool> response = await CrossFacebookClient.Current.LoginAsync(permisions);
            switch (response.Status)
            {
                case FacebookActionStatus.Completed:
                    IsLoggedIn = true;
                    OnLoadDataCommand.Execute(null);
                    break;
                case FacebookActionStatus.Canceled:

                    break;
                case FacebookActionStatus.Unauthorized:
                    await App.Current.MainPage.DisplayAlert("Unauthorized", response.Message, "Ok");
                    break;
                case FacebookActionStatus.Error:
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Ok");
                    break;
            }

        }

        async Task ShareDataAsync()
        {
            FacebookShareLinkContent linkContent = new FacebookShareLinkContent("Awesome team of developers, making the world a better place one project or plugin at the time!",
                                                                                new Uri("http://www.github.com/crossgeeks"));
            var ret = await CrossFacebookClient.Current.ShareAsync(linkContent);
        }

        public async Task LoadData()
        {
            IsBusy = true;

            var jsonData = await CrossFacebookClient.Current.RequestUserDataAsync
            (
                  new string[] { "id", "name", "picture", "cover", "friends" }, new string[] { }
            );

            var data = JObject.Parse(jsonData.Data);
            Profile = new FBProfileAndroid()
            {
                FullName = data["name"].ToString(),
                Picture = new UriImageSource { Uri = new Uri($"{data["picture"]["data"]["url"]}") },
            };
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MySQLite.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<T_Registro>();

            var newUsuario = new T_Registro();
            newUsuario.Id = data["id"].ToString();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(newUsuario.Id);
            newUsuario.userb64 = System.Convert.ToBase64String(plainTextBytes);
            newUsuario.Nombre = data["name"].ToString();
            newUsuario.Image = Profile.Picture.ToString();

            string urlRequest = globalValues.webSite
                + "reg_usuario.php"
                + "?fbUser=" + newUsuario.Id
                + "&b64User=" + newUsuario.userb64;
            string responseRegistro = client.GetStringAsync(urlRequest).Result;

            JObject regResponse = JObject.Parse(responseRegistro);

            string result = regResponse["success"].ToString();

            /**/
            string urlExistUsuario = globalValues.webSite
                + "datos_pers_musico.php"
                + "?tpBusqueda=S"
                + "&idUsuario=" + newUsuario.Id;

            string responseExiste = client.GetStringAsync(urlExistUsuario).Result;

            JObject regExiste = JObject.Parse(responseExiste);

            string tpMusico = regExiste["datosPersonalesM"]["tipo_musico"].ToString();

            if (string.IsNullOrEmpty(tpMusico))
            {
                tpMusico = "S";
            }
            /**/

            if (result.Equals("1"))
            {
                CreateFolder(newUsuario.userb64);
                newUsuario.isSynchronized = "Y";
            }
            else
            {
                newUsuario.isSynchronized = "N";
            }

            newUsuario.isActive = "Y";
            newUsuario.tpMusico = tpMusico;
            try
            {
                db.Insert(newUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsInserted: " + ex.ToString());
            }

            //Inicia validación de administrador
            string urlIsAdmin = globalValues.webSite
                + "check_admin.php"
                + "?Usuario=" + newUsuario.Id;

            string responseIsAdmin = client.GetStringAsync(urlIsAdmin).Result;

            JObject regIsAdmin = JObject.Parse(responseIsAdmin);

            string strIsAdmin = regIsAdmin["isAdmin"].ToString();

            if (strIsAdmin.Equals("Y"))
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            //Termina validación de administrador

            if (isAdmin)
            {
                Application.Current.MainPage = new NavigationPage(new Administracion(isAdmin, newUsuario.Id));
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new PerfilMusico(newUsuario.Id, tpMusico));
            }
        }

        public void CreateFolder(string folder)
        {
            bool IsCreated;
            try
            {
                WebRequest request = WebRequest.Create(globalValues.hostFTP + folder);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(globalValues.userFTP, globalValues.passwordFTP);
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                }
            }
            catch (Exception ex)
            {
                IsCreated = false;
                Console.WriteLine("IsCreated: " + IsCreated + " ex: " + ex.ToString());
            }
        }
    }
}
