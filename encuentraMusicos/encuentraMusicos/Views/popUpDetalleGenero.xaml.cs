using encuentraMusicos.Classes;
using encuentraMusicos.Models;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
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
    public partial class popUpDetalleGenero //: ContentPage
    {
        string usuario;
        string busquedaOrig;
        string swValue;
        string movimiento;
        double maxCdValue;
        string selectOption;
        double resolution;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        GenerosAdmin selected = new GenerosAdmin();
        public popUpDetalleGenero(GenerosAdmin generoSelected, string idUsuario, string busqueda, string tipoMov, double maxCd, string selOption)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            busquedaOrig = busqueda;
            movimiento = tipoMov;
            maxCdValue = maxCd;
            selected = generoSelected;
            selectOption = selOption;

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;

            if (resolution > 2000000)
            {
                lbEdGenero.FontSize = 18;
                lbDescripcion.FontSize = 14;
                enGenero.FontSize = 16;
                lbEstatus.FontSize = 14;
                estatusValue.FontSize = 14;
            }
            else
            {
                lbEdGenero.FontSize = 14;
                lbDescripcion.FontSize = 10;
                enGenero.FontSize = 12;
                lbEstatus.FontSize = 10;
                estatusValue.FontSize = 10;
            }

            if (tipoMov.Equals("update"))
            {
                lbEdGenero.Text = "Editar " + generoSelected.descripcion;
                enGenero.Text = generoSelected.descripcion;

                if (generoSelected.estatus.Equals("A"))
                {
                    swEstatus.IsToggled = true;
                    estatusValue.Text = "Activo";
                    swValue = "A";
                }
                else
                {
                    swEstatus.IsToggled = false;
                    estatusValue.Text = "Inactivo";
                    swValue = "I";
                }
            }
            else
            {
                swEstatus.IsToggled = true;
                lbEdGenero.Text = "Nuevo Género Musical";
                estatusValue.Text = "Activo";
                swValue = "A";
            }
        }
        private void swEstatusToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                estatusValue.Text = "Activo";
                swValue = "A";
            }
            else
            {
                estatusValue.Text = "Inactivo";
                swValue = "I";
            }
        }
        private async void cerrarPopUp(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
            Application.Current.MainPage = new NavigationPage(new GenerosMusicalesAdmin(usuario, busquedaOrig, selectOption));
        }
        private async void guardaGenero(object sender, EventArgs e)
        {
            double codeEnviado;
            string tpMov;

            if (movimiento.Equals("update"))
            {
                codeEnviado = Convert.ToDouble(selected.code);
                tpMov = "U";
            }
            else
            {
                codeEnviado = maxCdValue+1;
                tpMov = "A";
            }

            if (!string.IsNullOrEmpty(enGenero.Text))
            {
                string urlRequest = globalValues.webSite
                    + "admin_catalog.php"
                    + "?tpBusqueda="+ tpMov
                    + "&tpCatalogo=generosMusicales"
                    + "&Codigo=" + codeEnviado
                    + "&descripcion=" + enGenero.Text
                    + "&estatus=" + swValue;

                string responseUpdate = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseUpdate);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    await PopupNavigation.PopAsync();
                    Application.Current.MainPage = new NavigationPage(new GenerosMusicalesAdmin(usuario, busquedaOrig, selectOption));
                }
                else if(result.Equals("2"))
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "El género "+enGenero.Text+" ya existe", "Ok");
                    await PopupNavigation.PopAsync();
                    Application.Current.MainPage = new NavigationPage(new GenerosMusicalesAdmin(usuario, busquedaOrig, selectOption));
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                    await PopupNavigation.PopAsync();
                    Application.Current.MainPage = new NavigationPage(new GenerosMusicalesAdmin(usuario, busquedaOrig, selectOption));
                }
            }
            else
            {
                enGenero.Focus();
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Nombre de género no puede ir vacío", "Ok");
            }
        }
    }
}