using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using encuentraMusicos.Classes;
using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.Models;
using Newtonsoft.Json.Linq;
using SQLite;
using Xamarin.Essentials;

namespace encuentraMusicos.ViewModels
{
    public class BusquedaViewModel
    {
        double resolution;
        HttpClient client = new HttpClient();
        GlobalValues globalValues = new GlobalValues();
        public int numResultados;
        public ObservableCollection<GruposMusicales> resultados { get; set; }
        public BusquedaViewModel(string busqueda, double longSearch, double latSearch, string precio)
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;
            int heightImage, lbNombreFntSize, actFntSize, imStarHeight, lbPrecioFontsize, hgtActividad;
            double darkOpacity;

            resolution = width * height;

            if (resolution > 2000000)
            {
                heightImage = 200;
                lbNombreFntSize = 18;
                actFntSize = 14;
                imStarHeight = 20;
                lbPrecioFontsize = 16;
                darkOpacity = 0.3;
            }
            else
            {
                heightImage = 150;
                lbNombreFntSize = 12;
                actFntSize = 10;
                imStarHeight = 10;
                lbPrecioFontsize = 10;
                darkOpacity = 0.6;
            }

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);

            db.CreateTable<T_Resultados>();
            db.DeleteAll<T_Resultados>();

            string urlRequest = globalValues.webSite
                + "busqueda.php"
                + "?busqueda="+busqueda
                + "&precio="+precio;

            string responseSelect = client.GetStringAsync(urlRequest).Result;

