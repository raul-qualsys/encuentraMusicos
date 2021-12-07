using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class validacionEmail : ContentPage
    {
        double resolution;
        bool conexion;
        bool isAdmin = false;
        int existUsuario = 0;
        string databasePath = "";
        string usuario;
        SQLiteConnection db;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public validacionEmail(string idUsuario)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;

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
                    lbLoginEM.FontSize = 20;
                    textUsuario.FontSize = 16;
                }
                else
                {
                    lbLoginEM.FontSize = 12;
                    textUsuario.FontSize = 10;
                }

                idUsuarioEM.Text = usuario;
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
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private void sendValidacion(object sender, EventArgs e)
        {
            string urlRequest = globalValues.webSite
                + "val_usuario.php"
                + "?email=" + usuario
                + "&codigo=" + codUsuario.Text;
            string responseRegistro = client.GetStringAsync(urlRequest).Result;

            JObject regResponse = JObject.Parse(responseRegistro);

            string result = regResponse["success"].ToString();

            if (result.Equals("1"))
            {
                /**/

                //Inicia validación de administrador
                string urlIsAdmin = globalValues.webSite
                    + "check_admin.php"
                    + "?Usuario=" + usuario;

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

                string urlExistUsuario = globalValues.webSite
                    + "datos_pers_musico.php"
                    + "?tpBusqueda=S"
                    + "&idUsuario=" + usuario;

                string responseExiste = client.GetStringAsync(urlExistUsuario).Result;

                JObject regExiste = JObject.Parse(responseExiste);

                string tpMusico = regExiste["datosPersonalesM"]["tipo_musico"].ToString();

                if (string.IsNullOrEmpty(tpMusico))
                {
                    tpMusico = "S";
                }
                /**/

                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                db = new SQLiteConnection(databasePath);

                db.CreateTable<T_Registro>();

                db.Query<T_Registro>("Update USUARIOS set tpMusico ='"+ tpMusico +"', isActive='Y' where Id = '" + usuario + "'");

                if (isAdmin)
                {
                    Application.Current.MainPage = new NavigationPage(new Administracion(isAdmin, usuario));
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tpMusico));
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error en la validación de su código, intente más tarde", "Ok");
            }
        }
        private async void eliminarRegistro(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Eliminar Registro", "¿Está seguro de eliminar su registro?", "Si", "No");
            if (answer)
            {
                /*string urlRequest = globalValues.webSite
                    + "cancel_registro.php"
                    + "?email=" + usuario;
                string responseRegistro = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseRegistro);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                    db = new SQLiteConnection(databasePath);

                    db.CreateTable<T_Registro>();

                    db.Query<T_Registro>("Delete from USUARIOS where Id = '" + usuario + "'");
                    Application.Current.MainPage = new NavigationPage(new SelectUsuario());
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error en la eliminación de su registro, intente más tarde", "Ok");
                }*/
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                db = new SQLiteConnection(databasePath);

                db.CreateTable<T_Registro>();

                db.Query<T_Registro>("Delete from USUARIOS where Id = '" + usuario + "'");
                Application.Current.MainPage = new NavigationPage(new SelectUsuario());
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}