using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.DataBase.Tables
{
    [Table("MIS_GENEROS")]
    public class T_MisGeneros
    {
        [PrimaryKey]
        public string code_translate { get; set; }
        [MaxLength(500)]
        public string descripcion { get; set; }
    }
}
