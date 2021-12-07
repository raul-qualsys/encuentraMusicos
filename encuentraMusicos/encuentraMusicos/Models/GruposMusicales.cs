using System;
using System.Collections.Generic;
using System.Text;

namespace encuentraMusicos.Models
{
    public class GruposMusicales
    {
        public string id_musico { get; set; }
        public string nombre_musico { get; set; }
        public string precio { get; set; }
        public string descrprecio { get; set; }
        public string distancia { get; set; }
        public double valoracion { get; set; }
        public string actividad_musico { get; set; }
        public string descr_musico { get; set; }
        public string id_path { get; set; }
        public string nombre_media { get; set; } 
        public string isYoutube { get; set; }
        public string isFacebook { get; set; }
        public string urlYoutube { get; set; }
        public bool star1 { get; set; }
        public bool star2 { get; set; }
        public bool star3 { get; set; }
        public bool star4 { get; set; }
        public bool star5 { get; set; }
        public int heightImage { get; set; }
        public int lbNombreFntSize { get; set; }
        public int actFntSize { get; set; }
        public int heightActividad { get; set; }
        public int imStarHeight { get; set; }
        public int lbPrecioFontsize { get; set; }
        public double darkOpacity { get; set; }
    }
}
