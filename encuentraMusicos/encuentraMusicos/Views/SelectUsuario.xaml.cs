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
    public partial class SelectUsuario : ContentPage
    {
        bool conexion;
        double resolution;
        public SelectUsuario()
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
                    lbLoginCliente.FontSize = 18;
                    lbLoginMusico.FontSize = 18;
                    imLoginCliente.Margin = new Thickness(0, 0, 80, 0);
                    imLoginMusico.Margin = new Thickness(0, 0, 80, 0);
                }
                else
                {
                    lbLoginCliente.FontSize = 14;
                    lbLoginMusico.FontSize = 14;
                    imLoginCliente.Margin = new Thickness(0, 0, 40, 0);
                    imLoginMusico.Margin = new Thickness(0, 0, 40, 0);
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
        private void goLoginCliente(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalBusqueda("",""));
        }
        private void goLoginMusico(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new LoginMusico());
        }
        protected override bool OnBackButtonPressed() => true;
    }
}