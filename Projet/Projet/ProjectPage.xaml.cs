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
    public partial class ProjectPage : ContentPage
    {   
        public ProjectPage(Model.Projet project)
        {
            InitializeComponent();
            BindingContext = new ProjectViewModel(project);
        }
    }
}