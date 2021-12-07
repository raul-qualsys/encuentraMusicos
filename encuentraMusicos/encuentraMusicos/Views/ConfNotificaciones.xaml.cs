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
    public partial class ConfNotificaciones : ContentPage
    {
        bool conexion;
        string usuario;
        double resolution;
        string diaSeleccionado;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public ConfNotificaciones(string idUsuario)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;

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

                string urlRequest = globalValues.webSite
                    + "conf_notificaciones.php"
                    + "?tpBusqueda=S";

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                if (!responseSelect.Equals("[]"))
                {
                    JObject regResponse = JObject.Parse(responseSelect);

                    string diaNotif = regResponse["confNotificaciones"]["dia_notif_musico"].ToString();

                    diaSeleccionado = diaNotif;

                    switch (diaNotif)
                    {
                        case "DOM":
                            listaDias.SelectedItem = "Domingo";
                            break;
                        case "LUN":
                            listaDias.SelectedItem = "Lunes";
                            break;
                        case "MAR":
                            listaDias.SelectedItem = "Martes";
                            break;
                        case "MIE":
                            listaDias.SelectedItem = "Miércoles";
                            break;
                        case "JUE":
                            listaDias.SelectedItem = "Jueves";
                            break;
                        case "VIE":
                            listaDias.SelectedItem = "Viernes";
                            break;
                        case "SAB":
                            listaDias.SelectedItem = "Sábado";
                            break;
                    }

                    enDias.Text = regResponse["confNotificaciones"]["dias_notif_valoracion"].ToString();
                }

                if (resolution > 2000000)
                {
                    lbConfNotif.FontSize = 22;
                    lbDiaNotif.FontSize = 14;
                    listaDias.FontSize = 16;
                    lbDiasOpinion.FontSize = 14;
                    enDias.FontSize = 16;
                    enviarBtn.FontSize = 18;
                    regresarBtn.FontSize = 18;
                }
                else
                {
                    lbConfNotif.FontSize = 12;
                    lbDiaNotif.FontSize = 10;
                    listaDias.FontSize = 12;
                    lbDiasOpinion.FontSize = 10;
                    enDias.FontSize = 12;
                    enviarBtn.FontSize = 12;
                    regresarBtn.FontSize = 12;
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
            Application.Current.MainPage = new NavigationPage(new Configuracion(usuario));
        }
        private void guardaConfig(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(enDias.Text))
            {
                string urlRequest = globalValues.webSite
                    + "conf_notificaciones.php"
                    + "?tpBusqueda=U"
                    + "&diaNotif="+ diaSeleccionado
                    + "&diasValoracion="+enDias.Text;

                string responseUpdate = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseUpdate);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    Application.Current.MainPage = new NavigationPage(new Configuracion(usuario));
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "El número de días no puede ir vacío", "Ok");
                enDias.Focus();
            }
        }
        private void diaSelected(object sender, EventArgs e)
        {
            switch (listaDias.SelectedItem)
            {
                case "Domingo":
                    diaSeleccionado = "DOM";
                    break;
                case "Lunes":
                    diaSeleccionado = "LUN";
                    break;
                case "Martes":
                    diaSeleccionado = "MAR";
                    break;
                case "Miércoles":
                    diaSeleccionado = "MIE";
                    break;
                case "Jueves":
                    diaSeleccionado = "JUE";
                    break;
                case "Viernes":
                    diaSeleccionado = "VIE";
                    break;
                case "Sábado":
                    diaSeleccionado = "SAB";
                    break;
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}