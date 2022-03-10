using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;

namespace Projet
{
    public class TaskViewModel : ViewModelBase
    {
        private ObservableCollection<Timer> _timers;
        private Timer _chosenTimer;
        public ObservableCollection<Timer> Timers
        {
            get => _timers;
            set => SetProperty(ref _timers, value);
        }
        
        public Task Task
        {
            get;
            set;
        }
        
        public Timer ChosenTimer
        {
            get => _chosenTimer;
            set => SetProperty(ref _chosenTimer, value);
        }
        
        public TaskViewModel(Task task)
        {
            Task = task;
            Timers = new ObservableCollection<Timer>(Task.Times);
        }
    }
}