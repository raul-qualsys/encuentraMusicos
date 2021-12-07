using encuentraMusicos.Models;
using encuentraMusicos.ViewModels;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
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
    public partial class GenerosMusicalesAdmin : ContentPage
    {
        string usuario;
        string busquedaOrig;
        string selectOption;
        bool conexion;
        double resolution;
        double maxCdGeneros = 0;
        public GenerosMusicalesAdmin(string idUsuario, string busqueda, string selOption)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            busquedaOrig = busqueda;
            selectOption = selOption;

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

                if (selectOption.Equals("T"))
                {
                    rbTodos.IsChecked = true;
                    rbActivos.IsChecked = false;
                    rbInactivos.IsChecked = false;
                }
                else if (selectOption.Equals("A"))
                {
                    rbTodos.IsChecked = false;
                    rbActivos.IsChecked = true;
                    rbInactivos.IsChecked = false;
                }
                else if (selectOption.Equals("I"))
                {
                    rbTodos.IsChecked = false;
                    rbActivos.IsChecked = false;
                    rbInactivos.IsChecked = true;
                }

                busquedaGenero.Text = busquedaOrig;

                GenerosAdminViewModel vm = new GenerosAdminViewModel(busquedaOrig, selectOption);
                BindingContext = vm;

                maxCdGeneros = vm.maxCode;

                if (resolution > 2000000)
                {
                    lbGenerosAdmin.FontSize = 22;
                    regresarBtn.FontSize = 18;
                    busquedaGenero.FontSize = 14;
                    rbTodos.FontSize = 14;
                    rbActivos.FontSize = 14;
                    rbInactivos.FontSize = 14;
                    nuevoBtn.FontSize = 18;
                }
                else
                {
                    lbGenerosAdmin.FontSize = 12;
                    regresarBtn.FontSize = 12;
                    busquedaGenero.FontSize = 10;
                    rbTodos.FontSize = 10;
                    rbActivos.FontSize = 10;
                    rbInactivos.FontSize = 10;
                    nuevoBtn.FontSize = 12;
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
            Application.Current.MainPage = new NavigationPage(new Configuracion(usuario));
        }
        private void todosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbTodos.IsChecked)
            {
                selectOption = "T";
                rbActivos.IsChecked = false;
                rbInactivos.IsChecked = false;
                GenerosAdminViewModel vm = new GenerosAdminViewModel(busquedaGenero.Text, selectOption);
                BindingContext = vm;
            }
        }
        private void activosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbActivos.IsChecked)
            {
                selectOption = "A";
                rbTodos.IsChecked= false;
                rbInactivos.IsChecked = false;
                GenerosAdminViewModel vm = new GenerosAdminViewModel(busquedaGenero.Text, selectOption);
                BindingContext = vm;
            }
        }
        private void inactivosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbInactivos.IsChecked)
            {
                selectOption = "I";
                rbTodos.IsChecked = false;
                rbActivos.IsChecked = false;
                GenerosAdminViewModel vm = new GenerosAdminViewModel(busquedaGenero.Text, selectOption);
                BindingContext = vm;
            }
        }
        private void busqueda(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                busquedaOrig = e.NewTextValue;
            }
            else
            {
                busquedaOrig = "";
            }

            GenerosAdminViewModel vm = new GenerosAdminViewModel(e.NewTextValue, selectOption);
            BindingContext = vm;
        }
        private async void selectGenero(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<object> currentSelectedEvent = e.CurrentSelection;
            GenerosAdmin selectedGenero = currentSelectedEvent.FirstOrDefault() as GenerosAdmin;

            var pr = new popUpDetalleGenero(selectedGenero, usuario, busquedaOrig, "update", maxCdGeneros, selectOption);
            var scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Right,
                PositionOut = MoveAnimationOptions.Left
            };

            pr.Animation = scaleAnimation;
            pr.CloseWhenBackgroundIsClicked = false;
            await PopupNavigation.PushAsync(pr);
        }
        private async void addGenero(object sender, EventArgs e)
        {
            GenerosAdmin nuevoGenero = new GenerosAdmin();

            var pr = new popUpDetalleGenero(nuevoGenero, usuario, busquedaOrig, "insert", maxCdGeneros, selectOption);
            var scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Right,
                PositionOut = MoveAnimationOptions.Left
            };

            pr.Animation = scaleAnimation;
            pr.CloseWhenBackgroundIsClicked = false;
            await PopupNavigation.PushAsync(pr);
        }
        protected override bool OnBackButtonPressed() => true;
    }
}