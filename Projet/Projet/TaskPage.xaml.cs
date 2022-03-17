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
        
                
        public async void Refresh(Object sender, EventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(100);
            MyRefreshView.IsRefreshing = false;
            BindingContext = new TaskViewModel(_task);
        }
    }
}