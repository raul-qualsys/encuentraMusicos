using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;
using System.Net.Http;
using encuentraMusicos.Classes;
using Newtonsoft.Json.Linq;
using System.IO;
using SQLite;
using encuentraMusicos.DataBase.Tables;

namespace encuentraMusicos.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class popUpValoracion //: ContentPage
    {
        double resolution;
        double valoracion;
        string usuario;
        bool star1On = false;
        bool star2On = false;
        bool star3On = false;
        bool star4On = false;
        bool star5On = false;
        string musico;
        string valoracionId;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public popUpValoracion(string idUsuario, string idContacto, string id_musico, string nombreMusico)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            musico = id_musico;
            valoracionId = idContacto;

            star1.Source = "star1gray.png";
            star2.Source = "star1gray.png";
            star3.Source = "star1gray.png";
            star4.Source = "star1gray.png";
            star5.Source = "star1gray.png";

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;

            lbMusico.Text = "¿Tuviste un evento con "+nombreMusico+"?";

            string urlRequest = globalValues.webSite
                + "busqueda_valoracion.php"
                + "?idMusico="+id_musico;

            string responseSelect = client.GetStringAsync(urlRequest).Result;

            if (!responseSelect.Equals("[]"))
            {
                JObject regResponse = JObject.Parse(responseSelect);
                imageMusico.Source= globalValues.webSite + "Images/user_profile/" + regResponse["musico"]["id_path"].ToString() + "/" + regResponse["musico"]["nombre_media"].ToString();
            }

            if (resolution > 2000000)
            {
                lbOpinion.FontSize = 16;
                lbMusico.FontSize = 14;
                closeBtn.FontSize = 14;
                edMensaje.FontSize = 14;
                lbNombre.FontSize = 14;
                enNombre.FontSize = 14;
            }
            else
            {
                lbOpinion.FontSize = 12;
                lbMusico.FontSize = 10;
                closeBtn.FontSize = 10;
                edMensaje.FontSize = 10;
                lbNombre.FontSize = 12;
                enNombre.FontSize = 12;
            }
        }
        private async void eliminarValoracion(object sender, EventArgs e)
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);

            db.CreateTable<T_MusicoContactado>();

            db.Update(new T_MusicoContactado
            {
                id_usuario = musico,
                estatus = "V"
            });

            await PopupNavigation.PopAsync();
        }
        private void star1Clicked(object sender, EventArgs e)
        {
            if (!star1On)
            {
                star1.Source = "star1.png";
                star2.Source = "star1gray.png";
                star3.Source = "star1gray.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 20;
                star1On = true;
            }
            else
            {
                star1.Source = "star1gray.png";
                star2.Source = "star1gray.png";
                star3.Source = "star1gray.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 0;
                star1On = false;
            }
        }
        private void star2Clicked(object sender, EventArgs e)
        {
            if (!star2On)
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1gray.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 40;
                star2On = true;
            }
            else
            {
                star1.Source = "star1.png";
                star2.Source = "star1gray.png";
                star3.Source = "star1gray.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 20;
                star2On = false;
            }
        }
        private void star3Clicked(object sender, EventArgs e)
        {
            if (!star3On)
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 60;
                star3On = true;
            }
            else
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1gray.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 40;
                star3On = false;
            }
        }
        private void star4Clicked(object sender, EventArgs e)
        {
            if (!star4On)
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1.png";
                star4.Source = "star1.png";
                star5.Source = "star1gray.png";
                valoracion = 80;
                star4On = true;
            }
            else
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1.png";
                star4.Source = "star1gray.png";
                star5.Source = "star1gray.png";
                valoracion = 60;
                star4On = false;
            }
        }
        private void star5Clicked(object sender, EventArgs e)
        {
            if (!star5On)
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1.png";
                star4.Source = "star1.png";
                star5.Source = "star1.png";
                valoracion = 100;
                star5On = true;
            }
            else
            {
                star1.Source = "star1.png";
                star2.Source = "star1.png";
                star3.Source = "star1.png";
                star4.Source = "star1.png";
                star5.Source = "star1gray.png";
                valoracion = 80;
                star5On = false;
            }
        }
        private async void sendValoracion(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(enNombre.Text))
            {
                string urlRequest = globalValues.webSite
                    + "valoracion.php"
                    + "?tipoMov=A"
                    + "&Usuario=" + musico
                    + "&valoracion=" + valoracion
                    + "&nombre=" + enNombre.Text
                    + "&mensaje=" + edMensaje.Text
                    + "&fecha=" + DateTime.Now.ToString("yyyy-MM-dd");

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                    var db = new SQLiteConnection(databasePath);

                    db.CreateTable<T_MusicoContactado>();

                    db.Update(new T_MusicoContactado
                    {
                        id_usuario = musico,
                        estatus = "V"
                    });

                    await PopupNavigation.PopAsync();
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                    await PopupNavigation.PopAsync();
                }
            }
            else
            {
                enNombre.Focus();
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Ingrese su nombre", "Ok");
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}