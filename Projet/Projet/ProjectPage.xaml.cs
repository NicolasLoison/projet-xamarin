using System;
using System.Diagnostics;
using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Task = System.Threading.Tasks.Task;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectPage
    {
        private Project _project;
        
        public ProjectPage(Project project)
        {
            _project = project;
            InitializeComponent();
            BindingContext = new ProjectViewModel(_project);
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Model.Task task = e.Item as Model.Task;
            if (task != null)
            {
                task.ToPage();
            }
            else
            {
                Debug.WriteLine("Task tapped is null");
            }
        }
    }
}