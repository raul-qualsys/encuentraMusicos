using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.ViewModels;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginAP : ContentPage
	{
		double resolution;
		bool conexion;
        bool isAdmin = false;
        GlobalValues globalValues = new GlobalValues();

        public LoginAP ()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent ();

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

				appleLogin.Source = "https://appleid.apple.com/auth/authorize?client_id=com.libher.encuentraMusicos&response_type=code%20id_token&redirect_uri=https%3A%2F%2Fencuentramusicos.libher.info&response_mode=fragment";
                appleLogin.Navigated += loginAPWebViewNavigated;

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
		private void OnSignIn(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new PerfilMusico("usuarioapple","S"));
		}
		private void cancelLoginAP(object sender, EventArgs e)
		{
			Application.Current.MainPage = new NavigationPage(new LoginMusico());
		}
        private void loginAPWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            string idApple = "";
         
            var AccessURL = e.Url;

            string urlresp = AccessURL;

            if (AccessURL.Contains("&id_token="))
            {
				int tokenPosition = AccessURL.IndexOf("&id_token=");

				string tokenApple = AccessURL.Substring(tokenPosition);

				tokenApple = tokenApple.Substring(10);

				int point1 = tokenApple.IndexOf(".")+1;

				string dataTokenp1 = tokenApple.Substring(point1);

				string dataToken = dataTokenp1.Substring(0, dataTokenp1.IndexOf("."))+"==";

				byte[] newBytes = Convert.FromBase64String(dataToken);
				string decodedString = Encoding.UTF8.GetString(newBytes);

				int posSub = decodedString.IndexOf("\"sub\":")+7;

				string subContent = decodedString.Substring(posSub);

				int posSubFinal = subContent.IndexOf("\",");

				idApple = subContent.Substring(0, posSubFinal);

                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MySQLite.db3");
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<T_Registro>();

                var newUsuario = new T_Registro();
                newUsuario.Id = idApple;
                newUsuario.userb64 = Base64Encode(idApple);

                string urlRequest = globalValues.webSite
                    + "reg_usuario.php"
                    + "?fbUser=" + idApple
                    + "&b64User=" + newUsuario.userb64;

                HttpClient client = new HttpClient();

                string responseRegistro = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseRegistro);

                string result = regResponse["success"].ToString();


                string urlExistUsuario = globalValues.webSite
                    + "datos_pers_musico.php"
                    + "?tpBusqueda=S"
                    + "&idUsuario=" + idApple;

                string responseExiste = client.GetStringAsync(urlExistUsuario).Result;

                JObject regExiste = JObject.Parse(responseExiste);

                string tpMusico = regExiste["datosPersonalesM"]["tipo_musico"].ToString();

                if (string.IsNullOrEmpty(tpMusico))
                {
                    tpMusico = "S";
                }


                if (result.Equals("1"))
                {
                    CreateFolder(newUsuario.userb64);
                    newUsuario.isSynchronized = "Y";
                }
                else
                {
                    newUsuario.isSynchronized = "N";
                }

                newUsuario.isActive = "Y";
                newUsuario.tpMusico = tpMusico;

                db.Insert(newUsuario);

                //Inicia validación de administrador
                string urlIsAdmin = globalValues.webSite
                    + "check_admin.php"
                    + "?Usuario=" + idApple;

                string responseIsAdmin = client.GetStringAsync(urlIsAdmin).Result;

                JObject regIsAdmin = JObject.Parse(responseIsAdmin);

                string strIsAdmin = regIsAdmin["isAdmin"].ToString();

                if (strIsAdmin.Equals("Y"))
                {
                    isAdmin = true;
                }
                else
                {
                    isAdmin = false;
                }

                //Termina validación de administrador

                if (isAdmin)
                {
                    Application.Current.MainPage = new NavigationPage(new Administracion(isAdmin, idApple));
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new PerfilMusico(idApple, tpMusico));
                }
            }
            else if (AccessURL.Contains("user_cancelled_authorize"))
            {
                Application.Current.MainPage = new NavigationPage(new LoginMusico());
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public void CreateFolder(string folder)
        {
            bool IsCreated;
            try
            {
                WebRequest request = WebRequest.Create(globalValues.hostFTP + folder);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(globalValues.userFTP, globalValues.passwordFTP);
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                }
            }
            catch (Exception ex)
            {
                IsCreated = false;
                Console.WriteLine("IsCreated: " + IsCreated + " ex: " + ex.ToString());
            }
        }
        protected override bool OnBackButtonPressed() => true;
	}
}