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
    public partial class LoginMusico : ContentPage
    {
        double resolution;
        bool conexion;
        public LoginMusico()
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
                    lbFbMusico.FontSize = 18;
                    lbApMusico.FontSize = 18;
                    imFbMusico.Margin = new Thickness(0, 0, 120, 0);
                    imApMusico.Margin = new Thickness(0, 0, 120, 0);
                    lbGoogleMusico.FontSize = 18;
                    imGoogleMusico.Margin = new Thickness(0, 0, 120, 0);
                    lbeMailMusico.FontSize = 18;
                    imeMailUsuario.Margin = new Thickness(0, 0, 120, 0);
                    imLogoLogin.HeightRequest = 120;
                    imLogoLogin.Margin = new Thickness(0, 40, 0, 40);
                }
                else
                {
                    btAtras.FontSize = 10;
                    lbTitulo1.FontSize = 16;
                    lbTitulo2.FontSize = 12;
                    lbTitulo2.Margin = new Thickness(65, 20, 5, 0);
                    lbInicioSesion.FontSize = 16;
                    lbInicioSesion.Margin = new Thickness(0, 20, 0, 0);
                    lbFbMusico.FontSize = 14;
                    lbApMusico.FontSize = 14;
                    imFbMusico.Margin = new Thickness(0, 0, 80, 0);
                    imApMusico.Margin= new Thickness(0, 0, 80, 0);
                    lbGoogleMusico.FontSize = 14;
                    imGoogleMusico.Margin = new Thickness(0, 0, 80, 0);
                    lbeMailMusico.FontSize = 14;
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
        private void goPerfilMusico(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PerfilMusico("", "S"));
        }
        private void goLoginApple(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new LoginAP());
        }
        private void goLoginFacebook(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                Application.Current.MainPage = new NavigationPage(new LoginFB());
            }
            if (Device.RuntimePlatform == Device.Android)
            {
                Application.Current.MainPage = new NavigationPage(new FBAndLogin());
            }
        }
        private void goLoginEMail(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new loginEM());
        }
        private void goLoginGMail(object sender, EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new loginGM());
        }
        protected override bool OnBackButtonPressed() => true;
    }
}