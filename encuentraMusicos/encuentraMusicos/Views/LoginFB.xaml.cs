using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginFB : ContentPage
    {
        double resolution;
        bool isAdmin = false;
        GlobalValues globalValues = new GlobalValues();
        public LoginFB()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            fbLoginWebView.Source = "https://www.facebook.com/v7.0/dialog/oauth?client_id=169133225273780&response_type=token&redirect_uri=https://www.facebook.com/connect/login_success.html";
            fbLoginWebView.Navigated += loginFBWebViewNavigated;

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;
            if (resolution > 2000000)
            {
                btnCancelar.FontSize = 20;
                lbLoginFB.FontSize = 20;
            }
            else
            {
                btnCancelar.FontSize = 12;
                lbLoginFB.FontSize = 12;
            }
        }
        private void cancelLoginFB(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new LoginMusico());
        }
        private void loginFBWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            var AccessURL = e.Url;

            if (AccessURL.Contains("access_token"))
            {
                AccessURL = AccessURL.Replace("https://www.facebook.com/connect/login_success.html#access_token=", string.Empty);
                AccessURL = AccessURL.Replace("https://web.facebook.com/connect/login_success.html?_rdc=1&_rdr#access_token=", string.Empty);
                var accessToken = AccessURL.Split('&')[0];
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync("https://graph.facebook.com/me?fields=email,name,picture&access_token=" + accessToken).Result;
                var Data = JsonConvert.DeserializeObject<FacebookProfile>(response);

                string urlImage = "";

                if (!Data.Picture.Data.IsSilhouette)
                {
                    urlImage = Data.Picture.Data.Url;
                }
                else
                {
                    urlImage = "NONE";
                }

                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MySQLite.db3");
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<T_Registro>();

                var newUsuario = new T_Registro();
                newUsuario.Id = Data.Id;
                newUsuario.userb64 = Base64Encode(newUsuario.Id);
                newUsuario.Nombre = Data.Name;
                newUsuario.Image = urlImage;

                string urlRequest = globalValues.webSite 
                    + "reg_usuario.php"
                    + "?fbUser=" + Data.Id
                    + "&b64User=" + newUsuario.userb64;
                string responseRegistro = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseRegistro);

                string result = regResponse["success"].ToString();

                /**/
                string urlExistUsuario = globalValues.webSite
                    + "datos_pers_musico.php"
                    + "?tpBusqueda=S"
                    + "&idUsuario=" + Data.Id;

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

                db.Insert(newUsuario);

                //Inicia validación de administrador
                string urlIsAdmin = globalValues.webSite
                    + "check_admin.php"
                    + "?Usuario=" + Data.Id;

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
                    Application.Current.MainPage = new NavigationPage(new Administracion(isAdmin, Data.Id));
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new PerfilMusico(Data.Id, tpMusico));
                }
            }
            else if (AccessURL.Contains("error=access_denied"))
            {
                Application.Current.MainPage = new NavigationPage(new LoginMusico());
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
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
                Console.WriteLine("IsCreated: "+IsCreated+" ex: "+ex.ToString());
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}