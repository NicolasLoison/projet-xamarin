using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using TimeTracker.Dtos.Projects;
using Xamarin.Forms;

namespace Projet
{
    public class TaskViewModel : ViewModelBase
    {
        private ObservableCollection<Timer> _timers;
        public ObservableCollection<Timer> Timers
        {
            get => _timers;
            set => SetProperty(ref _timers, value);
        }

        public bool _clickable;
        
        public bool Clickable
        {
            get => _clickable;
            set
            {
                SetProperty(ref _clickable, value);
                OnPropertyChanged(nameof(Clickable));
            } 
        }
        
        private string TimerColorUpdater
        {
            get
            {
                if (TimerInstance.Timer.Started)
                {
                    return "#EB5809";
                }
                return "#FF9ACD32";
            }
        }

        private string _timerColor;
        
        public string TimerColor
        {
            get => _timerColor;
            set
            {
                SetProperty(ref _timerColor, value);
                // OnPropertyChanged(nameof(TimerColor));
                OnPropertyChanged(nameof(TimerInstance.Timer.Started));
            } 
        }
        
        public Task Task
        {
            get;
            set;
        }
        
        public ICommand TimerClick
        {
            get;
            set;
        }
        
        public ICommand AddTimerClick
        {
            get;
            set;
        }
        
        public TaskViewModel(Task task)
        {
            Task = task;
            TimerColor = TimerColorUpdater;
            AddTimerClick = new Command(AddTimer);
            TimerClick = new Command(TriggerTimer);
            Timers = new ObservableCollection<Timer>(Task.Times);
            foreach (Timer t in Timers)
            {
                t.View = this;
            }
        }
        
        public void TriggerTimer()
        {
            // Stop
            if (TimerInstance.Timer.Started)
            {
                TimerInstance.Timer.Stop();
                TimerColor = TimerColorUpdater;
            }
            // Start
            else
            {
                TimerInstance.Timer.Start();
                TimerColor = TimerColorUpdater;
            }
        }
        
        public async void AddTimer()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            AddTimeRequest request = new AddTimeRequest(TimerInstance.Timer.StartTime, TimerInstance.Timer.EndTime);
            if (TimerInstance.Timer.Started)
            {
                request.EndTime = DateTime.Now;
            }
            StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri(
                    Urls.ADD_TIME.
                        Replace("{projectId}", Task.View.Project.Id.ToString()).
                        Replace("{taskId}", Task.Id.ToString())),
                content);
            if (response.IsSuccessStatusCode)
            {
                string task = await response.Content.ReadAsStringAsync();
                Response<AddTimeResponse> data = JsonConvert.DeserializeObject<Response<AddTimeResponse>>(task);
                int Id = data.Data.Id;
                DateTime Start = data.Data.StartTime;
                DateTime End = data.Data.EndTime;
                Timer t = new Timer(Id, Start, End);
                t.View = this;
                Timers.Add(t);
            }
            else
            {
                Debug.WriteLine("Not success");
                Debug.WriteLine(response.ReasonPhrase);
            }
        }
    }
}