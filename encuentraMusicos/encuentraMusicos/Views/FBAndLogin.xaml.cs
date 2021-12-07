using encuentraMusicos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace encuentraMusicos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FBAndLogin : ContentPage
    {
        public FBAndLogin()
        {
            InitializeComponent();
            BindingContext = new FBLoginViewModel();
        }
    }
}