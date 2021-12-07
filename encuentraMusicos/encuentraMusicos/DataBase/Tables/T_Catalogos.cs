using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.DataBase.Tables
{
    [Table("CATALOGOS")]
    public class T_Catalogos
    {
        [PrimaryKey]
        public string code_translate { get; set; }
        [MaxLength(150)]
        public string tipo_translate { get; set; }
        [MaxLength(500)]
        public string descripcion { get; set; }
    }
}
