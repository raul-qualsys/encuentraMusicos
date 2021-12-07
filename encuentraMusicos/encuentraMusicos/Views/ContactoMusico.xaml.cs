using encuentraMusicos.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactoMusico : ContentPage
    {
        bool conexion;
        double resolution;
        string usuario;
        string tipoMusico;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public ContactoMusico(string idUsuario, string tpMusico)
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

                string urlContactosMusico = globalValues.webSite
                    + "contacto_musico.php"
                    + "?tipoMov=S"
                    + "&Usuario=" + idUsuario;

                string responseContacto = client.GetStringAsync(urlContactosMusico).Result;

                if (!responseContacto.Equals("[]"))
                {
                    JObject regPrecio = JObject.Parse(responseContacto);

                    enTelefono.Text = regPrecio["contactoMusico"]["telefono"].ToString();
                    enWhats.Text = regPrecio["contactoMusico"]["whatsapp"].ToString();
                    enFB.Text = regPrecio["contactoMusico"]["facebook"].ToString();
                    enEmail.Text = regPrecio["contactoMusico"]["email"].ToString();
                }

                if (resolution > 2000000)
                {
                    lbDatosContacto.FontSize = 22;
                    lbTelefono.FontSize = 14;
                    enTelefono.FontSize = 16;
                    lbWApp.FontSize = 14;
                    enWhats.FontSize = 16;
                    lbFB.FontSize = 14;
                    enFB.FontSize = 16;
                    lbEmail.FontSize = 14;
                    enEmail.FontSize = 16;
                    guardarButton.FontSize = 20;
                }
                else
                {
                    lbDatosContacto.FontSize = 14;
                    lbTelefono.FontSize = 10;
                    enTelefono.FontSize = 12;
                    lbWApp.FontSize = 10;
                    enWhats.FontSize = 12;
                    lbFB.FontSize = 10;
                    enFB.FontSize = 12;
                    lbEmail.FontSize = 10;
                    enEmail.FontSize = 12;
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
            guardaDatos.IsEnabled = false;
            bool isEmail = Regex.IsMatch(enEmail.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(enTelefono.Text)&& string.IsNullOrEmpty(enWhats.Text)&& string.IsNullOrEmpty(enFB.Text)&& string.IsNullOrEmpty(enEmail.Text))
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Agregue algún dato de contacto", "Ok");
                guardaDatos.IsEnabled = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(enEmail.Text) && isEmail || string.IsNullOrEmpty(enEmail.Text))
                {
                    string urlRequest = globalValues.webSite
                    + "contacto_musico.php"
                    + "?tipoMov=U"
                    + "&Usuario=" + usuario
                    + "&telefono=" + enTelefono.Text
                    + "&WA=" + enWhats.Text
                    + "&FB=" + enFB.Text
                    + "&EM=" + enEmail.Text;

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
                        guardaDatos.IsEnabled = true;
                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Ingrese un correo electrónico válido", "Ok");
                    guardaDatos.IsEnabled = true;
                }
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}