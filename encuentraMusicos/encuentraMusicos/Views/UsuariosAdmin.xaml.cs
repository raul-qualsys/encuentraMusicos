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
    public partial class UsuariosAdmin : ContentPage
    {
        bool conexion;
        double resolution;
        string usuario;
        string busquedaOrig;
        string selectOption;
        public UsuariosAdmin(string idUsuario, string busqueda, string swOption)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            usuario = idUsuario;
            busquedaOrig = busqueda;
            selectOption = swOption;

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

                busquedaMusico.Text = busquedaOrig;

                MusicosAdminViewModel vm = new MusicosAdminViewModel(busquedaOrig, selectOption);
                BindingContext = vm;

                if (resolution > 2000000)
                {
                    lbAdmin.FontSize = 22;
                    busquedaMusico.FontSize = 14;
                    rbTodos.FontSize = 14;
                    rbActivos.FontSize = 14;
                    rbInactivos.FontSize = 14;
                    regresarBtn.FontSize = 18;
                }
                else
                {
                    lbAdmin.FontSize = 14;
                    busquedaMusico.FontSize = 10;
                    rbTodos.FontSize = 10;
                    rbActivos.FontSize = 10;
                    rbInactivos.FontSize = 10;
                    regresarBtn.FontSize = 12;
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
            Application.Current.MainPage = new NavigationPage(new Administracion(true, usuario));
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

            MusicosAdminViewModel vm = new MusicosAdminViewModel(e.NewTextValue, selectOption);
            BindingContext = vm;
        }
        private void todosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbTodos.IsChecked)
            {
                selectOption = "T";
                rbActivos.IsChecked = false;
                rbInactivos.IsChecked = false;
                MusicosAdminViewModel vm = new MusicosAdminViewModel(busquedaMusico.Text, selectOption);
                BindingContext = vm;
            }
        }
        private void activosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbActivos.IsChecked)
            {
                selectOption = "A";
                rbTodos.IsChecked = false;
                rbInactivos.IsChecked = false;
                MusicosAdminViewModel vm = new MusicosAdminViewModel(busquedaMusico.Text, selectOption);
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
                MusicosAdminViewModel vm = new MusicosAdminViewModel(busquedaMusico.Text, selectOption);
                BindingContext = vm;
            }
        }
        private async void selectGrupo(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<object> currentSelectedEvent = e.CurrentSelection;
            MusicosAdmin selectedMusico = currentSelectedEvent.FirstOrDefault() as MusicosAdmin;

            var pr = new popUpDetalleMusico(selectedMusico, usuario, busquedaOrig, selectOption);
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