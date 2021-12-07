using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using encuentraMusicos.Models;
using encuentraMusicos.Classes;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenerosMusicalesVW : ContentPage
    {
        bool conexion;
        double resolution;
        GenerosMusicalesViewModel vm;
        MisGenerosViewModel mv;
        string usuario;
        string tipoMusico;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public GenerosMusicalesVW(string idUsuario, string tpMusico)
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

                if (resolution > 2000000)
                {
                    lbTituloGenerosMusicales.FontSize = 22;
                    busquedaGeneroMusical.FontSize = 14;
                    regresarBtn.FontSize = 20;
                    lblTitle.FontSize = 20;
                }
                else
                {
                    lbTituloGenerosMusicales.FontSize = 14;
                    busquedaGeneroMusical.FontSize = 10;
                    regresarBtn.FontSize = 12;
                    lblTitle.FontSize = 12;
                }

                mv = new MisGenerosViewModel(usuario);
                BindingContext = mv;

                misGenerosCV.IsVisible = true;
                generosCollection.IsVisible = false;
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
        private void busquedaChanged(object sender, TextChangedEventArgs e)
        {
            vm = new GenerosMusicalesViewModel(e.NewTextValue);

            BindingContext = vm;

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                misGenerosCV.IsVisible = false;
                generosCollection.IsVisible = true;
            }
            else
            {
                mv = new MisGenerosViewModel(usuario);

                BindingContext = mv;

                misGenerosCV.IsVisible = true;
                generosCollection.IsVisible = false;
            }
        }
        void selectGenero(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<object> currentSelectedEvent = e.CurrentSelection;
            GenerosMusicales selectedGenero = currentSelectedEvent.FirstOrDefault() as GenerosMusicales;

            string urlAgregaGenero = globalValues.webSite
                + "generos_musico.php"
                + "?Usuario=" + usuario
                + "&tipoMov=A"
                + "&codigo="+selectedGenero.code;

            string guardaGenero = client.GetStringAsync(urlAgregaGenero).Result;

            JObject regResponse = JObject.Parse(guardaGenero);

            string result = regResponse["success"].ToString();

            if (result.Equals("1"))
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                var db = new SQLiteConnection(databasePath);

                db.CreateTable<T_MisGeneros>();

                T_MisGeneros newMiGeneroItem = new T_MisGeneros();

                newMiGeneroItem.code_translate = selectedGenero.code;
                newMiGeneroItem.descripcion = selectedGenero.descripcion;
                db.Insert(newMiGeneroItem);

                Application.Current.MainPage = new NavigationPage(new GenerosMusicalesVW(usuario, tipoMusico));
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new GenerosMusicalesVW(usuario, tipoMusico));
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
            }
        }
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private void goBack(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PerfilMusico(usuario, tipoMusico));
        }
        private async void eliminarGenero(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<object> currentSelectedEvent = e.CurrentSelection;
            GenerosMusicales selectedGenero = currentSelectedEvent.FirstOrDefault() as GenerosMusicales;

            bool answer = await DisplayAlert("Eliminar Género", "¿Desea eliminar "+selectedGenero.descripcion+"?", "Yes", "No");
            if (answer)
            {
                string urlEliminaGenero = globalValues.webSite
                + "generos_musico.php"
                + "?Usuario=" + usuario
                + "&tipoMov=D"
                + "&codigo=" + selectedGenero.code;

                string eliminaGenero = client.GetStringAsync(urlEliminaGenero).Result;

                JObject regResponse = JObject.Parse(eliminaGenero);

                string result = regResponse["success"].ToString();

                if (result.Equals("1"))
                {
                    var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
                    var db = new SQLiteConnection(databasePath);

                    db.CreateTable<T_MisGeneros>();

                    db.Delete<T_MisGeneros>(selectedGenero.code);
                    Application.Current.MainPage = new NavigationPage(new GenerosMusicalesVW(usuario, tipoMusico));
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new GenerosMusicalesVW(usuario, tipoMusico));
                    Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
                }
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new GenerosMusicalesVW(usuario, tipoMusico));
            }
        }
        protected override bool OnBackButtonPressed() => true;
    }
}