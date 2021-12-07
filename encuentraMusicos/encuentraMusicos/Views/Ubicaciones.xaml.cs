using encuentraMusicos.Classes;
using encuentraMusicos.Models;
using encuentraMusicos.ViewModels;
using System;
using System.Collections.Generic;
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
    public partial class Ubicaciones : ContentPage
    {
        double resolution;
        bool conexion;
        string usuario;
        string tipoMusico;
        DireccionesViewModel vm;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public Ubicaciones(string idUsuario, string tpMusico)
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

                if (resolution > 2000000)
                {
                    lbUbicaciones.FontSize = 22;
                    nuevaUbicacionBtn.FontSize = 18;
                }
                else
                {
                    lbUbicaciones.FontSize = 14;
                    nuevaUbicacionBtn.FontSize = 14;
                }

                vm = new DireccionesViewModel(usuario);

                BindingContext = vm;

                listaUbicaciones.HeightRequest = vm.numDirecciones * 55;
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
        private void guardarDatos(object sender, EventArgs e)
        {

        }
        private void selectDireccion(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<object> currentSelectedEvent = e.CurrentSelection;
            Direcciones selectedDireccion = currentSelectedEvent.FirstOrDefault() as Direcciones;
            Application.Current.MainPage = new NavigationPage(new DetalleUbicacion(usuario, tipoMusico, selectedDireccion));
        }
        void agregaUbicacion(object sender, EventArgs e)
        {
            Direcciones empty = new Direcciones();
            empty.id_usuario = usuario;
            empty.titulo = "Nueva Ubicación";
            Application.Current.MainPage = new NavigationPage(new DetalleUbicacion(usuario, tipoMusico, empty));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}