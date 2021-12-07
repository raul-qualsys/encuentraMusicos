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
    public partial class popUpDetalleMusico// : ContentPage
    {
        string usuario;
        string busquedaOrig;
        string selectOption;
        string swValue;
        double resolution;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        MusicosAdmin usuarioSelected = new MusicosAdmin();
        public popUpDetalleMusico(MusicosAdmin musicoSelected, string idUsuario, string busqueda, string selOption)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            busquedaOrig = busqueda;
            selectOption = selOption;
            usuarioSelected = musicoSelected;

            lbEdMusico.Text = usuarioSelected.nombre_musico;
            swValue = usuarioSelected.is_active;
            imgMusico.Source = usuarioSelected.nombre_media;

            if (swValue.Equals("Y"))
            {
                swEstatus.IsToggled = true;
                estatusValue.Text = "Activo";
            }
            else
            {
                swEstatus.IsToggled = false;
                estatusValue.Text = "Suspendido";
            }

            if (usuarioSelected.valoracion < 20)
            {
                star1.IsVisible = false;
                star2.IsVisible = false;
                star3.IsVisible = false;
                star4.IsVisible = false;
                star5.IsVisible = false;
            }

            if (usuarioSelected.valoracion >= 20 && usuarioSelected.valoracion < 40)
            {
                star1.IsVisible = true;
                star2.IsVisible = false;
                star3.IsVisible = false;
                star4.IsVisible = false;
                star5.IsVisible = false;
            }

            if (usuarioSelected.valoracion >= 40 && usuarioSelected.valoracion < 60)
            {
                star1.IsVisible = true;
                star2.IsVisible = true;
                star3.IsVisible = false;
                star4.IsVisible = false;
                star5.IsVisible = false;
            }

            if (usuarioSelected.valoracion >= 60 && usuarioSelected.valoracion < 80)
            {
                star1.IsVisible = true;
                star2.IsVisible = true;
                star3.IsVisible = true;
                star4.IsVisible = false;
                star5.IsVisible = false;
            }

            if (usuarioSelected.valoracion >= 80 && usuarioSelected.valoracion < 100)
            {
                star1.IsVisible = true;
                star2.IsVisible = true;
                star3.IsVisible = true;
                star4.IsVisible = true;
                star5.IsVisible = false;
            }

            if (usuarioSelected.valoracion == 100)
            {
                star1.IsVisible = true;
                star2.IsVisible = true;
                star3.IsVisible = true;
                star4.IsVisible = true;
                star5.IsVisible = true;
            }

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;

            if (resolution > 2000000)
            {
                lbEdMusico.FontSize = 18;
            }
            else
            {
                lbEdMusico.FontSize = 14;
            }
        }
        private async void cerrarPopUp(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
            Application.Current.MainPage = new NavigationPage(new UsuariosAdmin(usuario, busquedaOrig, selectOption));
        }
        private async void guardaMusico(object sender, EventArgs e)
        {
            string urlRequest = globalValues.webSite
            + "admin_musicos.php"
            + "?tpBusqueda=U"
            + "&idUsuario=" + usuarioSelected.id_usuario
            + "&isActive=" + swValue;

            string responseUpdate = client.GetStringAsync(urlRequest).Result;

            JObject regResponse = JObject.Parse(responseUpdate);

            string result = regResponse["success"].ToString();

            if (result.Equals("1"))
            {
                await PopupNavigation.PopAsync();
                Application.Current.MainPage = new NavigationPage(new UsuariosAdmin(usuario, busquedaOrig, selectOption));
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                await PopupNavigation.PopAsync();
                Application.Current.MainPage = new NavigationPage(new UsuariosAdmin(usuario, busquedaOrig, selectOption));
            }
        }
        private void swEstatusToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                estatusValue.Text = "Activo";
                swValue = "Y";
            }
            else
            {
                estatusValue.Text = "Suspendido";
                swValue = "N";
            }
        }
    }
}