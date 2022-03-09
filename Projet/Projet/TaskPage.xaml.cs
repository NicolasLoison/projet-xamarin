using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : ContentPage
    {
        public TaskPage(Task task)
        {
            InitializeComponent();
            BindingContext = new TaskViewModel(task);
        }
    }
}