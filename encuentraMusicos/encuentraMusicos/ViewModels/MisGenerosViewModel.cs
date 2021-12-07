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
using encuentraMusicos.Classes;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace encuentraMusicos.ViewModels
{
    public class MisGenerosViewModel
    {
        public ObservableCollection<GenerosMusicales> MisGeneros { get; set; }
        public int numMisGeneros;
        double resolution;
        GlobalValues globalValues=new GlobalValues();
        HttpClient client = new HttpClient();
        public MisGenerosViewModel(string idUsuario)
        {
            ObservableCollection<GenerosMusicales> listGeneros = new ObservableCollection<GenerosMusicales>();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            resolution = width * height;

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySQLite.db3");
            var db = new SQLiteConnection(databasePath);

            string urlRequestMisGeneros = globalValues.webSite
                + "generos_musico.php"
                + "?Usuario=" + idUsuario
                + "&tipoMov=B";

            string responseSelectMisGeneros = client.GetStringAsync(urlRequestMisGeneros).Result;

            if (!responseSelectMisGeneros.Equals("[]"))
            {
                JObject regResponseMisGeneros = JObject.Parse(responseSelectMisGeneros);

                db.CreateTable<T_MisGeneros>();

                db.DeleteAll<T_MisGeneros>();

                for (int k = 0; k < regResponseMisGeneros.Count; k++)
                {
                    var newGeneroItem = new T_MisGeneros();

                    newGeneroItem.code_translate = regResponseMisGeneros["generoMusical" + k]["code_genero"].ToString();
                    newGeneroItem.descripcion = regResponseMisGeneros["generoMusical" + k]["descripcion"].ToString();
                    db.Insert(newGeneroItem);
                }
            }

            db.CreateTable<T_MisGeneros>();
            var resultado = db.Query<T_MisGeneros>("SELECT code_translate, descripcion"
                + " from MIS_GENEROS");

            numMisGeneros = resultado.Count();
            foreach (var s in resultado)
            {
                GenerosMusicales miGenero = new GenerosMusicales();

                miGenero.code = s.code_translate;
                miGenero.descripcion = s.descripcion;

                if (resolution > 2000000)
                {
                    miGenero.fontSz = 18;
                }
                else
                {
                    miGenero.fontSz = 12;
                }
                
                listGeneros.Add(miGenero);
            }
            MisGeneros = listGeneros;
        }
    }
}
