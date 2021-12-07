using encuentraMusicos.Classes;
using encuentraMusicos.Models;
using encuentraMusicos.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.OpenWhatsApp;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Contacto : ContentPage
    {
        double resolution;
        bool conexion;
        DireccionesViewModel vm;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        string busquedaOrig;
        double longSearchOrig;
        double latSearchOrig;
        string precioOrig;
        GruposMusicales selectedGrupoOrig;
        string idUsuarioOrig;
        string tpUsuarioOrig;
        string telefono, whatsapp, facebook, email;
        bool displayEmail=false;
        public Contacto(string busqueda, double longSearch, double latSearch, string precio, GruposMusicales selectedGrupo, string idUsuario, string tpUsuario)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            busquedaOrig=busqueda;
            longSearchOrig=longSearch;
            latSearchOrig=latSearch;
            precioOrig=precio;
            selectedGrupoOrig = selectedGrupo;
            idUsuarioOrig=idUsuario;
            tpUsuarioOrig=tpUsuario;

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

                lbDatosContacto.Text = "Contacta a " + selectedGrupoOrig.nombre_musico;

                string urlContactosMusico = globalValues.webSite
                        + "contacto_musico.php"
                        + "?tipoMov=S"
                        + "&Usuario=" + selectedGrupo.id_musico;

                string responseContacto = client.GetStringAsync(urlContactosMusico).Result;

                if (!responseContacto.Equals("[]"))
                {
                    JObject regPrecio = JObject.Parse(responseContacto);

                    lbTelefono.Text = regPrecio["contactoMusico"]["telefono"].ToString();
                    lbWhats.Text = regPrecio["contactoMusico"]["whatsapp"].ToString();
                    lbFB.Text = regPrecio["contactoMusico"]["facebook"].ToString();
                    lbEmail.Text = regPrecio["contactoMusico"]["email"].ToString();
                    telefono = regPrecio["contactoMusico"]["telefono"].ToString();
                    whatsapp = regPrecio["contactoMusico"]["whatsapp"].ToString();
                    facebook = regPrecio["contactoMusico"]["facebook"].ToString();
                    email = regPrecio["contactoMusico"]["email"].ToString();
                }

                if (string.IsNullOrEmpty(telefono))
                {
                    frTelefono.IsVisible = false;
                }
                else
                {
                    frTelefono.IsVisible = true;
                }

                if (string.IsNullOrEmpty(whatsapp))
                {
                    frWhatsApp.IsVisible = false;
                }
                else
                {
                    frWhatsApp.IsVisible = true;
                }

                if (string.IsNullOrEmpty(facebook))
                {
                    frFacebook.IsVisible = false;
                }
                else
                {
                    frFacebook.IsVisible = true;
                }

                if (string.IsNullOrEmpty(email))
                {
                    frEmail.IsVisible = false;
                }
                else
                {
                    frEmail.IsVisible = true;
                    datosEmail.IsVisible = false;
                }

                if (resolution > 2000000)
                {
                    lbDatosContacto.FontSize = 20;
                    lbTelefonoTitle.FontSize = 14;
                    lbTelefono.FontSize = 16;
                    lbWApp.FontSize = 14;
                    lbWhats.FontSize = 16;
                    lbFBTitle.FontSize = 14;
                    lbFB.FontSize = 16;
                    lbEmailTitle.FontSize = 14;
                    lbEmail.FontSize = 16;
                    lbEmailNombre.FontSize = 14;
                    enNombreCliente.FontSize = 16;
                    lbEmailEmail.FontSize = 14;
                    enEmailCliente.FontSize = 16;
                    lbEmailTelefono.FontSize = 14;
                    enTelefonoCliente.FontSize = 16;
                    lbEmailMensaje.FontSize = 14;
                    enMensajeCliente.FontSize = 16;
                    enviarEmail.FontSize = 18;
                }
                else
                {
                    lbDatosContacto.FontSize = 14;
                    lbTelefonoTitle.FontSize = 10;
                    lbTelefono.FontSize = 12;
                    lbWApp.FontSize = 10;
                    lbWhats.FontSize = 12;
                    lbFBTitle.FontSize = 10;
                    lbFB.FontSize = 12;
                    lbEmailTitle.FontSize = 10;
                    lbEmail.FontSize = 12;
                    lbEmailNombre.FontSize = 10;
                    enNombreCliente.FontSize = 12;
                    lbEmailEmail.FontSize = 10;
                    enEmailCliente.FontSize = 12;
                    lbEmailTelefono.FontSize = 10;
                    enTelefonoCliente.FontSize = 12;
                    lbEmailMensaje.FontSize = 10;
                    enMensajeCliente.FontSize = 12;
                    enviarEmail.FontSize = 14;
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
            Application.Current.MainPage = new NavigationPage(new DetalleGrupo(busquedaOrig, longSearchOrig, latSearchOrig, precioOrig, selectedGrupoOrig, idUsuarioOrig, tpUsuarioOrig));
        }
        private void openPhone(object sender, EventArgs e)
        {
            try
            {
                PhoneDialer.Open(lbTelefono.Text);
            }
            catch (FeatureNotSupportedException ex)
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "No soportado en este dispositivo", "Ok");
            }
            catch (Exception ex)
            {
                // Other error has occurred.  
            }
        }
        private async void OpenWhatsApp(object sender, EventArgs e)
        {
            try
            {
                Chat.Open("+52"+lbWhats.Text, "Hola, me interesa saber mas sobre tus servicios de músico");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void openFB(object sender, EventArgs e)
        {
            if (!await Launcher.TryOpenAsync("fb://facewebmodal/f?href="+lbFB.Text))
            {
                await Browser.OpenAsync(lbFB.Text);
            }
        }
        private void openEmail(object sender, EventArgs e)
        {
            if (displayEmail)
            {
                datosEmail.IsVisible = false;
                displayEmail = false;
            }
            else
            {
                datosEmail.IsVisible = true;
                displayEmail = true;
            }
        }
        private void enviarCorreo(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(enNombreCliente.Text))
            {
                if (!string.IsNullOrEmpty(enEmailCliente.Text) || !string.IsNullOrEmpty(enTelefonoCliente.Text))
                {
                    string urlContactosMusico = globalValues.webSite
                        + "contacto_mail.php"
                        + "?email=" + lbEmail.Text
                        + "&nombre=" + enNombreCliente.Text
                        + "&emailCliente=" + enEmailCliente.Text
                        + "&telefono=" + enTelefonoCliente.Text
                        + "&mensaje=" + enMensajeCliente.Text;

                    string responseContacto = client.GetStringAsync(urlContactosMusico).Result;

                    JObject regResponse = JObject.Parse(responseContacto);

                    string result = regResponse["success"].ToString();

                    if (result.Equals("1"))
                    {
                        Application.Current.MainPage.DisplayAlert("Mensaje Enviado", "El mensaje a " + selectedGrupoOrig.nombre_musico + " fue enviado con éxito", "Ok");
                        datosEmail.IsVisible = false;
                        displayEmail = false;
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Agregue teléfono o email para enviar mensaje", "Ok");
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Ingrese su nombre para enviar mensaje", "Ok");
            }
        }
        private void goInicio(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalBusqueda(idUsuarioOrig, tpUsuarioOrig));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}