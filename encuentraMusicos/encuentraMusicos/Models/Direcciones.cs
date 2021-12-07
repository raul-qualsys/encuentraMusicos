using System;
using System.Collections.Generic;
using System.Text;

namespace encuentraMusicos.Models
{
    public class Direcciones
    {
        public string id_ubicacion { get; set; }
        public string id_usuario { get; set; }
        public string titulo { get; set; }
        public string calleno { get; set; }
        public string postal { get; set; }
        public string colonia { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
        public string pais { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string distancia { get; set; }
        public int nombreFntSize { get; set; }
        public bool detailVisible { get; set; }
    }
}
