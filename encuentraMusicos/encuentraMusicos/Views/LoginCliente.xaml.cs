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
    public partial class LoginCliente : ContentPage
    {
        double resolution;
        bool conexion;
        public LoginCliente()
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
                    btAtras.FontSize = 16;
                    lbTitulo1.FontSize = 26;
                    lbTitulo2.FontSize = 20;
                    lbTitulo2.Margin = new Thickness(85, 20, 5, 0);
                    lbInicioSesion.FontSize = 22;
                    lbInicioSesion.Margin = new Thickness(0, 50, 0, 0);
                    lbFbUsuario.FontSize = 18;
                    imFbUsuario.Margin = new Thickness(0, 0, 120, 0);
                    lbGoogleUsuario.FontSize = 18;
                    imGoogleUsuario.Margin = new Thickness(0, 0, 120, 0);
                    lbeMailUsuario.FontSize = 18;
                    imeMailUsuario.Margin = new Thickness(0, 0, 120, 0);
                    imLogoLogin.HeightRequest = 120;
                    imLogoLogin.Margin = new Thickness(0, 40, 0, 40);
                }
                else
                {
                    btAtras.FontSize = 10;
                    lbTitulo1.FontSize = 16;
                    lbTitulo2.FontSize = 12;
                    lbTitulo2.Margin = new Thickness(105, 20, 5, 0);
                    lbInicioSesion.FontSize = 16;
                    lbInicioSesion.Margin = new Thickness(0, 20, 0, 0);
                    lbFbUsuario.FontSize = 14;
                    imFbUsuario.Margin = new Thickness(0, 0, 80, 0);
                    lbGoogleUsuario.FontSize = 14;
                    imGoogleUsuario.Margin = new Thickness(0, 0, 80, 0);
                    lbeMailUsuario.FontSize = 14;
                    imeMailUsuario.Margin = new Thickness(0, 0, 80, 0);
                    imLogoLogin.HeightRequest = 80;
                    imLogoLogin.Margin = new Thickness(0, 20, 0, 20);
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
            Application.Current.MainPage = new NavigationPage(new SelectUsuario());
        }
        private void inicioCliente(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalBusqueda("",""));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}