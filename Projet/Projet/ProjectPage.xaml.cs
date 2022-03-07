using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectPage : ContentPage
    {   
        public ProjectPage(User user, Model.Projet project)
        {
            InitializeComponent();
            BindingContext = new ProjectViewModel(user, project);
        }
    }
}