using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.DataBase.Tables
{
    [Table("USUARIOS")]
    class T_Registro
    {
        [PrimaryKey]
        public string Id { get; set; }
        [MaxLength(255)]
        public string userb64 { get; set; }
        [MaxLength(255)]
        public string Nombre { get; set; }
        [MaxLength(455)]
        public string Image { get; set; }
        [MaxLength(255)]
        public string isActive { get; set; }
        [MaxLength(1)]
        public string isSynchronized { get; set; }
        [MaxLength(1)]
        public string tpMusico { get; set; }
    }
}
