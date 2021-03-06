using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace encuentraMusicos.Classes
{
    public class FBProfileAndroid
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public UriImageSource Picture { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
