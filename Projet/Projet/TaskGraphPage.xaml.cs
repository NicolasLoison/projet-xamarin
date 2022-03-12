using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskGraphPage : ContentPage
    {
        public TaskGraphPage(Project project)
        {
            InitializeComponent();
            BindingContext = new TaskGraphViewModel(project);
        }
    }
}