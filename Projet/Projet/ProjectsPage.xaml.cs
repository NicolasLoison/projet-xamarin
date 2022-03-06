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
    public partial class ProjectsPage : ContentPage
    {
        private ProjectsViewModel _viewModel;
        public ProjectsPage()
        {
            InitializeComponent();
            _viewModel = new ProjectsViewModel();
            BindingContext = _viewModel;
        }
        
        public void UpdateChosenTodo(object sender, SelectedItemChangedEventArgs e)
        {
            _viewModel.ProjetChoisi = e.SelectedItem as Model.Projet;
        }
    }
}