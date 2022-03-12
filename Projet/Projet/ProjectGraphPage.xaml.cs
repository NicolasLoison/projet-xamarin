using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectGraphPage : ContentPage
    {
        public ProjectGraphPage()
        {
            InitializeComponent();
            BindingContext = new ProjectGraphViewModel();
        }
    }
}