using System.Collections.Generic;
using System.Collections.ObjectModel;
using Projet.Model;
using Storm.Mvvm;

namespace Projet
{
    public class ProjectViewModel : ViewModelBase
    {
        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }
        public Model.Projet Project
        {
            get;
            set;
        }
        public ProjectViewModel(Model.Projet project)
        {
            Project = project;
            Tasks = new ObservableCollection<Task>(project.Tasks);
        }
    }
}