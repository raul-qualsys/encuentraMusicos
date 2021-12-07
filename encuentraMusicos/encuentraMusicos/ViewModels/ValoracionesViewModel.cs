using encuentraMusicos.Classes;
using encuentraMusicos.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;

namespace encuentraMusicos.ViewModels
{
    public class ValoracionesViewModel
    {
        double resolution;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public int numValoraciones;
        public double promValoraciones=0;
        public ObservableCollection<ValoracionesClientes> valoraciones { get; set; }
        public ValoracionesViewModel(string idMusico)
        {
            int altoEstrella, sizeNombre, sizeMensaje;
            double sumValoraciones = 0;

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;
            if (resolution > 2000000)
            {
                altoEstrella = 10;
                sizeNombre = 14;
                sizeMensaje = 14;
            }
            else
            {
                altoEstrella = 10;
                sizeNombre = 10;
                sizeMensaje = 10;
            }

            string urlRequest = globalValues.webSite
                + "valoracion.php"
                + "?tipoMov=S"
                + "&Usuario=" + idMusico;

            string responseSelect = client.GetStringAsync(urlRequest).Result;

            ObservableCollection<ValoracionesClientes> valoracionesClientes = new ObservableCollection<ValoracionesClientes>();

            if (!responseSelect.Equals("[]"))
            {
                JObject regResponse = JObject.Parse(responseSelect);

                numValoraciones = regResponse.Count;

                for (int i = 0; i < regResponse.Count; i++)
                {
                    ValoracionesClientes valoracion = new ValoracionesClientes();
                    valoracion.id_musico = idMusico;
                    valoracion.nombre= regResponse["valoracion" + i]["nombre"].ToString();
                    valoracion.valoracion = Convert.ToDouble(regResponse["valoracion" + i]["valoracion"].ToString());
                    valoracion.mensaje= regResponse["valoracion" + i]["mensaje"].ToString();
                    valoracion.fecha= regResponse["valoracion" + i]["fecha_valoracion"].ToString();

                    if (valoracion.valoracion < 20)
                    {
                        valoracion.star1IV = false;
                        valoracion.star2IV = false;
                        valoracion.star3IV = false;
                        valoracion.star4IV = false;
                        valoracion.star5IV = false;
                    }

                    if (valoracion.valoracion >= 20 && valoracion.valoracion < 40)
                    {
                        valoracion.star1IV = true;
                        valoracion.star2IV = false;
                        valoracion.star3IV = false;
                        valoracion.star4IV = false;
                        valoracion.star5IV = false;
                    }

                    if (valoracion.valoracion >= 40 && valoracion.valoracion < 60)
                    {
                        valoracion.star1IV = true;
                        valoracion.star2IV = true;
                        valoracion.star3IV = false;
                        valoracion.star4IV = false;
                        valoracion.star5IV = false;
                    }

                    if (valoracion.valoracion >= 60 && valoracion.valoracion < 80)
                    {
                        valoracion.star1IV = true;
                        valoracion.star2IV = true;
                        valoracion.star3IV = true;
                        valoracion.star4IV = false;
                        valoracion.star5IV = false;
                    }

                    if (valoracion.valoracion >= 80 && valoracion.valoracion < 100)
                    {
                        valoracion.star1IV = true;
                        valoracion.star2IV = true;
                        valoracion.star3IV = true;
                        valoracion.star4IV = true;
                        valoracion.star5IV = false;
                    }

                    if (valoracion.valoracion == 100)
                    {
                        valoracion.star1IV = true;
                        valoracion.star2IV = true;
                        valoracion.star3IV = true;
                        valoracion.star4IV = true;
                        valoracion.star5IV = true;
                    }

                    valoracion.heightStar = altoEstrella;
                    valoracion.fontSizeNombre = sizeNombre;
                    valoracion.fontSizeMensaje = sizeMensaje;

                    sumValoraciones = sumValoraciones + valoracion.valoracion;

                    if (!string.IsNullOrEmpty(valoracion.mensaje))
                    {
                        valoracionesClientes.Add(valoracion);
                    }
                }
                promValoraciones = sumValoraciones / numValoraciones;
            }
            valoraciones = valoracionesClientes;
        }
    }
}
