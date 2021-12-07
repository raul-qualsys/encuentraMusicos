using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class updateApp : ContentPage
    {
        bool conexion;
        double resolution;
        string url;
        public updateApp()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

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

                }
                else
                {

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

                }
                else
                {

                }
            }
        }
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private async void goUpdate(object sender, EventArgs e)
        {
            var location = RegionInfo.CurrentRegion.Name.ToLower();
            if (Device.RuntimePlatform == Device.iOS)
            {
                url = "https://itunes.apple.com/" + location + "/app/encuentramusicos/id1578323852?mt=8";
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
            }
            if (Device.RuntimePlatform == Device.Android)
            {
                url = "https://play.google.com/store/apps/details?id=com.falyMusic.encuentramusicos";
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
            }
        }
    }
}