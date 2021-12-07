using encuentraMusicos.Models;
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
    public partial class ResultadosBusqueda : ContentPage
    {
        BusquedaViewModel vm;
        bool conexion;
        string strBusqueda;
        double resolution;
        string usuario;
        string tipoUsuario;
        double longitudeSearch;
        double latitudeSearch;
        string precioSearch;
        public ResultadosBusqueda(string busqueda, double longSearch, double latSearch, string precio, string idUsuario, string tpUsuario)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            longitudeSearch = longSearch;
            latitudeSearch = latSearch;
            usuario = idUsuario;
            tipoUsuario = tpUsuario;
            precioSearch = precio;

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
                    btnBack.WidthRequest = 30;
                    lbTituloBusqueda.FontSize = 22;
                    palabraBusqueda.FontSize = 22;
                    lbResultados.FontSize = 18;
                    sinResultadosLbl.FontSize = 18;
                    gridPrincipal.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    btnBack.WidthRequest = 25;
                    lbTituloBusqueda.FontSize = 12;
                    palabraBusqueda.FontSize = 12;
                    lbResultados.FontSize = 12;
                    sinResultadosLbl.FontSize = 12;
                    gridPrincipal.Margin = new Thickness(0, 0, 0, 20);
                }

                strBusqueda = busqueda;

                vm = new BusquedaViewModel(busqueda, longSearch, latSearch, precio);

                BindingContext = vm;

                palabraBusqueda.Text = busqueda;

                if (vm.numResultados>0)
                {
                    sinResultadosLbl.IsVisible = false;
                    lbResultados.IsVisible = true;
                    collectionMusicos.IsVisible = true;
                }
                else
                {
                    sinResultadosLbl.IsVisible = true;
                    lbResultados.IsVisible = false;
                    collectionMusicos.IsVisible = false;
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
            Application.Current.MainPage = new NavigationPage(new PrincipalBusqueda(usuario, tipoUsuario));
        }
        void selectGrupo(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<object> currentSelectedEvent = e.CurrentSelection;
            GruposMusicales selectedGrupo = currentSelectedEvent.FirstOrDefault() as GruposMusicales;
            Application.Current.MainPage = new NavigationPage(new DetalleGrupo(strBusqueda, longitudeSearch, latitudeSearch, precioSearch, selectedGrupo, usuario, tipoUsuario));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}