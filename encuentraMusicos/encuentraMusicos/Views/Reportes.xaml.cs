using encuentraMusicos.ViewModels;
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
    public partial class Reportes : ContentPage
    {
        double resolution;
        bool conexion;
        string usuario;
        string generos;
        string tarifas;
        string contacto;
        string direcciones;
        string valoraciones;
        string estatus_perfil;
        string estatus;
        bool descargaLocal = false;
        public Reportes(string idUsuario)
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

                descargaLocal = false;
                generos = "N";
                tarifas = "N";
                contacto = "N";
                direcciones = "N";
                valoraciones = "N";
                estatus_perfil= "N";
                rbTodos.IsChecked = true;
                rbActivos.IsChecked = false;
                rbInactivos.IsChecked = false;
                estatus = "T";

                BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);

                if (resolution > 2000000)
                {
                    lbReporte.FontSize = 22;
                    crearReporteBtn.FontSize = 20;
                    lbGeneros.FontSize = 18;
                    lbTarifas.FontSize = 18;
                    lbContacto.FontSize = 18;
                    lbUbicaciones.FontSize = 18;
                    lbValoraciones.FontSize = 18;
                    lbEstatusPerfil.FontSize = 18;
                    lbEstatus.FontSize = 18;
                    rbTodos.FontSize = 16;
                    rbActivos.FontSize = 16;
                    rbInactivos.FontSize = 16;
                }
                else
                {
                    lbReporte.FontSize = 18;
                    crearReporteBtn.FontSize = 16;
                    lbGeneros.FontSize = 14;
                    lbTarifas.FontSize = 14;
                    lbContacto.FontSize = 14;
                    lbUbicaciones.FontSize = 14;
                    lbValoraciones.FontSize = 14;
                    lbEstatusPerfil.FontSize = 14;
                    lbEstatus.FontSize = 14;
                    rbTodos.FontSize = 10;
                    rbActivos.FontSize = 10;
                    rbInactivos.FontSize = 10;
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
        private void swGeneroToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                generos = "Y";
            }
            else
            {
                generos = "N";
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        private void swTarifaToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                tarifas = "Y";
            }
            else
            {
                tarifas = "N";
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        private void swContactoToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                contacto = "Y";
            }
            else
            {
                contacto = "N";
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        private void swUbicacionesToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                direcciones = "Y";
            }
            else
            {
                direcciones = "N";
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        private void swValoracionesToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                valoraciones = "Y";
            }
            else
            {
                valoraciones = "N";
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        private void swEstatusPerfilToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value.Equals(true))
            {
                estatus_perfil = "Y";
            }
            else
            {
                estatus_perfil = "N";
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        private void todosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbTodos.IsChecked)
            {
                estatus = "T";
                rbActivos.IsChecked = false;
                rbInactivos.IsChecked = false;
                BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
            }
        }
        private void activosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbActivos.IsChecked)
            {
                estatus = "A";
                rbTodos.IsChecked = false;
                rbInactivos.IsChecked = false;
                BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
            }
        }
        private void inactivosIsChecked(object sender, CheckedChangedEventArgs e)
        {
            if (rbInactivos.IsChecked)
            {
                estatus = "I";
                rbTodos.IsChecked = false;
                rbActivos.IsChecked = false;
                BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
            }
        }
        void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (descargaDispositivo.IsChecked)
            {
                descargaLocal = true;
            }
            else
            {
                descargaLocal = false;
            }
            BindingContext = new ExportingExcelViewModel(usuario, descargaLocal, generos, tarifas, contacto, direcciones, valoraciones, estatus_perfil, estatus);
        }
        protected override bool OnBackButtonPressed() => true;
    }
}