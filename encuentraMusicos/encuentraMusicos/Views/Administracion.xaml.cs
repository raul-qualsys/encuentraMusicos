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
    public partial class Administracion : ContentPage
    {
        bool conexion;
        bool isAdmin = false;
        double resolution;
        string usuario;
        string tipoMusico;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public Administracion(bool Admin, string idUsuario)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            isAdmin = Admin;

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

                string urlRequest = globalValues.webSite
                    + "tipo_musico.php"
                    + "?Usuario=" + usuario
                    + "&tipoMov=G";

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);
                tipoMusico = regResponse["tipoMusico"]["tipo_musico"].ToString();

                if (resolution > 2000000)
                {
                    lbAdmin.FontSize = 22;
                    lbCatalogos.FontSize = 18;
                    lbUsersAdmin.FontSize = 18;
                    lbReports.FontSize = 18;
                    lbPerfilMusico.FontSize = 18;
                    lbCerrarSesion.FontSize = 18;
                }
                else
                {
                    lbAdmin.FontSize = 14;
                    lbCatalogos.FontSize = 12;
                    lbUsersAdmin.FontSize = 12;
                    lbReports.FontSize = 12;
                    lbPerfilMusico.FontSize = 12;
                    lbCerrarSesion.FontSize = 12;
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
        private void goCatalogos(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Configuracion(usuario));
        }
        private void goAdminUsuarios(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new UsuariosAdmin(usuario, "", "T"));
        }
        private void goReportes(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Reportes(usuario));
        }
        private void goPerfilMusico(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario,tipoMusico));
        }
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private async void logOut(object sender, EventArgs e)
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);
            bool answer = await DisplayAlert("Cerrar Sesión", "¿Está seguro de cerrar sesión?", "Yes", "No");
            if (answer)
            {
                try
                {
                    db.DeleteAll<T_Registro>();
                    db.DeleteAll<T_MisGeneros>();
                    db.DeleteAll<T_MusicoContactado>();
                    db.DeleteAll<T_Notificaciones>();
                    db.DeleteAll<T_Resultados>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                DependencyService.Get<IClearCookies>().ClearAllCookies();
                Application.Current.MainPage = new NavigationPage(new SelectUsuario());
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}