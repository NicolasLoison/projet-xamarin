using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Newtonsoft.Json;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace Projet
{
    public class ProjectDetailViewModel : ViewModelBase
    {
        private ProjectsViewModel _projectsViewModel;
        private Model.Projet _projet;
        private Model.Task _taskChoisi;
        
        static ObservableCollection<Model.Task> _tasks = new ObservableCollection<Model.Task>();

        public Model.Task TaskChoisie
        {
            get => _taskChoisi;
            set => SetProperty(ref _taskChoisi, value);
        }
        
        public ObservableCollection<Model.Task> Tasks
        {
            get => _tasks;
        }

        public Model.Projet Projet
        {
            get => _projet;
        }
        public ProjectDetailViewModel(ProjectsViewModel projectsViewModel, Model.Projet projet)
        {
            _projectsViewModel = projectsViewModel;
            _projet = projet;
        }
    }
}