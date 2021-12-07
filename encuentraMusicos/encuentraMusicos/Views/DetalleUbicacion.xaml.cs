using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.Models;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalleUbicacion : ContentPage
    {
        bool conexion;
        double resolution = 0;
        bool pinAdded = false;
        Pin pin;
        CancellationTokenSource cts;
        Direcciones selected = new Direcciones();
        string usuario;
        string tipoMusico;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        double longMusico=0;
        double latMusico=0;
        bool currentIsClicked=false;
        public DetalleUbicacion(string idUsuario, string tpMusico, Direcciones selectedDireccion)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            selected = selectedDireccion;
            //Console.WriteLine(selectedDireccion.id_ubicacion+" - "+selected.id_ubicacion);

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

                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                var db = new SQLiteConnection(databasePath);

                var loadCatalog = db.Query<T_Catalogos>("SELECT * from CATALOGOS where tipo_translate='ESTADOS_MX'");

                if (loadCatalog.Count() > 0)
                {
                    foreach (var k in loadCatalog)
                    {
                        pkEstado.Items.Add(k.descripcion);
                    }
                }

                if (string.IsNullOrEmpty(selectedDireccion.id_ubicacion))
                {
                    tituloEntry.IsEnabled = true;
                    frEliminar.IsVisible = false;
                    longMusico = selectedDireccion.longitude;
                    latMusico = selectedDireccion.latitude;
                }
                else
                {
                    tituloEntry.IsEnabled = false;
                    frEliminar.IsVisible = true;
                }

                getCurrentLocation();

                ubicacionLbl.Text = selectedDireccion.titulo;
                tituloEntry.Text = selectedDireccion.titulo;
                calleEntry.Text = selectedDireccion.calleno;
                postalEntry.Text = selectedDireccion.postal;
                coloniaEntry.Text = selectedDireccion.colonia;
                municipioEntry.Text = selectedDireccion.municipio;
                pkEstado.SelectedItem = selectedDireccion.estado;
                paisEntry.Text = selectedDireccion.pais;
                distanciaEntry.Text = selectedDireccion.distancia;

                longMusico = selectedDireccion.longitude;
                latMusico = selectedDireccion.latitude;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    principalScrollview.InputTransparent = false;
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    principalScrollview.InputTransparent = true;
                }

                if (resolution > 2000000)
                {
                    ubicacionLbl.FontSize = 20;
                    tituloLbl.FontSize = 14;
                    tituloEntry.FontSize = 16;
                    calleLbl.FontSize = 14;
                    calleEntry.FontSize = 16;
                    postalLbl.FontSize = 14;
                    postalEntry.FontSize = 16;
                    coloniaLbl.FontSize = 14;
                    coloniaEntry.FontSize = 16;
                    municipioLbl.FontSize = 14;
                    municipioEntry.FontSize = 16;
                    estadoLbl.FontSize = 14;
                    pkEstado.FontSize = 16;
                    paisLbl.FontSize = 14;
                    paisEntry.FontSize = 16;
                    distanciaLbl.FontSize = 12;
                    distanciaEntry.FontSize = 16;
                    lblKm.FontSize = 14;
                    actualizarMapalb1.FontSize = 14;
                    actualizarMapalb2.FontSize = 14;
                    eliminarBtn.FontSize = 18;
                    guardaBtn.FontSize = 18;
                    cancelarBtn.FontSize = 18;
                    verTutorial.FontSize = 16;
                }
                else
                {
                    ubicacionLbl.FontSize = 14;
                    tituloLbl.FontSize = 10;
                    tituloEntry.FontSize = 12;
                    calleLbl.FontSize = 10;
                    calleEntry.FontSize = 12;
                    postalLbl.FontSize = 10;
                    postalEntry.FontSize = 12;
                    coloniaLbl.FontSize = 10;
                    coloniaEntry.FontSize = 12;
                    municipioLbl.FontSize = 10;
                    municipioEntry.FontSize = 12;
                    estadoLbl.FontSize = 10;
                    pkEstado.FontSize = 12;
                    paisLbl.FontSize = 10;
                    paisEntry.FontSize = 12;
                    distanciaLbl.FontSize = 10;
                    distanciaEntry.FontSize = 12;
                    lblKm.FontSize = 10;
                    actualizarMapalb1.FontSize = 10;
                    actualizarMapalb2.FontSize = 10;
                    eliminarBtn.FontSize = 14;
                    guardaBtn.FontSize = 14;
                    cancelarBtn.FontSize = 14;
                    verTutorial.FontSize = 14;
                }

                tituloEntry.Completed += (s, e) =>
                {
                    tituloEntry.Unfocus();
                    calleEntry.Focus();
                };

                calleEntry.Completed += (s, e) =>
                {
                    calleEntry.Unfocus();
                    postalEntry.Focus();
                };

                postalEntry.Completed += (s, e) =>
                {
                    postalEntry.Unfocus();
                    coloniaEntry.Focus();
                };

                coloniaEntry.Completed += (s, e) =>
                {
                    coloniaEntry.Unfocus();
                    municipioEntry.Focus();
                };
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
        private void goDirecciones(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Ubicaciones(usuario, tipoMusico));
        }
        async Task getCurrentLocation()
        {
            double latLocation = 0;
            double longLocation = 0;
            GeolocationRequest request;
            Location location = null;

            request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            location = await Geolocation.GetLocationAsync(request, cts.Token);

            if (!string.IsNullOrEmpty(selected.id_ubicacion))
            {
                latLocation = selected.latitude;
                longLocation = selected.longitude;
            }
            else
            {
                latLocation = location.Latitude;
                longLocation = location.Longitude;
            }
            try
            {
                if (location != null)
                {
                    MapView.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Position(latLocation,
                            longLocation),
                        Distance.FromMiles(0.3)));

                    pin = new Pin
                    {
                        Label = tituloEntry.Text,
                        Address = calleEntry.Text + " " + coloniaEntry.Text,
                        Type = PinType.Place,
                        Position = new Position(latLocation, longLocation)
                    };

                    pin.MarkerClicked += (s, args) =>
                    {
                        MapView.Pins.Remove(pin);
                        calleEntry.Text = "";
                        postalEntry.Text = "";
                        coloniaEntry.Text = "";
                        municipioEntry.Text = "";
                        pkEstado.SelectedItem = "";
                        paisEntry.Text = "";
                    };

                    if (currentIsClicked)
                    {
                        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                        var placemark = placemarks?.FirstOrDefault();
                        
                        if (placemark != null)
                        {
                            if (placemark.CountryName.Equals("México") || placemark.CountryName.Equals("Mexico"))
                            {
                                calleEntry.Text = placemark.Thoroughfare + " " + placemark.FeatureName;
                                postalEntry.Text = placemark.PostalCode;
                                coloniaEntry.Text = placemark.SubLocality;
                                municipioEntry.Text = placemark.Locality;
                                pkEstado.SelectedItem = GetEstado(placemark.AdminArea);
                                paisEntry.Text = placemark.CountryName;
                                principalScrollview.ScrollToAsync(0, 0, true);

                                longMusico = location.Longitude;
                                latMusico = location.Latitude;
                            }
                            else
                            {
                                Application.Current.MainPage = new NavigationPage(new DetalleUbicacion(usuario, tipoMusico, selected));
                                Application.Current.MainPage.DisplayAlert("Ubicación no permitida", "Disponible sólo en la República Mexicana", "Ok");
                            }
                        }
                    }

                    if (longMusico!=0  && latMusico!=0)
                    {
                        MapView.Pins.Add(pin);
                        pinAdded = true;
                    }

                    longMusico = longLocation;
                    latMusico = latLocation;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine(fneEx);
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine(pEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                    if (placemark.CountryName.Equals("México") || placemark.CountryName.Equals("Mexico"))
                    {
                        pin = new Pin
                        {
                            Label = tituloEntry.Text,
                            Address = calleEntry.Text + " " + coloniaEntry.Text,
                            Type = PinType.Place,
                            Position = new Position(e.Position.Latitude, e.Position.Longitude)
                        };
                        MapView.Pins.Add(pin);
                        pinAdded = true;

                        pin.MarkerClicked += (s, args) =>
                        {
                            MapView.Pins.Remove(pin);
                            calleEntry.Text = "";
                            postalEntry.Text = "";
                            coloniaEntry.Text = "";
                            municipioEntry.Text = "";
                            pkEstado.SelectedItem = "";
                            paisEntry.Text = "";
                        };

                        calleEntry.Text = placemark.Thoroughfare + " " + placemark.FeatureName;
                        postalEntry.Text = placemark.PostalCode;
                        coloniaEntry.Text = placemark.SubLocality;
                        municipioEntry.Text = placemark.Locality;
                        pkEstado.SelectedItem = placemark.AdminArea;
                        string deleg = placemark.SubThoroughfare;
                        paisEntry.Text = placemark.CountryName;

                        longMusico = e.Position.Longitude;
                        latMusico = e.Position.Latitude;
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new DetalleUbicacion(usuario, tipoMusico, selected));
                        Application.Current.MainPage.DisplayAlert("Ubicación no permitida", "Disponible sólo en la República Mexicana", "Ok");
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
        }
        async void actualizaMapa(object sender, EventArgs e)
        {
            try
            {
                var address = calleEntry.Text + " " + postalEntry.Text + " " + coloniaEntry.Text + " " + municipioEntry.Text + " " + pkEstado.SelectedItem + " " + paisEntry.Text;
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
                        Label = tituloEntry.Text,
                        Address = calleEntry.Text + " " + coloniaEntry.Text,
                        Type = PinType.Place,
                        Position = new Position(location.Latitude, location.Longitude)
                    };
                    MapView.Pins.Add(pin);
                    pinAdded = true;

                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                    var placemark = placemarks?.FirstOrDefault();
                    string pais = placemark.CountryName;
                    if (placemark != null)
                    {
                        if (placemark.CountryName.Equals("México") || placemark.CountryName.Equals("Mexico"))
                        {
                            calleEntry.Text = placemark.Thoroughfare + " " + placemark.FeatureName;
                            postalEntry.Text = placemark.PostalCode;
                            coloniaEntry.Text = placemark.SubLocality;
                            municipioEntry.Text = placemark.Locality;
                            pkEstado.SelectedItem = GetEstado(placemark.AdminArea);
                            paisEntry.Text = placemark.CountryName;
                            principalScrollview.ScrollToAsync(0, 0, true);

                            longMusico = location.Longitude;
                            latMusico = location.Latitude;
                        }
                        else
                        {
                            Application.Current.MainPage = new NavigationPage(new DetalleUbicacion(usuario, tipoMusico, selected));
                            Application.Current.MainPage.DisplayAlert("Ubicación no permitida", "Disponible sólo en la República Mexicana", "Ok");
                        }
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
        }
        void guardaUbicacion(object sender, EventArgs e)
        {
            guardaBtn.IsEnabled = false;
            string idUbicacion;
            if (!string.IsNullOrEmpty(selected.id_ubicacion))
            {
                idUbicacion = selected.id_ubicacion;
            }
            else
            {
                string dtTimeNow = DateTime.Now.ToString("ddMMyyyyHHmmss");
                idUbicacion = Base64Encode(usuario) + dtTimeNow;
            }

            if (pinAdded)
            {
                if (!string.IsNullOrEmpty(distanciaEntry.Text))
                {
                    HttpClient client = new HttpClient();
                    string urlRequest = globalValues.webSite
                        + "ubicaciones_musico.php"
                        + "?tpBusqueda=U"
                        + "&idUbicacion=" + idUbicacion
                        + "&idUsuario=" + usuario
                        + "&titUbicacion=" + tituloEntry.Text
                        + "&calleNo=" + calleEntry.Text
                        + "&cdPostal=" + postalEntry.Text
                        + "&Colonia=" + coloniaEntry.Text
                        + "&Municipio=" + municipioEntry.Text
                        + "&Estado=" + pkEstado.SelectedItem
                        + "&Pais=" + paisEntry.Text
                        + "&Latitude=" + pin.Position.Latitude
                        + "&Longitude=" + pin.Position.Longitude
                        + "&Distancia=" + distanciaEntry.Text;

                    string responseRegistro = client.GetStringAsync(urlRequest).Result;

                    JObject regResponse = JObject.Parse(responseRegistro);

                    string result = regResponse["success"].ToString();

                    if (result.Equals("1"))
                    {
                        Application.Current.MainPage = new NavigationPage(new Ubicaciones(usuario, tipoMusico));
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                        guardaBtn.IsEnabled = true;
                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Defina una distancia válida para ubicación de clientes", "Ok");
                    distanciaEntry.Focus();
                    guardaBtn.IsEnabled = true;
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Agregue una ubicación válida", "Ok");
                guardaBtn.IsEnabled = true;
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        void tituloChanged(object sender, EventArgs e)
        {
            ubicacionLbl.Text = tituloEntry.Text;
        }
        private async void eliminaUbicacion(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Eliminar Ubicación", "¿Desea eliminar " + selected.titulo + "?", "Yes", "No");
            if (answer)
            {
                string urlEliminaUbicacion = globalValues.webSite
                + "ubicaciones_musico.php"
                + "?idUsuario=" + usuario
                + "&tpBusqueda=D"
                + "&idUbicacion=" + selected.id_ubicacion;

                string eliminaUbicacion = client.GetStringAsync(urlEliminaUbicacion).Result;

                JObject regResponse = JObject.Parse(eliminaUbicacion);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    Application.Current.MainPage = new NavigationPage(new Ubicaciones(usuario, tipoMusico));
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                }
            }
        }
        private void goCurrent(object sender, EventArgs e)
        {
            currentIsClicked = true;
            getCurrentLocation();
        }
        private string GetEstado(string estadoLbl)
        {
            string estadoCorr=estadoLbl;
            switch (estadoLbl)
            {
                case "AGS":
                    estadoCorr="Aguascalientes";
                    break;
                case "BC":
                    estadoCorr= "Baja California";
                    break;
                case "BCS":
                    estadoCorr= "Baja California Sur";
                    break;
                case "CAMP":
                    estadoCorr= "Campeche";
                    break;
                case "CHIH":
                    estadoCorr= "Chihuahua";
                    break;
                case "COAH":
                    estadoCorr= "Coahuila de Zaragoza";
                    break;
                case "COL":
                    estadoCorr= "Colima";
                    break;
                case "CHIS":
                    estadoCorr= "Chiapas";
                    break;
                case "CDMX":
                    estadoCorr= "Ciudad de México";
                    break;
                case "DGO":
                    estadoCorr="Durango";
                    break;
                case "GRO":
                    estadoCorr="Guerrero";
                    break;
                case "GTO":
                    estadoCorr="Guanajuato";
                    break;
                case "HGO":
                    estadoCorr="Hidalgo";
                    break;
                case "JAL":
                    estadoCorr="Jalisco";
                    break;
                case "Edomex":
                    estadoCorr= "Estado de México";
                    break;
                case "MICH":
                    estadoCorr= "Michoacán";
                    break;
                case "MOR":
                    estadoCorr= "Morelos";
                    break;
                case "NL":
                    estadoCorr= "Nuevo León";
                    break;
                case "NAY":
                    estadoCorr="Nayarit";
                    break;
                case "OAX":
                    estadoCorr= "Oaxaca";
                    break;
                case "PUE":
                    estadoCorr="Puebla";
                    break;
                case "QROO":
                    estadoCorr="Quintana Roo";
                    break;
                case "QRO":
                    estadoCorr= "Querétaro";
                    break;
                case "SIN":
                    estadoCorr="Sinaloa";
                    break;
                case "S.L.P.":
                    estadoCorr= "San Luis Potosí";
                    break;
                case "SON":
                    estadoCorr="Sonora";
                    break;
                case "TAB":
                    estadoCorr="Tabasco";
                    break;
                case "TLAX":
                    estadoCorr="Tlaxcala";
                    break;
                case "TAMPS":
                    estadoCorr="Tamaulipas";
                    break;
                case "VER":
                    estadoCorr="Veracruz";
                    break;
                case "YUC":
                    estadoCorr="Yucatán";
                    break;
                case "ZAC":
                    estadoCorr="Zacatecas";
                    break;
            }
            return estadoCorr;
        }
        private void verDemoUbicacion(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new DemoUbicaciones(usuario, tipoMusico, selected));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}