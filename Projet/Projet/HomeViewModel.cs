using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;

namespace Projet
{
    public class HomeViewModel : ViewModelBase
    {
        private ObservableCollection<Project> _projets = new ObservableCollection<Project>();
        private string _timerColor = "#FF9ACD32";
        
        static ISettings AppSettings => CrossSettings.Current;
        public static string JsonProjects;
        public static string SavedProjects 
        {
            get => AppSettings.GetValueOrDefault("MySettingKey", JsonProjects);
            set => AppSettings.AddOrUpdateValue("MySettingKey", value);
        }

        public string TimerColor
        {
            get => _timerColor;
            set
            {
                SetProperty(ref _timerColor, value);
                OnPropertyChanged(nameof(TimerColor));
            } 
        }
        public ICommand ProfileClick
        {
            get;
            set;
        }
        public ICommand TimerClick
        {
            get;
            set;
        }
        
        public ICommand AddProjectClick
        {
            get;
            set;
        }
        public ObservableCollection<Project> Projects
        {
            get => _projets;
            set
            {
                SetProperty(ref _projets, value);
                OnPropertyChanged(nameof(_projets));
            } 
        }
        
        public User User
        {
            get => UserInstance.User;
        }

        public HomeViewModel()
        {
            //TODO Commencer un timer et l'assigner à une tache
            FindProjects();
            
            ProfileClick = new Command(ToProfile);
            AddProjectClick = new Command(AddProject);
            TimerClick = new Command(TriggerTimer);
            // Device.StartTimer (new TimeSpan (0, 0, 1), () =>
            // {
            //     // do something every 60 seconds
            //     Device.BeginInvokeOnMainThread (() => 
            //     {
            //         if (TimerInstance.Timer != null)
            //         {
            //             Console.WriteLine(TimerInstance.Timer.GetTotalTime());
            //         }
            //     });
            //     return true; // runs again, or false to stop
            // });
        }

        public async void FindProjects()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(User.TokenType, User.AccessToken);
            HttpResponseMessage response = await client.GetAsync(new Uri(Urls.LIST_PROJECTS));
            if (response.IsSuccessStatusCode)
            {
                Task<string> task = response.Content.ReadAsStringAsync();
                Response<List<Project>> userProjects =
                    JsonConvert.DeserializeObject<Response<List<Project>>>(task.Result);
                UserInstance.User.Projets = userProjects.Data;
                ObservableCollection<Project> projects = new ObservableCollection<Project>(UserInstance.User.Projets);
                Projects = projects;
                foreach (Project p in Projects)
                {
                    p.View = this;
                }
            }
        }
        
        public async void ToProfile()
        {
            try
            {
                await NavigationService.PushAsync(new UserProfilePage());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        
        public async void AddProject()
        {
            try
            {
                AddProjectPage addProjectPage = new AddProjectPage();
                await NavigationService.PushAsync(addProjectPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        public void TriggerTimer()
        {
            if (TimerInstance.Timer != null)
            {
                // Stop
                if (TimerInstance.Timer.Started)
                {
                    TimerColor = "#FF9ACD32";
                    TimerInstance.Timer.Stop();
                }
                // Start
                else
                {
                    TimerColor = "#EB5809";
                    TimerInstance.Timer = new TimerInstance();
                }
            }
            // Start
            else
            {
                TimerColor = "#EB5809";
                TimerInstance.Timer = new TimerInstance();
            }
        }
    }
}