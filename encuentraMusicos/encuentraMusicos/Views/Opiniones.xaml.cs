using encuentraMusicos.ViewModels;
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
    public partial class Opiniones : ContentPage
    {
        bool conexion;
        double resolution;
        string usuario;
        string tipoMusico;
        public Opiniones(string idUsuario, string tpMusico)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            tipoMusico = tpMusico;

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

                ValoracionesViewModel vm = new ValoracionesViewModel(idUsuario);
                BindingContext = vm;

                if (vm.numValoraciones>0)
                {
                    lbValClientes.IsVisible = true;
                }
                else
                {
                    lbValClientes.IsVisible = false;
                }

                collectionMusicos.HeightRequest = 70 * vm.numValoraciones;

                if (vm.promValoraciones < 20)
                {
                    star1.Source = "star1gray.png";
                    star2.Source = "star1gray.png";
                    star3.Source = "star1gray.png";
                    star4.Source = "star1gray.png";
                    star5.Source = "star1gray.png";
                }

                if (vm.promValoraciones >= 20 && vm.promValoraciones < 40)
                {
                    star1.Source = "star1.png";
                    star2.Source = "star1gray.png";
                    star3.Source = "star1gray.png";
                    star4.Source = "star1gray.png";
                    star5.Source = "star1gray.png";
                }

                if (vm.promValoraciones >= 40 && vm.promValoraciones < 60)
                {
                    star1.Source = "star1.png";
                    star2.Source = "star1.png";
                    star3.Source = "star1gray.png";
                    star4.Source = "star1gray.png";
                    star5.Source = "star1gray.png";
                }

                if (vm.promValoraciones >= 60 && vm.promValoraciones < 80)
                {
                    star1.Source = "star1.png";
                    star2.Source = "star1.png";
                    star3.Source = "star1.png";
                    star4.Source = "star1gray.png";
                    star5.Source = "star1gray.png";
                }

                if (vm.promValoraciones >= 80 && vm.promValoraciones < 100)
                {
                    star1.Source = "star1.png";
                    star2.Source = "star1.png";
                    star3.Source = "star1.png";
                    star4.Source = "star1.png";
                    star5.Source = "star1gray.png";
                }

                if (vm.promValoraciones == 100)
                {
                    star1.Source = "star1.png";
                    star2.Source = "star1.png";
                    star3.Source = "star1.png";
                    star4.Source = "star1.png";
                    star5.Source = "star1.png";
                }

                if (resolution > 2000000)
                {
                    lbValoraciones.FontSize = 22;
                    lbValGeneral.FontSize = 18;
                    lbValClientes.FontSize = 18;
                    regresarBtn.FontSize = 18;
                }
                else
                {
                    lbValoraciones.FontSize = 16;
                    lbValGeneral.FontSize = 14;
                    lbValClientes.FontSize = 14;
                    regresarBtn.FontSize = 14;
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
            Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}