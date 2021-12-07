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
    public class GenerosAdminViewModel
    {
        public double maxCode = 0;
        double resolution;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public ObservableCollection<GenerosAdmin> catalogo { get; set; }

        public GenerosAdminViewModel(string busqueda, string swOption)
        {
            ObservableCollection<GenerosAdmin> elementosCatalogo = new ObservableCollection<GenerosAdmin>();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            int tamFuente, tamFuenteEstatus;

            if (string.IsNullOrEmpty(busqueda))
            {
                busqueda = "";
            }

            resolution = width * height;
            if (resolution > 2000000)
            {
                tamFuente = 16;
                tamFuenteEstatus = 12;
            }
            else
            {
                tamFuente = 12;
                tamFuenteEstatus = 8;
            }

            string urlRequest = globalValues.webSite
                + "admin_catalog.php"
                + "?tpBusqueda=S"
                + "&tpCatalogo=generosMusicales";

            string responseSelect = client.GetStringAsync(urlRequest).Result;

            if (!responseSelect.Equals("[]"))
            {
                JObject regResponse = JObject.Parse(responseSelect);

                for (int i = 0; i < regResponse.Count; i++)
                {
                    GenerosAdmin elemento = new GenerosAdmin();
                    elemento.code= regResponse["generoMusical" + i]["cd_translate"].ToString();
                    if (Convert.ToDouble(elemento.code) > maxCode)
                    {
                        maxCode = Convert.ToDouble(elemento.code);
                    }
                    elemento.descripcion = regResponse["generoMusical" + i]["descripcion"].ToString();
                    elemento.estatus = regResponse["generoMusical" + i]["estatus"].ToString();
                    elemento.fontSz = tamFuente;
                    elemento.fntszDescrEstatus = tamFuenteEstatus;

                    if (elemento.estatus.Equals("I"))
                    {
                        elemento.descrEstatus = "Inactivo";
                        elemento.opacityEstatus = 0.5;
                    }
                    else
                    {
                        elemento.descrEstatus = "Activo";
                        elemento.opacityEstatus = 0;
                    }

                    if (elemento.descripcion.ToLower().Contains(busqueda.ToLower()))
                    {
                        if (swOption.Equals("T"))
                        {
                            elementosCatalogo.Add(elemento);
                        }
                        else if (swOption.Equals("A") && elemento.estatus.Equals("A"))
                        {
                            elementosCatalogo.Add(elemento);
                        }
                        else if (swOption.Equals("I") && elemento.estatus.Equals("I"))
                        {
                            elementosCatalogo.Add(elemento);
                        }
                    }
                }
            }
            catalogo = elementosCatalogo;
        }
    }
}
