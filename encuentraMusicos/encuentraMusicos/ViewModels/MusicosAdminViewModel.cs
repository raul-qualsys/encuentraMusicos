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
    public class MusicosAdminViewModel
    {
        double resolution;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public ObservableCollection<MusicosAdmin> musicos { get; set; }
        public MusicosAdminViewModel(string busqueda, string swOption)
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;
            int heightImage, lbNombreFntSize, imStarHeight;
            double darkOpacity;

            if (string.IsNullOrEmpty(busqueda))
            {
                busqueda = "";
            }

            resolution = width * height;
            if (resolution > 2000000)
            {
                heightImage = 200;
                darkOpacity = 0.3;
                lbNombreFntSize = 18;
                imStarHeight = 20;
            }
            else
            {
                heightImage = 150;
                darkOpacity = 0.6;
                lbNombreFntSize = 12;
                imStarHeight = 10;
            }

            string urlRequest = globalValues.webSite
                + "admin_musicos.php"
                + "?tpBusqueda=S"
                + "&cadBusqueda=" + busqueda;

            string responseSelect = client.GetStringAsync(urlRequest).Result;

            if (!responseSelect.Equals("[]"))
            {
                JObject regResponse = JObject.Parse(responseSelect);

                ObservableCollection<MusicosAdmin> gruposMusicales = new ObservableCollection<MusicosAdmin>();

                for (int i = 0; i < regResponse.Count; i++)
                {
                    MusicosAdmin musico = new MusicosAdmin();
                    musico.id_usuario = regResponse["Musico" + i]["id_usuario"].ToString();
                    musico.nombre_musico = regResponse["Musico" + i]["nombre_musico"].ToString();
                    musico.descripcion = regResponse["Musico" + i]["descripcion"].ToString();
                    if (!string.IsNullOrEmpty(regResponse["Musico" + i]["valoracion"].ToString()))
                    {
                        musico.valoracion = Convert.ToDouble(regResponse["Musico" + i]["valoracion"].ToString());
                    }
                    else
                    {
                        musico.valoracion = 0;
                    }
                    musico.tipo_musico = regResponse["Musico" + i]["tipo_musico"].ToString();
                    musico.fecha_registro = DateTime.Parse(regResponse["Musico" + i]["fecha_registro"].ToString());
                    musico.is_active = regResponse["Musico" + i]["is_active"].ToString();
                    if (musico.is_active.Equals("Y"))
                    {
                        musico.isActiveDescr = "Activo";
                    }
                    else
                    {
                        musico.isActiveDescr = "Suspendido";
                    }
                    musico.id_path = regResponse["Musico" + i]["id_path"].ToString();
                    musico.nombre_media = globalValues.webSite+ "Images/user_profile/" + musico.id_path + "/" + regResponse["Musico" + i]["nombre_media"].ToString();
                    musico.heightImage = heightImage;
                    musico.darkOpacity = darkOpacity;
                    musico.lbNombreFntSize = lbNombreFntSize;
                    musico.imStarHeight = imStarHeight;
                    if (musico.valoracion < 20)
                    {
                        musico.star1 = false;
                        musico.star2 = false;
                        musico.star3 = false;
                        musico.star4 = false;
                        musico.star5 = false;
                    }

                    if (musico.valoracion >= 20 && musico.valoracion < 40)
                    {
                        musico.star1 = true;
                        musico.star2 = false;
                        musico.star3 = false;
                        musico.star4 = false;
                        musico.star5 = false;
                    }

                    if (musico.valoracion >= 40 && musico.valoracion < 60)
                    {
                        musico.star1 = true;
                        musico.star2 = true;
                        musico.star3 = false;
                        musico.star4 = false;
                        musico.star5 = false;
                    }

                    if (musico.valoracion >= 60 && musico.valoracion < 80)
                    {
                        musico.star1 = true;
                        musico.star2 = true;
                        musico.star3 = true;
                        musico.star4 = false;
                        musico.star5 = false;
                    }

                    if (musico.valoracion >= 80 && musico.valoracion < 100)
                    {
                        musico.star1 = true;
                        musico.star2 = true;
                        musico.star3 = true;
                        musico.star4 = true;
                        musico.star5 = false;
                    }

                    if (musico.valoracion == 100)
                    {
                        musico.star1 = true;
                        musico.star2 = true;
                        musico.star3 = true;
                        musico.star4 = true;
                        musico.star5 = true;
                    }

                    if (swOption.Equals("T"))
                    {
                        gruposMusicales.Add(musico);
                    }
                    else if (swOption.Equals("A") && musico.is_active.Equals("Y"))
                    {
                        gruposMusicales.Add(musico);
                    }
                    else if (swOption.Equals("I") && musico.is_active.Equals("N"))
                    {
                        gruposMusicales.Add(musico);
                    }
                }
                musicos = gruposMusicales;
            }
        }
    }
}