            if (!responseSelect.Equals("[]"))
            {
                JObject regResponse = JObject.Parse(responseSelect);

                numResultados = regResponse.Count;

                for (int i = 0; i < regResponse.Count; i++)
                {
                    T_Resultados grupo = new T_Resultados();
                    grupo.id_usuario = regResponse["musico" + i]["id_usuario"].ToString();
                    grupo.nombre_musico = regResponse["musico" + i]["nombre_musico"].ToString();
                    grupo.precio = regResponse["musico" + i]["precio"].ToString();
                    grupo.descrprecio= regResponse["musico" + i]["texto"].ToString();
                    grupo.actividad_musico = regResponse["musico" + i]["actividad"].ToString();

                    grupo.descr_musico = regResponse["musico" + i]["descr_musico"].ToString();
                    grupo.distancia= regResponse["musico" + i]["distancia"].ToString();
                    
                    if (!string.IsNullOrEmpty(regResponse["musico" + i]["valoracion"].ToString()))
                    {
                        grupo.valoracion = Convert.ToDouble(regResponse["musico" + i]["valoracion"].ToString());
                    }
                    else
                    {
                        grupo.valoracion = 0;
                    }

                    if (!string.IsNullOrEmpty(regResponse["musico" + i]["nombre_media"].ToString()))
                    {
                        grupo.nombre_media = globalValues.webSite + "Images/user_profile/" + regResponse["musico" + i]["id_path"].ToString() + "/" + regResponse["musico" + i]["nombre_media"].ToString();
                    }
                    else
                    {
                        grupo.nombre_media = "noimage.png";
                    }

                    double longitudeSearch;
                    if(string.IsNullOrEmpty(regResponse["musico" + i]["longitude"].ToString()))
                    {
                        longitudeSearch = Convert.ToDouble("0");
                    }
                    else
                    {
                        longitudeSearch = Convert.ToDouble(regResponse["musico" + i]["longitude"].ToString());
                    }
                        
                    grupo.longitude= longitudeSearch;

                    double latitudeSearch;
                    if (string.IsNullOrEmpty(regResponse["musico" + i]["latitude"].ToString()))
                    {
                        latitudeSearch = Convert.ToDouble("0");
                    }
                    else
                    {
                        latitudeSearch = Convert.ToDouble(regResponse["musico" + i]["latitude"].ToString());
                    }

                    grupo.latitude = latitudeSearch;

                    grupo.isYoutube= regResponse["musico" + i]["yt_video"].ToString();
                    grupo.isFacebook = regResponse["musico" + i]["fb_video"].ToString();

                    if (grupo.isYoutube.Equals("Y"))
                    {
                        grupo.urlYoutube = "https://www.youtube.com/embed/" + regResponse["musico" + i]["url_video"].ToString();
                    }

                    if (grupo.isFacebook.Equals("Y"))
                    {
                        grupo.urlYoutube = regResponse["musico" + i]["url_video"].ToString();
                    }

                    var existe = db.Query<T_Resultados>("SELECT * from RESULTADOS where id_usuario='" + regResponse["musico" + i]["nombre_musico"].ToString() + "'");

                    int existeGrupo = existe.Count();

                    double distance = Location.CalculateDistance(latSearch, longSearch, grupo.latitude, grupo.longitude, DistanceUnits.Kilometers);
                    
                    if (distance < Convert.ToDouble(grupo.distancia))
                    {
                        try
                        {
                            db.Insert(grupo);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Generic error: " + ex.Message);
                        }
                    }
                }
                ObservableCollection<GruposMusicales> grupoMusical = new ObservableCollection<GruposMusicales>();

                var resultado = db.Query<T_Resultados>("SELECT * from RESULTADOS");

                foreach (var s in resultado)
                {
                    GruposMusicales grupoSeleccionado = new GruposMusicales();
                    grupoSeleccionado.id_musico = s.id_usuario;
                    grupoSeleccionado.nombre_musico = s.nombre_musico;
                    grupoSeleccionado.precio = "$"+s.precio;
                    grupoSeleccionado.descrprecio = s.descrprecio;
                    grupoSeleccionado.distancia = s.distancia;
                    grupoSeleccionado.valoracion = s.valoracion;
                    grupoSeleccionado.actividad_musico = s.actividad_musico;
                    if (string.IsNullOrEmpty(grupoSeleccionado.actividad_musico))
                    {
                        hgtActividad = 0;
                    }
                    else
                    {
                        hgtActividad = 20;
                    }
                    grupoSeleccionado.heightActividad = hgtActividad;

                    grupoSeleccionado.descr_musico = s.descr_musico;
                    grupoSeleccionado.nombre_media = s.nombre_media;
                    grupoSeleccionado.isYoutube = s.isYoutube;
                    grupoSeleccionado.isFacebook = s.isFacebook;
                    grupoSeleccionado.urlYoutube = s.urlYoutube;

                    if (s.valoracion < 20)
                    {
                        grupoSeleccionado.star1 = false;
                        grupoSeleccionado.star2 = false;
                        grupoSeleccionado.star3 = false;
                        grupoSeleccionado.star4 = false;
                        grupoSeleccionado.star5 = false;
                    }

                    if (s.valoracion>=20 && s.valoracion < 40)
                    {
                        grupoSeleccionado.star1 = true;
                        grupoSeleccionado.star2 = false;
                        grupoSeleccionado.star3 = false;
                        grupoSeleccionado.star4 = false;
                        grupoSeleccionado.star5 = false;
                    }

                    if (s.valoracion >= 40 && s.valoracion < 60)
                    {
                        grupoSeleccionado.star1 = true;
                        grupoSeleccionado.star2 = true;
                        grupoSeleccionado.star3 = false;
                        grupoSeleccionado.star4 = false;
                        grupoSeleccionado.star5 = false;
                    }

                    if (s.valoracion >= 60 && s.valoracion < 80)
                    {
                        grupoSeleccionado.star1 = true;
                        grupoSeleccionado.star2 = true;
                        grupoSeleccionado.star3 = true;
                        grupoSeleccionado.star4 = false;
                        grupoSeleccionado.star5 = false;
                    }

                    if (s.valoracion >= 80 && s.valoracion < 100)
                    {
                        grupoSeleccionado.star1 = true;
                        grupoSeleccionado.star2 = true;
                        grupoSeleccionado.star3 = true;
                        grupoSeleccionado.star4 = true;
                        grupoSeleccionado.star5 = false;
                    }

                    if (s.valoracion == 100)
                    {
                        grupoSeleccionado.star1 = true;
                        grupoSeleccionado.star2 = true;
                        grupoSeleccionado.star3 = true;
                        grupoSeleccionado.star4 = true;
                        grupoSeleccionado.star5 = true;
                    }

                    grupoSeleccionado.heightImage = heightImage;
                    grupoSeleccionado.lbNombreFntSize = lbNombreFntSize;
                    grupoSeleccionado.actFntSize = actFntSize;
                    grupoSeleccionado.imStarHeight = imStarHeight;
                    grupoSeleccionado.lbPrecioFontsize = lbPrecioFontsize;
                    grupoSeleccionado.darkOpacity = darkOpacity;
                    grupoMusical.Add(grupoSeleccionado);
                }

                resultados = grupoMusical;
            }
        }
    }
}
