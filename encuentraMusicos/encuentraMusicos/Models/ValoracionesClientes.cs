using System;
using System.Collections.Generic;
using System.Text;

namespace encuentraMusicos.Models
{
    public class ValoracionesClientes
    {
        public string id_musico { get; set; }
        public double valoracion { get; set; }
        public string nombre { get; set; }
        public string mensaje { get; set; }
        public string fecha { get; set; }
        public int heightStar { get; set; }
        public int fontSizeNombre { get; set; }
        public int fontSizeMensaje { get; set; }
        public bool star1IV { get; set; }
        public bool star2IV { get; set; }
        public bool star3IV { get; set; }
        public bool star4IV { get; set; }
        public bool star5IV { get; set; }
        public bool StackIsVisible { get; set; }

    }
}
