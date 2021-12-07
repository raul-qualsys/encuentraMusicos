using encuentraMusicos.Classes;
using Newtonsoft.Json.Linq;
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
    public partial class Tarifas : ContentPage
    {
        bool conexion;
        double resolution;
        string usuario;
        string tipoMusico;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public Tarifas(string idUsuario, string tpMusico)
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

                string urlPreciosMusico = globalValues.webSite
                    + "precios_musico.php"
                    + "?tipoMov=S"
                    + "&Usuario=" + idUsuario;

                string responsePrecio = client.GetStringAsync(urlPreciosMusico).Result;

                if (!responsePrecio.Equals("[]"))
                {
                    JObject regPrecio = JObject.Parse(responsePrecio);

                    string precio = regPrecio["precioMusico"]["precio"].ToString();
                    if (!string.IsNullOrEmpty(precio))
                    {
                        enPrecio.Text = precio;
                    }

                    enTexto.Text = regPrecio["precioMusico"]["texto"].ToString();
                }

                if (resolution > 2000000)
                {
                    lbTarifas.FontSize = 22;
                    lbPrecio.FontSize = 14;
                    enPrecio.FontSize = 16;
                    lbTexto.FontSize = 14;
                    enTexto.FontSize = 16;
                    signoPesos.FontSize = 16;
                    guardarButton.FontSize = 20;
                }
                else
                {
                    lbTarifas.FontSize = 14;
                    lbPrecio.FontSize = 10;
                    enPrecio.FontSize = 12;
                    lbTexto.FontSize = 10;
                    enTexto.FontSize = 12;
                    signoPesos.FontSize = 10;
                    guardarButton.FontSize = 12;
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
        private void guardarDatos(object sender, EventArgs e)
        {
            botonGuardar.IsEnabled = false;
            if (!string.IsNullOrEmpty(enPrecio.Text))
            {
                string urlRequest = globalValues.webSite
                    + "precios_musico.php"
                    + "?tipoMov=U"
                    + "&Usuario=" + usuario
                    + "&precio=" + enPrecio.Text
                    + "&texto=" + enTexto.Text;

                string responseRegistro = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseRegistro);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                    botonGuardar.IsEnabled = true;
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "El precio no puede ir vacío", "Ok");
                botonGuardar.IsEnabled = true;
            }
        }
        private void precioChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Contains("."))
            {
                if (e.NewTextValue.Length - 1 - e.NewTextValue.IndexOf(".") > 2)
                {
                    var s = e.NewTextValue.Substring(0, e.NewTextValue.IndexOf(".") + 2 + 1);
                    enPrecio.Text = s;
                    enPrecio.SelectionLength = s.Length;
                }
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}