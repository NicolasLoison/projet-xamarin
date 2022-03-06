using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfilePage : ContentPage
    {
        public UserProfilePage(User user)
        {
            InitializeComponent();
            BindingContext = new UserProfileViewModel(user);
        }
    }
}