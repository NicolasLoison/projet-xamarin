using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using TimeTracker.Dtos.Projects;
using Xamarin.Forms;
using Task = Projet.Model.Task;

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

        private Task _task;

        public Task Task
        {
            get => _task;
            set
            {
                SetProperty(ref _task, value);
                OnPropertyChanged(nameof(Task));
            }
        }

        private bool _editing;

        public bool Editing
        {
            get => _editing;
            set
            {
                SetProperty(ref _editing, value);
                OnPropertyChanged(nameof(Editing));
            }
        }

        private string _timerValue;

        public string TimerValue
        {
            get => _timerValue;
            set
            {
                SetProperty(ref _timerValue, value);
                OnPropertyChanged(TimerValue);
            }
        }

        public ICommand HomeClick { get; set; }

        public ICommand EditClick { get; set; }

        public ICommand ConfirmEditClick { get; set; }

        public ICommand DeleteClick { get; set; }

        public ICommand TimerClick { get; set; }

        public ICommand AddTimerClick { get; set; }

        public string SaveName { get; set; }

        private string _entryName;

        public string EntryName
        {
            get => _entryName;
            set
            {
                SetProperty(ref _entryName, value);
                OnPropertyChanged(nameof(EntryName));
            }
        }

        public TaskViewModel(Task task)
        {
            Task = task;
            SaveName = task.Name;
            EntryName = task.Name;
            TimerColor = TimerColorUpdater;
            Editing = false;
            HomeClick = new Command(ToHome);
            EditClick = new Command(TriggerEdit);
            ConfirmEditClick = new Command(ConfirmEdit);
            DeleteClick = new Command(DeleteTask);

            AddTimerClick = new Command(AddTimer);
            TimerClick = new Command(TriggerTimer);
            Timers = new ObservableCollection<Timer>(Task.Times);
            Clickable = TimerInstance.Timer.GetCurrentTotalTime().Milliseconds > 0;
            foreach (Timer t in Timers)
            {
                t.View = this;
            }

            // NavigationService.OnPop();
            TimerInstance.Timer.TaskViewModel = this;
            TimerValue = TimerInstance.Timer.GetCurrentTotalTime().ToString("hh':'mm':'ss");

        }

        public void TriggerEdit()
        {
            if (Editing)
            {
                Task.ModifyTask(this);
            }
            else
            {
                Editing = true;
            }
        }

        public void ConfirmEdit()
        {
            Task.ModifyTask(this);
        }

        public void DeleteTask()
        {
            Task.DeleteTask();
        }

        public async void ToHome()
        {
            try
            {
                while (!(Application.Current.MainPage.Navigation.NavigationStack.Last() is HomePage))
                {
                    await NavigationService.PopAsync(false);
                }

                HomePage page = Application.Current.MainPage.Navigation.NavigationStack.Last() as HomePage;
                HomeViewModel viewModel = page.BindingContext as HomeViewModel;
                viewModel.FindProjects();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                Clickable = true;
            }
        }

        public async void AddTimer()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            RefreshRequest refreshRequest =
                new RefreshRequest(UserInstance.User.RefreshToken, Urls.CLIENT_ID, Urls.CLIENT_SECRET);
            StringContent content = new StringContent(JsonConvert.SerializeObject(refreshRequest), Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri(Urls.REFRESH_TOKEN), content);
            if (response.IsSuccessStatusCode)
            {
                Task<string> task = response.Content.ReadAsStringAsync();
                Response<LoginResponse> r =
                    JsonConvert.DeserializeObject<Response<LoginResponse>>(task.Result);
                UserInstance.User.AccessToken = r.Data.AccessToken;
                UserInstance.User.RefreshToken = r.Data.RefreshToken;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                AddTimeRequest request = new AddTimeRequest(TimerInstance.Timer.StartTime, TimerInstance.Timer.EndTime);
                if (TimerInstance.Timer.Started)
                {
                    request.EndTime = DateTime.Now;
                }

                content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                response = await client.PostAsync(new Uri(
                        Urls.ADD_TIME.Replace("{projectId}", Task.View.Project.Id.ToString())
                            .Replace("{taskId}", Task.Id.ToString())),
                    content);
                if (response.IsSuccessStatusCode)
                {
                    string task2 = await response.Content.ReadAsStringAsync();
                    Response<AddTimeResponse> data = JsonConvert.DeserializeObject<Response<AddTimeResponse>>(task2);
                    int Id = data.Data.Id;
                    DateTime Start = data.Data.StartTime;
                    DateTime End = data.Data.EndTime;
                    Timer t = new Timer(Id, Start, End);
                    t.View = this;
                    Timers.Add(t);
                }
                else
                {
                    Debug.WriteLine("Not success add timer: " + response.ReasonPhrase);
                }
            }
        }
    }
}