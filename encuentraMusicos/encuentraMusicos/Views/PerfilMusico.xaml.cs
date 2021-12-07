using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using Newtonsoft.Json.Linq;
using Plugin.FacebookClient;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class PerfilMusico : ContentPage
    {
        double resolution;
        bool conexion;
        string tipoMusico, isactive;
        bool isAdmin = false;
        bool isSingle=false, isBand=false;
        string usuario;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public PerfilMusico(string idUsuario, string tpMusico)
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
                    + "tipo_musico.php"
                    + "?Usuario=" + usuario
                    + "&tipoMov=G";

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);
                tipoMusico = regResponse["tipoMusico"]["tipo_musico"].ToString();

                string urlIsAdmin = globalValues.webSite
                        + "check_admin.php"
                        + "?Usuario=" + idUsuario;

                string responseIsAdmin = client.GetStringAsync(urlIsAdmin).Result;

                JObject regIsAdmin = JObject.Parse(responseIsAdmin);

                string strIsAdmin = regIsAdmin["isAdmin"].ToString();

                if (strIsAdmin.Equals("Y"))
                {
                    isAdmin = true;
                    btnBack.IsVisible = true;
                }
                else
                {
                    isAdmin = false;
                    btnBack.IsVisible = false;
                }

                    if (resolution > 2000000)
                {
                    lbTituloPerfilMusico.FontSize = 22;
                    lbYoMusico.FontSize = 18;
                    lbGenerosMusicales.FontSize = 18;
                    lbTarifa.FontSize = 18;
                    lbContacto.FontSize = 18;
                    lbUbicaciones.FontSize = 18;
                    lbOpiniones.FontSize = 18;
                    lbGuiaOpiniones.FontSize = 18;
                    lbInactivar.FontSize = 18;
                    lbCerrarSesion.FontSize = 18;
                    lbBusqueda.FontSize = 14;
                    lbSolista.FontSize = 16;
                    lbBanda.FontSize = 16;
                }
                else
                {
                    lbTituloPerfilMusico.FontSize = 14;
                    lbYoMusico.FontSize = 12;
                    lbGenerosMusicales.FontSize = 12;
                    lbTarifa.FontSize = 12;
                    lbContacto.FontSize = 12;
                    lbUbicaciones.FontSize = 12;
                    lbOpiniones.FontSize = 12;
                    lbGuiaOpiniones.FontSize = 12;
                    lbInactivar.FontSize = 12;
                    lbCerrarSesion.FontSize = 12;
                    lbBusqueda.FontSize = 10;
                    lbSolista.FontSize = 10;
                    lbBanda.FontSize = 10;
                }

                if (tipoMusico.Equals("S"))
                {
                    isSingle = true;
                    isBand = false;
                    graySingle.IsVisible = false;
                    grayBand.IsVisible = true;
                    lbYoMusico.Text = "¿Quien Soy?";
                    lbGenerosMusicales.Text = "¿Qué Toco?";
                    lbTarifa.Text = "¿Cuánto Cobro?";
                    lbContacto.Text = "¿Cómo Me Contactan?";
                    lbUbicaciones.Text = "Indica tu ubicación de inicio de búsqueda";
                    lbOpiniones.Text = "¿Qué Dicen Mis Clientes?";
                    lbGuiaOpiniones.Text = "¿Como pedir calificación?";
                    singleButton.IsVisible = true;
                    bandButton.IsVisible = false;
                }
                else
                {
                    isBand = true;
                    isSingle = false;
                    graySingle.IsVisible = true;
                    grayBand.IsVisible = false;
                    lbYoMusico.Text = "¿Quienes Somos?";
                    lbGenerosMusicales.Text = "¿Qué Tocamos?";
                    lbTarifa.Text = "¿Cuánto Cobramos?";
                    lbContacto.Text = "¿Cómo Nos Contactan?";
                    lbUbicaciones.Text = "Indica tu ubicación de inicio de búsqueda";
                    lbOpiniones.Text = "¿Qué Dicen Nuestros Clientes?";
                    lbGuiaOpiniones.Text = "¿Como pedir calificación?";
                    singleButton.IsVisible = false;
                    bandButton.IsVisible = true;
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
        private void goQuienesSomos(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new DatosMusico(usuario, tipoMusico));
        }
        private void goGenerosMusicales(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GenerosMusicalesVW(usuario, tipoMusico));
        }
        private void goTarifas(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Tarifas(usuario, tipoMusico));
        }
        private void goContacto(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ContactoMusico(usuario, tipoMusico));
        }
        private void goUbicaciones(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Ubicaciones(usuario, tipoMusico));
        }
        private void clickSingle(object sender, EventArgs e)
        {
            if (!isSingle)
            {
                string urlRequest = globalValues.webSite
                    + "tipo_musico.php"
                    + "?Usuario=" + usuario
                    + "&tipoMov=S";

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);
                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                    var db = new SQLiteConnection(databasePath);
                    graySingle.IsVisible = false;
                    grayBand.IsVisible = true;
                    isBand = false;
                    lbYoMusico.Text = "¿Quien Soy?";
                    lbGenerosMusicales.Text = "¿Qué Toco?";
                    lbTarifa.Text = "¿Cuánto Cobro?";
                    lbContacto.Text = "¿Cómo Me Contactan?";
                    lbUbicaciones.Text = "¿Dónde Toco?";
                    lbOpiniones.Text = "¿Qué Dicen Mis Clientes?";
                    singleButton.IsVisible = true;
                    bandButton.IsVisible = false;
                    db.Query<T_Registro>("Update USUARIOS set tpMusico ='S' where Id = '" + usuario + "'");
                    tipoMusico = "S";
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                }
            }
        }
        private void clickBand(object sender, EventArgs e)
        {
            if (!isBand)
            {
                string urlRequest = globalValues.webSite
                    + "tipo_musico.php"
                    + "?Usuario=" + usuario
                    + "&tipoMov=B";

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);
                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                    var db = new SQLiteConnection(databasePath);
                    graySingle.IsVisible = true;
                    grayBand.IsVisible = false;
                    isSingle = false;
                    lbYoMusico.Text = "¿Quienes Somos?";
                    lbGenerosMusicales.Text = "¿Qué Tocamos?";
                    lbTarifa.Text = "¿Cuánto Cobramos?";
                    lbContacto.Text = "¿Cómo Nos Contactan?";
                    lbUbicaciones.Text = "¿Dónde Tocamos?";
                    lbOpiniones.Text = "¿Qué Dicen Nuestros Clientes?";
                    singleButton.IsVisible = false;
                    bandButton.IsVisible = true;
                    db.Query<T_Registro>("Update USUARIOS set tpMusico ='B' where Id = '" + usuario + "'");
                    tipoMusico = "B";
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                }
            }
        }
        private async void inactivar(object sender, EventArgs e)
        {
            if (isactive.Equals("Y"))
            {
                bool answer = await DisplayAlert("Dejará de aparecer en búsquedas de Músicos", "¿Está seguro de inactivar su perfil?", "Yes", "No");
                if (answer)
                {
                    string urlRequest = globalValues.webSite
                        + "admin_musicos.php"
                        + "?tpBusqueda=U"
                        + "&idUsuario=" + usuario
                        + "&isActive=N";

                    string responseUpdate = client.GetStringAsync(urlRequest).Result;

                    JObject regResponse = JObject.Parse(responseUpdate);

                    string result = regResponse["success"].ToString();

                    if (result.Equals("1"))
                    {
                        Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario,tipoMusico));
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                        Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
                    }
                }
            }
            else
            {
                bool answer = await DisplayAlert("Usted volverá a aparecer en búsquedas de Músicos", "¿Está seguro de activar su perfil?", "Yes", "No");
                if (answer)
                {
                    string urlRequest = globalValues.webSite
                        + "admin_musicos.php"
                        + "?tpBusqueda=U"
                        + "&idUsuario=" + usuario
                        + "&isActive=Y";

                    string responseUpdate = client.GetStringAsync(urlRequest).Result;

                    JObject regResponse = JObject.Parse(responseUpdate);

                    string result = regResponse["success"].ToString();

                    if (result.Equals("1"))
                    {
                        Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                        Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
                    }
                }
            }
        }
        private async void logOut(object sender, EventArgs e)
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);
            bool answer = await DisplayAlert("Cerrar Sesión", "¿Está seguro de cerrar sesión?", "Yes", "No");
            if (answer)
            {
                try
                {
                    db.DeleteAll<T_Registro>();
                    db.DeleteAll<T_MisGeneros>();
                    db.DeleteAll<T_MusicoContactado>();
                    db.DeleteAll<T_Notificaciones>();
                    db.DeleteAll<T_Resultados>();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                DependencyService.Get<IClearCookies>().ClearAllCookies();
                Application.Current.MainPage = new NavigationPage(new SelectUsuario());
            }
            CrossFacebookClient.Current.Logout();
        }
        private void goGuia(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GuiaCalificacion(usuario, tipoMusico));
        }
        private void goBusqueda(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalBusqueda(usuario,tipoMusico));
        }
        private void goOpiniones(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Opiniones(usuario, tipoMusico));
        }
        private void goBack(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Administracion(isAdmin, usuario));
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (conexion)
            {
                string urlRequest = globalValues.webSite
                + "perfil_completado.php"
                + "?Usuario=" + usuario;

                string responseCompletado = client.GetStringAsync(urlRequest).Result;

                double avance = 0;

                JObject porcResponse = JObject.Parse(responseCompletado);

                if (porcResponse["perfil"]["datos_musico"].ToString().Equals("Y"))
                {
                    avance = avance + 0.2;
                    grayDatosMusico.IsVisible = false;
                }
                else
                {
                    grayDatosMusico.IsVisible = true;
                }

                if (porcResponse["perfil"]["generos_musicales"].ToString().Equals("Y"))
                {
                    avance = avance + 0.2;
                    grayGeneros.IsVisible = false;
                }
                else
                {
                    grayGeneros.IsVisible = true;
                }

                if (porcResponse["perfil"]["tarifas"].ToString().Equals("Y"))
                {
                    avance = avance + 0.2;
                    grayTarifas.IsVisible = false;
                }
                else
                {
                    grayTarifas.IsVisible = true;
                }

                if (porcResponse["perfil"]["contacto"].ToString().Equals("Y"))
                {
                    avance = avance + 0.2;
                    grayContacto.IsVisible = false;
                }
                else
                {
                    grayContacto.IsVisible = true;
                }

                if (porcResponse["perfil"]["ubicaciones"].ToString().Equals("Y"))
                {
                    avance = avance + 0.2;
                    grayUbicaciones.IsVisible = false;
                }
                else
                {
                    grayUbicaciones.IsVisible = true;
                }

                if (resolution > 2000000)
                {
                    porcCompletado.FontSize = 12;
                }
                else
                {
                    porcCompletado.FontSize = 10;
                }

                isactive = porcResponse["perfil"]["is_active"].ToString();

                if (isactive.Equals("Y"))
                {
                    isactiveImage.Source = "nomusic.png";
                    lbInactivar.Text = "Inactivar Perfil";
                }
                else
                {
                    isactiveImage.Source = "musicallow.png";
                    lbInactivar.Text = "Activar Perfil";
                }

                double porcentajeAvance = avance * 100;

                await pgBar.ProgressTo(avance, 500, Easing.Linear);
                porcCompletado.Text = "Perfil Completado: " + porcentajeAvance + "%";

                if (porcentajeAvance < 100)
                {
                    lbIncompleto.IsVisible = true;
                    lbCompleto.IsVisible = false;
                }
                else
                {
                    lbIncompleto.IsVisible = false;
                    lbCompleto.IsVisible = true;
                }
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}