using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectDetailPage : ContentPage
    {
        private ProjectsViewModel _projectsViewModel;
        private ProjectDetailViewModel _projectViewModel;
        public ProjectDetailPage(ProjectsViewModel projectsViewModel, Model.Projet projet = null)
        {
            InitializeComponent();
            _projectsViewModel = projectsViewModel;
            _projectViewModel = new ProjectDetailViewModel(projectsViewModel, projet);
            BindingContext = _projectViewModel;
        }
        
        public void UpdateChosenTask(object sender, SelectedItemChangedEventArgs e)
        {
            _projectViewModel.TaskChoisie = e.SelectedItem as Model.Task;
        }
    }
}