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
    public partial class GuiaCalificacion : ContentPage
    {
        bool conexion;
        double resolution;
        string usuario;
        string tipoMusico;
        public GuiaCalificacion(string idUsuario, string tpMusico)
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

                if (tipoMusico.Equals("S"))
                {
                    goClientes.Text = "¿Qué Dicen Mis Clientes?";
                }
                else
                {
                    goClientes.Text = "¿Qué Dicen Nuestros Clientes?";
                }

                if (resolution > 2000000)
                {
                    lbGuia.FontSize = 20;
                    intro0.FontSize = 16;
                    intro1.FontSize = 16;
                    intro2.FontSize = 16;
                    intro3.FontSize = 16;
                    intro4.FontSize = 16;
                    intro5.FontSize = 16;
                    intro6.FontSize = 16;
                    intro7.FontSize = 16;
                    regresarBtn.FontSize = 18;
                }
                else
                {
                    lbGuia.FontSize = 14;
                    intro0.FontSize = 12;
                    intro1.FontSize = 12;
                    intro2.FontSize = 16;
                    intro3.FontSize = 12;
                    intro4.FontSize = 12;
                    intro5.FontSize = 12;
                    intro6.FontSize = 12;
                    intro7.FontSize = 12;
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
        private void goOpiniones(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Opiniones(usuario, tipoMusico));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}