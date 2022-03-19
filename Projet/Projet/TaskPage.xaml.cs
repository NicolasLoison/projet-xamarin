using System;
using Projet.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : ContentPage
    {
        private Task _task;
        
        public TaskPage(Task task)
        {
            _task = task;
            InitializeComponent();
            BindingContext = new TaskViewModel(task);
        }
    }
}