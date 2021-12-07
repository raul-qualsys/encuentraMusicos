using encuentraMusicos.Classes;
using encuentraMusicos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;

namespace encuentraMusicos.ViewModels
{
    public class DireccionesViewModel
    {
        public ObservableCollection<Direcciones> Direccion { get; set; }
        public int numDirecciones;
        GlobalValues globalValues = new GlobalValues();
        public DireccionesViewModel(string idUsuario)
        {
            HttpClient client = new HttpClient();

            ObservableCollection<Direcciones> listDirecciones = new ObservableCollection<Direcciones>();

            string requestUrl = globalValues.webSite
                + "ubicaciones_musico.php"
                + "?tpBusqueda=S"
                + "&idUsuario=" + idUsuario;
            string responseDirecciones = client.GetStringAsync(requestUrl).Result;
            if (responseDirecciones != "[]")
            {
                JObject jsonDireccion = JObject.Parse(responseDirecciones);

                numDirecciones = jsonDireccion.Count;

                if (jsonDireccion.Count > 0)
                {
                    for (int i = 0; i < jsonDireccion.Count; i++)
                    {
                        string contDireccion = jsonDireccion["ubicacion" + i].ToString();
                        Direcciones direccion = JsonConvert.DeserializeObject<Direcciones>(contDireccion);

                        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                        var width = mainDisplayInfo.Width;
                        var height = mainDisplayInfo.Height;

                        var resolution = width * height;

                        if (resolution > 2000000)
                        {
                            direccion.nombreFntSize = 18;
                        }
                        else
                        {
                            direccion.nombreFntSize = 14;
                        }

                        direccion.detailVisible = false;

                        listDirecciones.Add(direccion);
                    }
                }
            }
            Direccion = listDirecciones;
        }
    }
}
