using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DemoVideos : ContentPage
	{
		bool conexion;
		double resolution = 0;
		string Usuario;
		string tipoMusico;
		public DemoVideos (string idUsuario, string tpMusico)
		{
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            Usuario = idUsuario;
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

                string vidH;
                string vidW;

                videoTutorial.HeightRequest = height;
                videoTutorial.WidthRequest = width;

                if (resolution > 2000000)
                {
                    vidH = (height * 0.275).ToString();
                    vidW = (width * 0.33).ToString();
                    regresarDetalleLb.FontSize = 18;
                }
                else
                {
                    vidH = (height * 0.5).ToString();
                    vidW = (width * 0.55).ToString();
                    regresarDetalleLb.FontSize = 14;
                }

                videoTutorial.Source = new HtmlWebViewSource
                {
                    Html = "<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>" +
                    "<html><body><div style=\"padding: 0%; align-items: center; display: flex; justify-content: center;\">" +
                    "<iframe width = \"" + vidW + "\" height = \"" + vidH + "\" src = \"" +
                    "https://www.youtube.com/embed/" + "NG-Vwckg-f0" +
                    "\" title = \"YouTube video player\" frameborder = \"0\" " +
                    "allow = \"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" " +
                    "allowfullscreen=\"allowfullscreen\" ></ iframe ></div></body></html>"
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
        private void regresarDetalle(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new DatosMusico(Usuario, tipoMusico));
        }
        protected override bool OnBackButtonPressed() => true;
	}
}