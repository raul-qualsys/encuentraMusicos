using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.DataBase.Tables
{
    [Table("RESULTADOS")]
    public class T_Resultados
    {
        [PrimaryKey]
        public string id_usuario { get; set; }
        [MaxLength(300)]
        public string nombre_musico { get; set; }
        [MaxLength(10)]
        public string precio { get; set; }
        [MaxLength(300)]
        public string descrprecio { get; set; }
        [MaxLength(500)]
        public string descr_musico { get; set; }
        [MaxLength(50)]
        public string actividad_musico { get; set; }
        [MaxLength(500)]
        public string nombre_media { get; set; }
        [MaxLength(1)]
        public string isYoutube { get; set; }
        [MaxLength(1)]
        public string isFacebook { get; set; }
        [MaxLength(500)]
        public string urlYoutube { get; set; }
        [MaxLength(100)]
        public double longitude { get; set; }
        [MaxLength(100)]
        public double latitude { get; set; }
        public string distancia { get; set; }
        public double valoracion { get; set; }
    }
}
