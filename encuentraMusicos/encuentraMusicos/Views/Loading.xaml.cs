using encuentraMusicos.DataBase.Tables;
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
using encuentraMusicos.Classes;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Plugin.LocalNotification;
using System.Globalization;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loading : ContentPage
    {
        bool isAdmin=false;
        DateTime fechaNotif;
        bool conexion;
        double resolution;
        string isActive;
        string tpMusico;
        int existUsuario = 0;
        string idUsuario;
        string nombreContactado;
        int addDays;
        string diaNotifMusico;
        string buildNumber, dbBuildnumber;
        double diasValoracion;
        GlobalValues globalValues = new GlobalValues();
        HttpClient client = new HttpClient();
        public Loading()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            buildNumber = AppInfo.BuildString;

            NotificationCenter.Current.NotificationTapped += OnLocalNotificationTapped;

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);

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

                string urlRequestVersion = globalValues.webSite
                    + "check_version.php";

                string responseVersion = client.GetStringAsync(urlRequestVersion).Result;

                if (!responseVersion.Equals("[]"))
                {
                    JObject regVersion = JObject.Parse(responseVersion);

                    string IosVersion = regVersion["platform"]["IOS"].ToString();
                    string AndVersion = regVersion["platform"]["ANDROID"].ToString();

                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        dbBuildnumber = IosVersion;
                    }
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        dbBuildnumber = AndVersion;
                    }
                }

                string urlRequestConfigNotif = globalValues.webSite
                    + "conf_notificaciones.php"
                    + "?tpBusqueda=S";

                string responseConfig = client.GetStringAsync(urlRequestConfigNotif).Result;

                if (!responseConfig.Equals("[]"))
                {
                    JObject regConfig = JObject.Parse(responseConfig);

                    diaNotifMusico = regConfig["confNotificaciones"]["dia_notif_musico"].ToString();
                    diasValoracion = Convert.ToDouble(regConfig["confNotificaciones"]["dias_notif_valoracion"].ToString());
                }

                db.CreateTable<T_Registro>();
                var resultado = db.Query<T_Registro>("SELECT * from USUARIOS");

                existUsuario = resultado.Count();
                foreach (var s in resultado)
                {
                    isActive = s.isActive.ToString();
                    idUsuario = s.Id;
                }

                /**/
                if (!string.IsNullOrEmpty(idUsuario))
                {
                    //Inicia validación de administrador
                    string urlIsAdmin = globalValues.webSite
                        + "check_admin.php"
                        + "?Usuario="+ idUsuario;

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

                    /*Registro de Notificaciones*/

                    string today = DateTime.Now.ToString("dddd", new CultureInfo("es-ES"));

                    switch (today)
                    {
                        case "domingo":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 0;
                                    break;
                                case "LUN":
                                    addDays = 1;
                                    break;
                                case "MAR":
                                    addDays = 2;
                                    break;
                                case "MIE":
                                    addDays = 3;
                                    break;
                                case "JUE":
                                    addDays = 4;
                                    break;
                                case "VIE":
                                    addDays = 5;
                                    break;
                                case "SAB":
                                    addDays = 6;
                                    break;
                            }
                            break;
                        case "lunes":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 6;
                                    break;
                                case "LUN":
                                    addDays = 0;
                                    break;
                                case "MAR":
                                    addDays = 1;
                                    break;
                                case "MIE":
                                    addDays = 2;
                                    break;
                                case "JUE":
                                    addDays = 3;
                                    break;
                                case "VIE":
                                    addDays = 4;
                                    break;
                                case "SAB":
                                    addDays = 5;
                                    break;
                            }
                            break;
                        case "martes":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 5;
                                    break;
                                case "LUN":
                                    addDays = 6;
                                    break;
                                case "MAR":
                                    addDays = 0;
                                    break;
                                case "MIE":
                                    addDays = 1;
                                    break;
                                case "JUE":
                                    addDays = 2;
                                    break;
                                case "VIE":
                                    addDays = 3;
                                    break;
                                case "SAB":
                                    addDays = 4;
                                    break;
                            }
                            break;
                        case "miércoles":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 4;
                                    break;
                                case "LUN":
                                    addDays = 5;
                                    break;
                                case "MAR":
                                    addDays = 6;
                                    break;
                                case "MIE":
                                    addDays = 0;
                                    break;
                                case "JUE":
                                    addDays = 1;
                                    break;
                                case "VIE":
                                    addDays = 2;
                                    break;
                                case "SAB":
                                    addDays = 3;
                                    break;
                            }
                            break;
                        case "jueves":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 3;
                                    break;
                                case "LUN":
                                    addDays = 4;
                                    break;
                                case "MAR":
                                    addDays = 5;
                                    break;
                                case "MIE":
                                    addDays = 6;
                                    break;
                                case "JUE":
                                    addDays = 0;
                                    break;
                                case "VIE":
                                    addDays = 1;
                                    break;
                                case "SAB":
                                    addDays = 2;
                                    break;
                            }
                            break;
                        case "viernes":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 2;
                                    break;
                                case "LUN":
                                    addDays = 3;
                                    break;
                                case "MAR":
                                    addDays = 4;
                                    break;
                                case "MIE":
                                    addDays = 5;
                                    break;
                                case "JUE":
                                    addDays = 6;
                                    break;
                                case "VIE":
                                    addDays = 0;
                                    break;
                                case "SAB":
                                    addDays = 1;
                                    break;
                            }
                            break;
                        case "sábado":
                            switch (diaNotifMusico)
                            {
                                case "DOM":
                                    addDays = 1;
                                    break;
                                case "LUN":
                                    addDays = 2;
                                    break;
                                case "MAR":
                                    addDays = 3;
                                    break;
                                case "MIE":
                                    addDays = 4;
                                    break;
                                case "JUE":
                                    addDays = 5;
                                    break;
                                case "VIE":
                                    addDays = 6;
                                    break;
                                case "SAB":
                                    addDays = 0;
                                    break;
                            }
                            break;
                    }

                    fechaNotif = DateTime.Now.Date.AddDays(addDays).AddHours(10);

                    var notification = new NotificationRequest
                    {
                        NotificationId = Convert.ToInt32(1),
                        Title = "Consejos Encuentra Músico",
                        Description = "Cuando termines un evento no te olvides de pedir tu calificación al cliente",
                        NotifyTime = fechaNotif,
                        Android =
                        {
                            IconName = "logoloading.png"
                        }
                    };
                    NotificationCenter.Current.Show(notification);

                    /*Termina Registro de Notificaciones*/

                    /*Validación de Tipo de Músico*/
                    string urlExistUsuario = globalValues.webSite
                        + "datos_pers_musico.php"
                        + "?tpBusqueda=S"
                        + "&idUsuario=" + idUsuario;

                    string responseExiste = client.GetStringAsync(urlExistUsuario).Result;

                    if (!responseExiste.Equals("[]"))
                    {
                        JObject regExiste = JObject.Parse(responseExiste);

                        tpMusico = regExiste["datosPersonalesM"]["tipo_musico"].ToString();

                        if (string.IsNullOrEmpty(tpMusico))
                        {
                            tpMusico = "S";
                        }

                        db.Query<T_Registro>("Update USUARIOS set tpMusico ='" + tpMusico + "' where Id = '" + idUsuario + "'");
                    }
                }
                /*Termina Validación de Tipo de Músico*/

                db.CreateTable<T_MusicoContactado>();
                
                var resultadoValor = db.Query<T_MusicoContactado>("SELECT * from CONTACTADOS where estatus='I'");
                
                int k = 0;

                DateTime fechac=DateTime.Now;

                foreach (var s in resultadoValor)
                {
                    k++;
                    if (k==1)
                    {
                        nombreContactado = s.nombre_usuario;
                        fechac = s.fecha_contacto;
                    }
                }
                
                if (k>=1)
                {
                    var notification = new NotificationRequest
                    {
                        NotificationId = Convert.ToInt32(2),
                        Title = "Encuentra Músico",
                        Description = "¿Tuviste un evento con "+nombreContactado+"? Danos tu opinión al respecto.",
                        NotifyTime = fechac,
                        Android =
                        {
                            IconName = "logoloading.png"
                        }
                    };
                    NotificationCenter.Current.Show(notification);
                }
                /**/
                db.CreateTable<T_Catalogos>();

                db.DeleteAll<T_Catalogos>();

                string urlRequest = globalValues.webSite
                    + "load_catalog.php"
                    + "?tpCatalogo=generosMusicales";

                string responseSelect = client.GetStringAsync(urlRequest).Result;

                JObject regResponse = JObject.Parse(responseSelect);

                for (int i = 0; i < regResponse.Count; i++)
                {
                    var newCatalogItem = new T_Catalogos();

                    newCatalogItem.tipo_translate = "GENERO_MUSICAL";
                    newCatalogItem.code_translate = regResponse["generoMusical" + i]["cd_translate"].ToString();
                    newCatalogItem.descripcion = regResponse["generoMusical" + i]["descripcion"].ToString();
                    db.Insert(newCatalogItem);
                }

                string urlRequestEstados = globalValues.webSite
                    + "load_catalog.php"
                    + "?tpCatalogo=estadosMX";

                string responseSelectEstados = client.GetStringAsync(urlRequestEstados).Result;

                JObject regResponseEstados = JObject.Parse(responseSelectEstados);

                for (int i = 0; i < regResponseEstados.Count; i++)
                {
                    var newEstadoItem = new T_Catalogos();

                    newEstadoItem.tipo_translate = "ESTADOS_MX";
                    newEstadoItem.code_translate = regResponseEstados["estado" + i]["cd_translate"].ToString();
                    newEstadoItem.descripcion = regResponseEstados["estado" + i]["descripcion"].ToString();
                    db.Insert(newEstadoItem);
                }
            }
            else
            {
                conexion = false;
                sinConexion.IsVisible = true;
                gridPrincipal.IsVisible = false;
                gridPrincipal.HeightRequest = 0;
            }
            
            if (resolution > 2000000)
            {
                logoLoading.Margin = new Thickness(100, 250, 100, 0);
                lbSinConexion.FontSize = 22;
                reintentarBtn.FontSize = 20;
            }
            else
            {
                logoLoading.Margin = new Thickness(85,100,85,15);
                lbSinConexion.FontSize = 14;
                reintentarBtn.FontSize = 12;
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await pgBar.ProgressTo(1, 2500, Easing.Linear);
            string url;
            //Application.Current.MainPage = new NavigationPage(new SelectUsuario());
            if (!buildNumber.Equals(dbBuildnumber))
            {

                Application.Current.MainPage = new NavigationPage(new updateApp());

            }
            else if (existUsuario > 0)
            {
                if (isActive.Equals("Y"))
                {
                    if (isAdmin)
                    {
                        Application.Current.MainPage = new NavigationPage(new Administracion(isAdmin, idUsuario));
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new PerfilMusico(idUsuario, tpMusico));
                    }
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new validacionEmail(idUsuario));
                }
            }
            else
            {
                if (conexion)
                {
                    Application.Current.MainPage = new NavigationPage(new SelectUsuario());
                }
            }
        }
        private void reintentar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Loading());
        }
        private void OnLocalNotificationTapped(NotificationTappedEventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalBusqueda(idUsuario, tpMusico));
        }
        protected override bool OnBackButtonPressed() => true;
    }
}