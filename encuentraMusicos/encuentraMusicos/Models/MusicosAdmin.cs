using System;
using System.Collections.Generic;
using System.Text;

namespace encuentraMusicos.Models
{
    public class MusicosAdmin
    {
        public string id_usuario { get; set; }
        public string nombre_musico { get; set; }
        public string descripcion { get; set; }
        public double valoracion { get; set; }
        public string tipo_musico { get; set; }
        public DateTime fecha_registro { get; set; }
        public string is_active { get; set; }
        public string isActiveDescr { get; set; }
        public string id_path { get; set; }
        public string nombre_media { get; set; }
        public int fntSize { get; set; }
        public bool star1 { get; set; }
        public bool star2 { get; set; }
        public bool star3 { get; set; }
        public bool star4 { get; set; }
        public bool star5 { get; set; }
        public int heightImage { get; set; }
        public int lbNombreFntSize { get; set; }
        public int imStarHeight { get; set; }
        public double darkOpacity { get; set; }
    }
}
