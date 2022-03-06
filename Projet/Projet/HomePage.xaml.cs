using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage(User user)
        {
            InitializeComponent();
            BindingContext = new HomeModelView(user);
        }
    }
}