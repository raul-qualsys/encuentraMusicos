using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.Models;
using encuentraMusicos.ViewModels;
using Newtonsoft.Json.Linq;
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
    public partial class DetalleGrupo : ContentPage
    {
        double resolution;
        bool conexion;
        string usuario;
        string tipoUsuario;
        string busquedaOrig;
        double longitudeSearch;
        double latitudeSearch;
        double heightPrincipalOrig, heightVideoOrig;
        string precioSearch;
        string isYTVideo, isFBVideo, urlVideo;
        bool isSeenVideo = false;
        GruposMusicales grupoSeleccionado = new GruposMusicales();
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public DetalleGrupo(string busqueda, double longSearch, double latSearch, string precio, GruposMusicales selectedGrupo, string idUsuario, string tpUsuario)
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

                heightPrincipalOrig = gridPrincipal.HeightRequest;

                ValoracionesViewModel vm = new ValoracionesViewModel(selectedGrupo.id_musico);
                BindingContext = vm;

                collectionMusicos.HeightRequest = 70 * vm.numValoraciones;

                if (selectedGrupo.valoracion == 0)
                {
                    valoracionTotal.IsVisible = false;
                }
                else
                {
                    valoracionTotal.IsVisible = true;
                }

                if (vm.numValoraciones > 0)
                {
                    valoracionesClientes.IsVisible = true;
                }
                else
                {
                    valoracionesClientes.IsVisible = false;
                }

                longitudeSearch = longSearch;
                latitudeSearch = latSearch;
                busquedaOrig = busqueda;
                usuario = idUsuario;
                tipoUsuario = tpUsuario;
                precioSearch = precio;

                grupoSeleccionado = selectedGrupo;

                if (resolution > 2000000)
                {
                    btnBack.WidthRequest = 30;
                    nombreGrupo.FontSize = 20;
                    imageGrupo.HeightRequest = 200;
                    descripcionGrupo.FontSize = 16;
                    actividadGrupo.FontSize = 16;
                    lbPrecioTxt.FontSize = 18;
                    precioGrupo.FontSize = 18;
                    descrPrecio.FontSize = 14;
                    lbValoracion.FontSize = 18;
                    lbOpiniones.FontSize = 18;
                    star1.HeightRequest = 20;
                    star2.HeightRequest = 20;
                    star3.HeightRequest = 20;
                    star4.HeightRequest = 20;
                    star5.HeightRequest = 20;
                    yt_video.HeightRequest = 220;
                    yt_video.WidthRequest = 150;
                    contactoBtn.FontSize = 18;
                }
                else
                {
                    btnBack.WidthRequest = 25;
                    nombreGrupo.FontSize = 14;
                    imageGrupo.HeightRequest = 150;
                    descripcionGrupo.FontSize = 10;
                    actividadGrupo.FontSize = 10;
                    lbPrecioTxt.FontSize = 14;
                    precioGrupo.FontSize = 14;
                    descrPrecio.FontSize = 10;
                    lbValoracion.FontSize = 12;
                    lbOpiniones.FontSize = 12;
                    star1.HeightRequest = 10;
                    star2.HeightRequest = 10;
                    star3.HeightRequest = 10;
                    star4.HeightRequest = 10;
                    star5.HeightRequest = 10;
                    yt_video.HeightRequest = 165;
                    yt_video.WidthRequest = 160;
                    contactoBtn.FontSize = 14;
                }

                nombreGrupo.Text = selectedGrupo.nombre_musico;
                imageGrupo.Source = selectedGrupo.nombre_media;
                descripcionGrupo.Text = selectedGrupo.descr_musico;
                actividadGrupo.Text = selectedGrupo.actividad_musico;
                star1.IsVisible = selectedGrupo.star1;
                star2.IsVisible = selectedGrupo.star2;
                star3.IsVisible = selectedGrupo.star3;
                star4.IsVisible = selectedGrupo.star4;
                star5.IsVisible = selectedGrupo.star5;

                precioGrupo.Text = selectedGrupo.precio;
                descrPrecio.Text = selectedGrupo.descrprecio;

                frVideo.IsVisible = false;

                isYTVideo = grupoSeleccionado.isYoutube;
                isFBVideo = grupoSeleccionado.isFacebook;

                urlVideo = grupoSeleccionado.urlYoutube;

                if (grupoSeleccionado.isYoutube.Equals("Y"))
                {
                    showYTVideo();
                }

                if (grupoSeleccionado.isFacebook.Equals("Y"))
                {
                    showFBVideo();
                }

                videoPlayer.IsVisible = false;
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
        private void goResultados(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ResultadosBusqueda(busquedaOrig, longitudeSearch, latitudeSearch, precioSearch, usuario, tipoUsuario));
        }
        private void showFBVideo()
        {
            string vidH;
            string vidW;
            if (resolution > 2000000)
            {
                vidH = "226";
                vidW = "376";
            }
            else
            {
                vidH = "146";
                vidW = "251";
            }

            yt_video.Source = new HtmlWebViewSource
            {
                Html = "<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>"
                + "<html><body><div style=\"padding: 0%; align-items: center; display: flex; justify-content: center;\">"
                + "<iframe src=\"https://www.facebook.com/plugins/video.php?"
                + "height=" + vidH
                + "&href="
                + grupoSeleccionado.urlYoutube
                + "&show_text=false"
                + "&width=" + vidW
                + "&t=0\" "
                + "width=\"" + vidW + "\" "
                + "height=\"" + vidH + "\" "
                + "style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\" allowFullScreen=\"true\">"
                + "</iframe>"
                + "</div></body></html>"
            };

            frVideo.IsVisible = true;

            if (Device.RuntimePlatform == Device.Android)
            {
                yt_video.IsEnabled = false;
                verVideoBtn.IsVisible = true;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                yt_video.IsEnabled = true;
                verVideoBtn.IsVisible = false;
            }
        }
        private void showYTVideo()
        {
            string vidH;
            string vidW;
            if (resolution > 2000000)
            {
                vidH = "205";
                vidW = "350";
            }
            else
            {
                vidH = "150";
                vidW = "255";
            }

            yt_video.Source = new HtmlWebViewSource
            {
                Html = "<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>" +
                "<html><body><div style=\"padding: 0%; align-items: center; display: flex; justify-content: center;\">" +
                "<iframe width = \"" + vidW + "\" height = \"" + vidH + "\" src = \"" +
                grupoSeleccionado.urlYoutube +
                "\" title = \"YouTube video player\" frameborder = \"0\" " +
                "allow = \"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" " +
                "allowfullscreen ></ iframe ></div></body></html>"
            };

            frVideo.IsVisible = true;

            if (Device.RuntimePlatform == Device.Android)
            {
                yt_video.IsEnabled = false;
                verVideoBtn.IsVisible = true;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                yt_video.IsEnabled = true;
                verVideoBtn.IsVisible = false;
            }
        }
        private void goContacto(object sender, EventArgs e)
        {
            double diasValoracionMusico=5;
            string urlRequest = globalValues.webSite
            + "conf_notificaciones.php"
            + "?tpBusqueda=S";

            string responseSelect = client.GetStringAsync(urlRequest).Result;

            if (!responseSelect.Equals("[]"))
            {
                JObject regResponse = JObject.Parse(responseSelect);

                string diasValoracion = regResponse["confNotificaciones"]["dias_notif_valoracion"].ToString();

                diasValoracionMusico = Convert.ToDouble(diasValoracion);
            }

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);

            db.CreateTable<T_MusicoContactado>();

            T_MusicoContactado newContactado = new T_MusicoContactado();

            newContactado.id_usuario = grupoSeleccionado.id_musico;
            newContactado.id_contacto = DateTime.Now.AddDays(diasValoracionMusico).ToString("yyyyMMddHHmmss");
            newContactado.nombre_usuario = grupoSeleccionado.nombre_musico;
            newContactado.fecha_contacto = DateTime.Now.Date.AddDays(diasValoracionMusico).AddHours(10);
            newContactado.estatus = "I";  //Inicial
            
            try
            {
                db.Insert(newContactado);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Generic error: " + ex.Message);
            }

            Application.Current.MainPage = new NavigationPage(new Contacto(busquedaOrig, longitudeSearch, latitudeSearch, precioSearch, grupoSeleccionado, usuario, tipoUsuario));
        }
        private void verVideo(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "PreventPortrait");

            if (!isSeenVideo)
            {
                heightVideoOrig = videoPlayer.HeightRequest;
                isSeenVideo = true;
            }

            gridPrincipal.IsVisible = false;
            gridPrincipal.HeightRequest = 0;
            sinConexion.IsVisible = false;
            sinConexion.HeightRequest = 0;
            videoPlayer.IsVisible = true;
            videoPlayer.HeightRequest = heightVideoOrig;

            yt_videoFS.HeightRequest = Width;
            yt_videoFS.WidthRequest = Height;

            if (isYTVideo.Equals("Y"))
            {
                yt_videoFS.Source = new HtmlWebViewSource
                {
                    Html = "<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>" +
                    "<html><body><div style=\"padding: 0%; align-items: center; display: flex; justify-content: center;\">" +
                    "<iframe style=\"position: absolute; top: 0; left: 0; right: 0; width: 100%; height: 95%;\" src = \"" +
                    urlVideo +
                    "\" title = \"YouTube video player\" frameborder = \"0\" " +
                    "allow = \"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" " +
                    "allowfullscreen ></ iframe ></div></body></html>"
                };
            }
            else if (isFBVideo.Equals("Y"))
            {
                yt_videoFS.Source = new HtmlWebViewSource
                {
                    Html = "<meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1'/>"
                    + "<html><body style = \"padding: 0; margin: 0; width:100%;\">"
                    + "<div style = \"border: 1px solid black; background-color: black; padding:0%; margin:0%; position:absolute; top:0; left:0; right:0; width:100%; height:100%; display: flex; justify-content: center;\">"
                    + "<iframe src=\"https://www.facebook.com/plugins/video.php?"
                    + "href=" + urlVideo
                    + "&show_text=false"
                    + "&t=0\" "
                    + "width=\"85%\" "
                    + "height=\"100%\" "
                    + "style=\"padding:0%; margin:0%; border:none; overflow:hidden;\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\" allowFullScreen=\"true\">"
                    + "</iframe>"
                    + "</div>"
                    + "</body></html>"
                };
            }
        }
        private void cerrarVideo(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AllowPortrait");

            gridPrincipal.IsVisible = true;
            gridPrincipal.HeightRequest = heightPrincipalOrig;
            sinConexion.IsVisible = false;
            sinConexion.HeightRequest = 0;
            videoPlayer.IsVisible = false;
            videoPlayer.HeightRequest = 0;

            yt_videoFS.Source = new HtmlWebViewSource
            {
                Html = "<p></p"
            };
        }
        protected override bool OnBackButtonPressed() => true;
    }
}