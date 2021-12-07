using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuracion : ContentPage
    {
        bool conexion;
        string usuario;
        double resolution;
        public Configuracion(string idUsuario)
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
                    lbCatalogos.FontSize = 22;
                    cat1.FontSize = 18;
                    notif1.FontSize = 18;
                    regresarBtn.FontSize = 18;
                    frGeneros.Margin = new Thickness(-25, 0, 195, 0);
                    frNotif.Margin = new Thickness(-25, 0, 195, 0);
                }
                else
                {
                    lbCatalogos.FontSize = 16;
                    cat1.FontSize = 14;
                    notif1.FontSize = 14;
                    regresarBtn.FontSize = 14;
                    frGeneros.Margin = new Thickness(-25, 0, 125, 0);
                    frNotif.Margin = new Thickness(-25, 0, 125, 0);
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
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private void goBack(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Administracion(true, usuario));
        }
        private void goGenerosAdmin(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GenerosMusicalesAdmin(usuario,"","T"));
        }
        private void goNotifAdmin(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ConfNotificaciones(usuario));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}