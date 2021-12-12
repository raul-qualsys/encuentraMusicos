using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using encuentraMusicos.Classes;
using System.IO;
using SQLite;
using encuentraMusicos.DataBase.Tables;
using Newtonsoft.Json.Linq;
using System.Net;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Stormlion.ImageCropper;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatosMusico : ContentPage
    {
        bool conexion;
        double resolution;
        string urlVideo;
        string isFBVideo = "N";
        string isYTVideo = "N";
        string usuario;
        string tpMusico;
        bool isNewMedia = false;
        string filePath;
        string fileName;
        string dtTimeNow;
        string idMedia;
        string idUsuariob64;
        string path_media;
        bool isDeletedImage = false;
        double heightPrincipalOrig, heightVideoOrig;
        bool isSeenVideo = false;

        GlobalValues globalValues = new GlobalValues();
        public DatosMusico(string idUsuario, string tipoMusico)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            tpMusico = tipoMusico;

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            videoPlayer.IsVisible = false;

            resolution = width * height;

            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                conexion = true;
                sinConexion.IsVisible = false;
                sinConexion.HeightRequest = 0;
                gridPrincipal.IsVisible = true;

                heightPrincipalOrig = gridPrincipal.HeightRequest;

                videoFake.Navigating += getURLVideo;

                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                var db = new SQLiteConnection(databasePath);
                var resultado = db.Query<T_Registro>("SELECT * from USUARIOS where Id='" + usuario + "'");

                if (resultado.Count() > 0)
                {
                    foreach (var s in resultado)
                    {
                        idUsuariob64 = s.userb64;
                    }
                }

                /**/
                HttpClient client = new HttpClient();

                string urlRequest = globalValues.webSite
                    + "datos_pers_musico.php"
                    + "?tpBusqueda=S"
                    + "&idUsuario=" + idUsuario;

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);

                string idMediaBasic = regResponse["datosPersonalesM"]["nombre_media"].ToString();

                if (!string.IsNullOrEmpty(idMediaBasic))
                {
                    idMedia = idMediaBasic;
                }

                path_media = regResponse["datosPersonalesM"]["id_path"].ToString();
                if (string.IsNullOrEmpty(path_media))
                {
                    path_media = idUsuariob64;
                }

                string fotoMusico = regResponse["datosPersonalesM"]["nombre_media"].ToString();

                if (string.IsNullOrEmpty(fotoMusico))
                {
                    tomaFotoBtn.IsVisible = true;
                    borraImagenBtn.IsVisible = false;
                    fileName = "";
                    noImage.IsVisible = true;
                    image.IsVisible = false;
                }
                else
                {
                    fileName = fotoMusico;
                    image.Source = globalValues.webSite
                        + "Images/user_profile/"
                        + path_media + "/"
                        + fotoMusico;
                    tomaFotoBtn.IsVisible = false;
                    borraImagenBtn.IsVisible = true;
                    noImage.IsVisible = false;
                    image.IsVisible = true;
                }
                enNombre.Text = regResponse["datosPersonalesM"]["nombre"].ToString();
                enActividad.Text = regResponse["datosPersonalesM"]["actividad"].ToString();
                enDescripcion.Text = regResponse["datosPersonalesM"]["descripcion"].ToString();
                enURLYoutube.Text = regResponse["datosPersonalesM"]["urluser_video"].ToString();

                isFBVideo = regResponse["datosPersonalesM"]["fb_video"].ToString();
                isYTVideo = regResponse["datosPersonalesM"]["yt_video"].ToString();
                urlVideo = regResponse["datosPersonalesM"]["url_video"].ToString();
                /**/

                if (isFBVideo.Equals("N") && isYTVideo.Equals("N"))
                {
                    frVideo.IsVisible = false;
                }

                if (isFBVideo.Equals("Y"))
                {
                    showFBVideo();
                }

                if (isYTVideo.Equals("Y"))
                {
                    showYTVideo();
                }

                if (resolution > 2000000)
                {
                    lbPerfilMusico.FontSize = 20;
                    lbNombre.FontSize = 14;
                    enNombre.FontSize = 16;
                    lbActividad.FontSize = 14;
                    enActividad.FontSize = 16;
                    lbDescripcion.FontSize = 14;
                    lbDescripcion.WidthRequest = 90;
                    enDescripcion.FontSize = 16;
                    lbImagen.FontSize = 14;
                    lbYoutube.FontSize = 14;
                    enURLYoutube.FontSize = 16;
                    yt_video.HeightRequest = 220;
                    yt_video.WidthRequest = 150;
                    botonGuardar.FontSize = 20;
                }
                else
                {
                    lbPerfilMusico.FontSize = 16;
                    lbNombre.FontSize = 10;
                    enNombre.FontSize = 12;
                    lbActividad.FontSize = 10;
                    enActividad.FontSize = 12;
                    lbDescripcion.FontSize = 10;
                    lbDescripcion.WidthRequest = 80;
                    enDescripcion.FontSize = 12;
                    lbImagen.FontSize = 10;
                    lbYoutube.FontSize = 10;
                    enURLYoutube.FontSize = 12;
                    yt_video.HeightRequest = 165;
                    yt_video.WidthRequest = 160;
                    botonGuardar.FontSize = 12;
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
            Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tpMusico));
        }
        private void urlVideoChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Contains("facebook.com") || e.NewTextValue.Contains("fb.watch"))
            {
                isFBVideo = "Y";
                isYTVideo = "N";

                if (e.NewTextValue.Contains("fb.watch"))
                {
                    videoFake.Source = e.NewTextValue;
                }

                if (e.NewTextValue.Contains("facebook.com"))
                {
                    urlVideo = e.NewTextValue;
                    showFBVideo();
                }
            }

            if (e.NewTextValue.Contains("youtu.be") || e.NewTextValue.Contains("youtube.com"))
            {
                isFBVideo = "N";
                isYTVideo = "Y";

                string idVideoYT = e.NewTextValue.Replace("https://youtu.be/", "");
                idVideoYT = idVideoYT.Replace("https://www.youtube.com/watch?v=", "");
                urlVideo = idVideoYT;

                showYTVideo();
            }

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                frVideo.IsVisible = false;
                isFBVideo = "N";
                isYTVideo = "N";
                urlVideo = "";
            }
        }
        private void getURLVideo(object sender, WebNavigatingEventArgs e)
        {
            urlVideo = e.Url;

            showFBVideo();
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
                + urlVideo
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
            }else if (Device.RuntimePlatform == Device.iOS)
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
                "<iframe width = \""+ vidW +"\" height = \""+ vidH +"\" src = \"" +
                "https://www.youtube.com/embed/" + urlVideo +
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
        private void guardarDatos(object sender, EventArgs e)
        {
            bool uploaded = false;
            botonGuardar.IsEnabled = false;
            if (!string.IsNullOrEmpty(enNombre.Text))
            {
                HttpClient client = new HttpClient();

                string updMedia = "&idImagen=" + idMedia
                        + "&nmFile=" + fileName;

                string urlRequest = globalValues.webSite
                    + "datos_pers_musico.php"
                    + "?tpBusqueda=U"
                    + "&idUsuario=" + usuario
                    + "&Nombre=" + enNombre.Text
                    + "&Actividad=" + enActividad.Text
                    + "&Descripcion=" + enDescripcion.Text
                    + "&tipoMusico=" + tpMusico
                    + "&urlUserVideo=" + enURLYoutube.Text
                    + "&ytVideo=" + isYTVideo
                    + "&fbVideo=" + isFBVideo
                    + "&urlVideo=" + urlVideo
                    + updMedia;

                string responseRegistro = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseRegistro);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    if (isNewMedia)
                    {
                        uploaded=UploadFile(filePath, globalValues.hostFTP);
                        if (uploaded)
                        {
                            Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tpMusico));
                        }
                        else
                        {
                            botonGuardar.IsEnabled = true;
                            Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                        }
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tpMusico));
                    }
                }
                else
                {
                    botonGuardar.IsEnabled = true;
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                }
            }
            else
            {
                botonGuardar.IsEnabled = true;
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "El nombre no puede ir vacío", "Ok");
            }
        }
        public bool UploadFile(string file, string host)
        {
            bool isuploaded = true;
            string From = file;
            string To = host + "/" + path_media + "/" + fileName;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(globalValues.userFTP, globalValues.passwordFTP);
                    byte[] responseArray = client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, From);
                    Console.WriteLine("\nResponse Received. The contents of the file uploaded are:\n{0}",
                        System.Text.Encoding.ASCII.GetString(responseArray));
                }
            }
            catch (Exception e)
            {
                isuploaded = false;
                Console.WriteLine("Error: "+e.ToString());
            }
            return isuploaded;
        }
        async void tomaFoto(object sender, EventArgs e)
        {
            string file = "";
            isNewMedia = true;
            new ImageCropper()
            {
                Success = (imageFile) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        image.Source = ImageSource.FromFile(imageFile);
                    });
                    file = imageFile;
                    if (file == null)
                    {
                        return;
                    }
                    else
                    {
                        filePath = file;
                        fileName = Path.GetFileName(filePath);

                        dtTimeNow = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        idMedia = idUsuariob64 + dtTimeNow;
                        tomaFotoBtn.IsVisible = false;
                        borraImagenBtn.IsVisible = true;

                        noImage.IsVisible = false;
                        image.IsVisible = true;
                    }
                }
            }.Show(this);
        }
        async void deletePhoto(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Eliminar Foto", "¿Está seguro de eliminar la foto?", "Yes", "No");
            if (answer)
            {
                image.Source = null;
                tomaFotoBtn.IsVisible = true;
                borraImagenBtn.IsVisible = false;
                fileName = "";
                idMedia = "";
                isDeletedImage = true;
                noImage.IsVisible = true;
                image.IsVisible = false;
            }
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
                    "https://www.youtube.com/embed/" + urlVideo +
                    "\" title = \"YouTube video player\" frameborder = \"0\" " +
                    "allow = \"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" " +
                    "allowfullscreen ></ iframe ></div></body></html>"
                };
            }
            else if (isFBVideo.Equals("Y"))
            {
                yt_videoFS.Source = new HtmlWebViewSource
                {
                    Html="<meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1'/>"
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
            videoPlayer.HeightRequest=0;

            yt_videoFS.Source = new HtmlWebViewSource
            {
                Html="<p></p"
            };
        }
        private void verDemoVideos(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new DemoVideos(usuario, tpMusico));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}