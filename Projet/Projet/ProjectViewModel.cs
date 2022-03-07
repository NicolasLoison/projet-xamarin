using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Projet.Model;
using Storm.Mvvm;
using Xamarin.Forms;

namespace Projet
{
    public class ProjectViewModel : ViewModelBase
    {
        private Model.Projet _project;
        private Task _chosenTask;
        private ObservableCollection<Task> _tasks;

        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        public Task ChosenTask
        {
            get => _chosenTask;
            set => SetProperty(ref _chosenTask, value);
        }

        public Model.Projet Project
        {
            get => _project;
            set => SetProperty(ref _project, value);
        }

        public ICommand TaskClick { get; set; }

        public ProjectViewModel(Model.Projet project)
        {
            Project = project;
            Tasks = new ObservableCollection<Task>(project.Tasks);
            TaskClick = new Command(ToTask);
        }

        public async void ToTask()
        {
            if (ChosenTask == null) return;
            try
            {
                // TaskPage taskPage = new TaskPage(ChosenTask);
                // await NavigationService.PushAsync(taskPage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        public async void DeleteTask()
        {
            if (ChosenTask == null) return;
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this task?", "Yes", "No");
            if (!answer) return;
            Tasks.Remove(ChosenTask);
            ChosenTask = null;
        }
    }
}