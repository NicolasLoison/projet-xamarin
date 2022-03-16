using System;
using System.Collections.ObjectModel;
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
        
        public TaskViewModel(Task task)
        {
            Task = task;
            TimerClick = new Command(AddTimer);
            Timers = new ObservableCollection<Timer>(Task.Times);
            foreach (Timer t in Timers)
            {
                t.View = this;
            }

            if (TimerInstance.Timer != null)
            {
                Console.WriteLine(TimerInstance.Timer.Started);
                if (TimerInstance.Timer.Started)
                {
                    Clickable = true;
                }
                else
                {
                    Clickable = false;
                }
                
            }
            else
            {
                Clickable = false;
            }
        }
        
        public async void AddTimer()
        {
            if (TimerInstance.Timer.Started)
            {
                Clickable = false;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                AddTimeRequest request = new AddTimeRequest(TimerInstance.Timer.StartTime, TimerInstance.Timer.EndTime);
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
            }
        }
    }
}