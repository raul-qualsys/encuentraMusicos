using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.DataBase.Tables
{
    [Table("CONTACTADOS")]
    public class T_MusicoContactado
    {
        [PrimaryKey]
        public string id_usuario { get; set; }
        [MaxLength(15)]
        public string id_contacto { get; set; }
        [MaxLength(300)]
        public string nombre_usuario { get; set; }
        [MaxLength(15)]
        public DateTime fecha_contacto { get; set; }
        [MaxLength(1)]
        public string estatus { get; set; }
    }
}
