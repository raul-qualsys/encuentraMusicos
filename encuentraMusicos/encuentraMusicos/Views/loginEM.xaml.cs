using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
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
    public partial class loginEM : ContentPage
    {
        bool conexion;
        double resolution;
        GlobalValues globalValues = new GlobalValues();
        public loginEM()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;

            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                conexion = true;
                sinConexion.IsVisible = false;
                sinConexion.HeightRequest = 0;
                gridPrincipal.IsVisible = true;
                
                if (resolution > 2000000)
                {
                    btnCancelar.FontSize = 20;
                    lbLoginEM.FontSize = 20;
                    emUsrLbl.FontSize = 18;
                    usrEmail.FontSize = 18;
                    enviarEmail.FontSize = 20;
                    lbCorreo.FontSize = 16;
                }
                else
                {
                    btnCancelar.FontSize = 12;
                    lbLoginEM.FontSize = 12;
                    emUsrLbl.FontSize = 10;
                    usrEmail.FontSize = 10;
                    enviarEmail.FontSize = 12;
                    lbCorreo.FontSize = 10;
                }
            }
            else
            {
                conexion = false;
                sinConexion.IsVisible = true;
                gridPrincipal.IsVisible = false;
                gridPrincipal.HeightRequest = 0;

                if (resolution > 2000000)
                {
                    logoLoadingSC.Margin = new Thickness(100, 100, 100, 0);
                    lbSinConexion.FontSize = 22;
                    reintentarBtn.FontSize = 20;
                }
                else
                {
                    logoLoadingSC.Margin = new Thickness(85, 100, 85, 15);
                    lbSinConexion.FontSize = 14;
                    reintentarBtn.FontSize = 12;
                }
            }
        }
        private void sendEmail(object sender, EventArgs e)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MySQLite.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<T_Registro>();

            var newUsuario = new T_Registro();
            newUsuario.Id = usrEmail.Text;
            newUsuario.userb64 = Base64Encode(newUsuario.Id);
            newUsuario.Nombre = usrEmail.Text;
            newUsuario.Image = "NONE";

            HttpClient client = new HttpClient();

            string urlRequest = globalValues.webSite
                + "reg_usuario.php"
                + "?email=" + usrEmail.Text
                + "&b64User=" + newUsuario.userb64;
            string responseRegistro = client.GetStringAsync(urlRequest).Result;

            JObject regResponse = JObject.Parse(responseRegistro);

            string result = regResponse["success"].ToString();

            if (result.Equals("1"))
            {
                newUsuario.isSynchronized = "Y";
                CreateFolder(newUsuario.userb64);
            }
            else
            {
                newUsuario.isSynchronized = "N";
            }
            newUsuario.isActive = "N";

            db.Insert(newUsuario);

            Application.Current.MainPage = new NavigationPage(new validacionEmail(usrEmail.Text));
        }
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private void cancelLoginEM(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new LoginMusico());
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
                Console.WriteLine("IsCreated: " + IsCreated + " ex: " + ex.ToString());
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}