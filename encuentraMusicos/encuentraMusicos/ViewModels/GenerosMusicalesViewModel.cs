using encuentraMusicos.DataBase.Tables;
using encuentraMusicos.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace encuentraMusicos.ViewModels
{
    public class GenerosMusicalesViewModel
    {
        public ObservableCollection<GenerosMusicales> Generos { get; set; }
        public int numGeneros;
        double resolution;
        public GenerosMusicalesViewModel(string busqueda)
        {
            ObservableCollection<GenerosMusicales> listGeneros = new ObservableCollection<GenerosMusicales>();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);

            db.CreateTable<T_Catalogos>();
            var resultado = db.Query<T_Catalogos>("SELECT code_translate, descripcion"
                + " from CATALOGOS where tipo_translate='GENERO_MUSICAL'"
                + " and upper(descripcion) like '%"+busqueda+"%'"
                + " and code_translate not in (select code_translate from MIS_GENEROS)");

            numGeneros = resultado.Count();
            foreach (var s in resultado)
            {
                GenerosMusicales genero = new GenerosMusicales();

                genero.code = s.code_translate;
                genero.descripcion = s.descripcion;

                if (resolution > 2000000)
                {
                    genero.fontSz = 18;
                }
                else
                {
                    genero.fontSz = 12;
                }

                if (!string.IsNullOrEmpty(busqueda))
                {
                    listGeneros.Add(genero);
                }
            }
            Generos = listGeneros;
        }
    }
}
