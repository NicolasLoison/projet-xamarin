using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Task = System.Threading.Tasks.Task;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectPage : ContentPage
    {
        private Project _project;
        
        public ProjectPage(Project project)
        {
            _project = project;
            InitializeComponent();
            BindingContext = new ProjectViewModel(_project);
        }
        
        public async void Refresh(Object sender, EventArgs e)
        {
            await Task.Delay(500);
            MyRefreshView.IsRefreshing = false;
            BindingContext = new ProjectViewModel(_project);
        }
    }
}