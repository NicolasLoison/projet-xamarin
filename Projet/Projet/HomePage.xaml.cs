using System;
using System.Diagnostics;
using Projet.Model;
using Storm.Mvvm.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }
        
        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Project project = e.Item as Project;
            if (project != null)
            {
                project.ToPage();
            }
            else
            {
                Debug.WriteLine("Project tapped is null");
            }
        }
    }
}