using encuentraMusicos.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using encuentraMusicos.Models;
using encuentraMusicos.Services;
using Xamarin.Essentials;

namespace encuentraMusicos
{
    public partial class App : Application
    {
        string userId;

        public App()
        {
            InitializeComponent();

            MainPage = new Loading();
        }

        protected override async void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
