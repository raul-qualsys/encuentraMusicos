using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using encuentraMusicos.Classes;
using encuentraMusicos.Models;
using encuentraMusicos.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace encuentraMusicos.ViewModels
{
    public class ExportingExcelViewModel
    {
        public ICommand ExportToExcelCommand { private set; get; }
        private ExcelServices excelService;
        public ObservableCollection<ReporteDB> filas;
        string usuario;
        bool descargaLocal=false;
        public string generosExec, tarifasExec, contactoExec, direccionesExec, valoracionesExec, estatus_perfilExec, estatusExec;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public ExportingExcelViewModel(string idUsuario, bool local, string generos, string tarifas, string contacto, string direcciones, string valoraciones, string estatus_perfil, string estatus)
        {
            ObservableCollection<ReporteDB> listaMusicos = new ObservableCollection<ReporteDB>();

            usuario = idUsuario;
            generosExec = generos;
            tarifasExec = tarifas;
            contactoExec = contacto;
            direccionesExec = direcciones;
            valoracionesExec = valoraciones;
            estatus_perfilExec = estatus_perfil;
            estatusExec=estatus;
            descargaLocal = local;

            string urlRequest= globalValues.webSite
                + "reportdb.php"
                + "?tpBusqueda=S"
                + "&generos="+generos
                + "&tarifas="+tarifas
                + "&contacto="+contacto
                + "&direcciones="+direcciones
                + "&valoraciones="+valoraciones
                + "&estatusPerfil="+estatus_perfil;

            string responseReporte = client.GetStringAsync(urlRequest).Result;
            if (responseReporte != "[]")
            {
                JObject jsonElementos = JObject.Parse(responseReporte);

                if (jsonElementos.Count > 0)
                {
                    for (int i = 0; i < jsonElementos.Count; i++)
                    {
                        string contElementos = jsonElementos["musico" + i].ToString();
                        ReporteDB fila = JsonConvert.DeserializeObject<ReporteDB>(contElementos);

                        double porcentaje = 0;

                        if (estatus_perfil.Equals("Y"))
                        {
                            if (fila.datos_musico.Equals("Y"))
                            {
                                porcentaje = porcentaje + 20;
                            }
                            if (fila.generos_musicales.Equals("Y"))
                            {
                                porcentaje = porcentaje + 20;
                            }
                            if (fila.tarifas.Equals("Y"))
                            {
                                porcentaje = porcentaje + 20;
                            }
                            if (fila.contacto.Equals("Y"))
                            {
                                porcentaje = porcentaje + 20;
                            }
                            if (fila.ubicaciones.Equals("Y"))
                            {
                                porcentaje = porcentaje + 20;
                            }
                        }

                        if (estatus_perfilExec.Equals("Y"))
                        {
                            fila.porcentaje_estatus = porcentaje.ToString();
                        }

                        if (estatus.Equals("T"))
                        {
                            listaMusicos.Add(fila);
                        }else if(estatus.Equals("A") && fila.estatus.Equals("Activo"))
                        {
                            listaMusicos.Add(fila);
                        }else if(estatus.Equals("I") && fila.estatus.Equals("Suspendido"))
                        {
                            listaMusicos.Add(fila);
                        }
                    }
                }
            }

            filas = listaMusicos;

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            excelService = new ExcelServices();
        }
        async Task ExportToExcel()
        {
            var fileName = $"reporteMusicos-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            string filepath = excelService.GenerateExcel(fileName);

            var data = new ExcelStructure
            {
                Headers = new List<string>() { "Id Usuario",
                    "Nombre Músico", 
                    "Descripción", 
                    "Tipo",
                    "Video URL",
                    "Fecha de Registro",
                    "Género Musical",
                    "Precio",
                    "Texto Adicional",
                    "Teléfono",
                    "Whatsapp",
                    "Facebook",
                    "eMail",
                    "Nombre Ubicación",
                    "Calle y No.",
                    "Código Postal",
                    "Colonia",
                    "Municipio",
                    "Estado",
                    "País",
                    "Kilometros",
                    "Nombre cliente",
                    "Valoración",
                    "Mensaje",
                    "Fecha de Valoración",
                    "Datos músico",
                    "Géneros musicales",
                    "Tarifas",
                    "Contacto",
                    "Ubicaciones",
                    "Porcentaje Completado",
                    "Estatus"
                }
            };

            foreach (var item in filas)
            {
                data.Values.Add(new List<string>() { item.id_usuario,
                    item.nombre_musico,
                    item.descripcion_musico,
                    item.tipo_musico,
                    item.urluser_video,
                    item.fecha_registro,
                    item.genero_musical,
                    item.precio,
                    item.texto,
                    item.telefono,
                    item.whatsapp,
                    item.facebook,
                    item.email,
                    item.titulo,
                    item.calleno,
                    item.postal,
                    item.colonia,
                    item.municipio,
                    item.estado,
                    item.pais,
                    item.kilometros,
                    item.cliente,
                    item.valoracion,
                    item.mensaje,
                    item.fecha_valoracion,
                    item.datos_musico,
                    item.generos_musicales,
                    item.tarifas,
                    item.contacto,
                    item.ubicaciones,
                    item.porcentaje_estatus,
                    item.estatus});
            }

            excelService.InsertDataIntoSheet(filepath, "Musicos", data);

            string From = filepath;
            string To = globalValues.hostFTP + "/Reportes/" + fileName;

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(globalValues.userFTP, globalValues.passwordFTP);
                client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, From);
            }

            string urlCorreoReporte = globalValues.webSite
                        + "reporte_mail.php"
                        + "?email=" + usuario
                        + "&archivo=" + globalValues.webSite+ "Images/user_profile/Reportes/"+fileName;

            string responseEmail = client.GetStringAsync(urlCorreoReporte).Result;

            JObject regResponse = JObject.Parse(responseEmail);

            string result = regResponse["success"].ToString();

            if (result.Equals("1"))
            {
                Application.Current.MainPage.DisplayAlert("Mensaje Enviado", "El reporte fue enviado con éxito a " + usuario, "Ok");
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Ocurrió un error", "Intente más tarde", "Ok");
            }

            if (descargaLocal)
            {
                await Launcher.OpenAsync(new OpenFileRequest()
                {
                    File = new ReadOnlyFile(filepath)
                });
            }
        }
    }
}
