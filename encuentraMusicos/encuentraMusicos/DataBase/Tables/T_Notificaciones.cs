using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.DataBase.Tables
{
    [Table("NOTIFICACIONES")]
    public class T_Notificaciones
    {
        [PrimaryKey]
        public string id_notificacion { get; set; }
        [MaxLength(15)]
        public DateTime fecha_notificacion { get; set; }
    }
}
