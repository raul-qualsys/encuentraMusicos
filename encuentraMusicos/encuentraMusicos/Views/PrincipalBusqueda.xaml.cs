using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System.IO;
using SQLite;
using encuentraMusicos.DataBase.Tables;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrincipalBusqueda : ContentPage
    {
        bool conexion;
        Pin pin;
        CancellationTokenSource cts;
        double resolution;
        double longitudeSearch;
        double latitudeSearch;
        string precioSearch;
        string tipoMusico;
        string usuario;
        bool ubicManual = false;
        public PrincipalBusqueda(string idUsuario, string tpMusico)
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

                agregarUbicacion.Text = "Escribir Dirección";

                if (Device.RuntimePlatform == Device.iOS)
                {
                    scrPrincipal.InputTransparent = false;
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    scrPrincipal.InputTransparent = true;
                }

                if (resolution > 2000000)
                {
                    lbTitulo1.FontSize = 22;
                    lbTitulo2.FontSize = 22;
                    detalleUbicacion.FontSize = 14;
                    lbRangoPrecios.FontSize = 22;
                    precioInicial.FontSize = 16;
                    precioFinal.FontSize = 16;
                    lbBtnBuscar.FontSize = 18;
                    lbBtnBuscar0.FontSize = 18;
                    MapView.HeightRequest = 250;
                    busqueda.FontSize = 14;
                    agregarUbicacion.FontSize = 18;
                }
                else
                {
                    lbTitulo1.FontSize = 14;
                    lbTitulo2.FontSize = 14;
                    detalleUbicacion.FontSize = 10;
                    lbRangoPrecios.FontSize = 14;
                    precioInicial.FontSize = 10;
                    precioFinal.FontSize = 10;
                    lbBtnBuscar.FontSize = 14;
                    lbBtnBuscar0.FontSize = 14;
                    MapView.HeightRequest = 180;
                    busqueda.FontSize = 10;
                    agregarUbicacion.FontSize = 12;
                }

                getCurrentLocation();

                precioFinal.Text = "+ $" + precioSlider.Value;
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
        async Task getCurrentLocation()
        {
            try
            {
                MapView.Pins.Remove(pin);
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(0.5));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        pin = new Pin
                        {
                            Label = placemark.Thoroughfare + " " + placemark.FeatureName,
                            Address = placemark.SubLocality + " " + placemark.Locality,
                            Type = PinType.Place,
                            Position = new Position(location.Latitude, location.Longitude)
                        };
                        MapView.Pins.Add(pin);

                        detalleUbicacion.Text = placemark.Thoroughfare
                            + " " + placemark.FeatureName
                            + ", C.P. " + placemark.PostalCode
                            + ", Col. " + placemark.SubLocality
                            + ", " + placemark.Locality
                            + ", " + placemark.AdminArea
                            + ", " + placemark.CountryName;

                        longitudeSearch = location.Longitude;
                        latitudeSearch = location.Latitude;
                    }

                    MapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMiles(0.3)));
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
        async void agregaPin(object sender, MapClickedEventArgs e)
        {
            try
            {
                MapView.Pins.Remove(pin);
                var placemarks = await Geocoding.GetPlacemarksAsync(e.Position.Latitude, e.Position.Longitude);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    pin = new Pin
                    {
                        Label = placemark.Thoroughfare + " " + placemark.FeatureName,
                        Address = placemark.SubLocality + " " + placemark.Locality,
                        Type = PinType.Place,
                        Position = new Position(e.Position.Latitude, e.Position.Longitude)
                    };

                    detalleUbicacion.Text = placemark.Thoroughfare
                            + " " + placemark.FeatureName
                            + ", C.P. " + placemark.PostalCode
                            + ", Col. " + placemark.SubLocality
                            + ", " + placemark.Locality
                            + ", " + placemark.AdminArea
                            + ", " + placemark.CountryName;

                    MapView.Pins.Add(pin);

                    longitudeSearch = e.Position.Longitude;
                    latitudeSearch = e.Position.Latitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            double value = Math.Round(args.NewValue,0);
            precioFinal.Text = "$" + value;
            if (value >= 20000)
            {
                precioFinal.Text = "+ $" + value;
            }
        }
        private void goBusqueda(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(busqueda.Text))
            {
                precioSearch = precioSlider.Value.ToString();
                Application.Current.MainPage = new NavigationPage(new ResultadosBusqueda(busqueda.Text, longitudeSearch, latitudeSearch, precioSearch, usuario, tipoMusico));
            }
            else
            {
                busqueda.Focus();
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "La búsqueda no puede ir vacía", "Ok");
            }
        }
        private void goCurrent(object sender, EventArgs e)
        {
            getCurrentLocation();
        }
        private void goBack(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usuario))
            {
                Application.Current.MainPage = new NavigationPage(new SelectUsuario());
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (conexion)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                var db = new SQLiteConnection(databasePath);

                db.CreateTable<T_MusicoContactado>();

                var resultado = db.Query<T_MusicoContactado>("SELECT * from CONTACTADOS where estatus='I'");

                int cursor = 0;

                foreach (var s in resultado)
                {
                    DateTime fechac = s.fecha_contacto;
                    if (DateTime.Now >= s.fecha_contacto)
                    {
                        cursor++;
                        if (cursor == 1)
                        {
                            var pr = new popUpValoracion(usuario, s.id_contacto, s.id_usuario, s.nombre_usuario);
                            var scaleAnimation = new ScaleAnimation
                            {
                                PositionIn = MoveAnimationOptions.Right,
                                PositionOut = MoveAnimationOptions.Left
                            };

                            pr.Animation = scaleAnimation;
                            await PopupNavigation.PushAsync(pr);
                            break;
                        }
                    }
                }
            }
        }
        private async void actualizaMapa(object sender, EventArgs e)
        {
            if (ubicManual)
            {
                agregarUbicacion.Text = "Escribir dirección";
                try
                {
                    var address = detalleUbicacion.Text;
                    var locations = await Geocoding.GetLocationsAsync(address);

                    var location = locations?.FirstOrDefault();
                    if (location != null)
                    {
                        MapView.Pins.Remove(pin);
                        MapView.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(
                                Convert.ToDouble(location.Latitude),
                                Convert.ToDouble(location.Longitude)),
                            Distance.FromMiles(0.3)));

                        pin = new Pin
                        {
                            Label = "Punto de Referencia",
                            Type = PinType.Place,
                            Position = new Position(location.Latitude, location.Longitude)
                        };
                        MapView.Pins.Add(pin);

                        longitudeSearch = location.Longitude;
                        latitudeSearch = location.Latitude;

                        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                        var placemark = placemarks?.FirstOrDefault();
                        if (placemark != null)
                        {
                            detalleUbicacion.Text = placemark.Thoroughfare + " "
                            + placemark.FeatureName + " "
                            + placemark.PostalCode + " "
                            + placemark.SubLocality + " "
                            + placemark.Locality + " "
                            + placemark.AdminArea + " "
                            + placemark.CountryName;
                        }
                    }
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    Console.WriteLine(fnsEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                ubicManual = false;
            }
            else
            {
                agregarUbicacion.Text = "Ver en el mapa";
                detalleUbicacion.Text = "";
                detalleUbicacion.Focus();
                ubicManual = true;
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}