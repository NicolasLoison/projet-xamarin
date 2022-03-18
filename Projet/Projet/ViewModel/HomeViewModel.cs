﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Projet.Model;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;
using Timer = System.Timers.Timer;

namespace Projet
{
    public class HomeViewModel : ViewModelBase
    {
        private ObservableCollection<Project> _projets = new ObservableCollection<Project>();
        
        static ISettings AppSettings => CrossSettings.Current;
        public static string JsonProjects;
        public static string SavedProjects 
        {
            get => AppSettings.GetValueOrDefault("MySettingKey", JsonProjects);
            set => AppSettings.AddOrUpdateValue("MySettingKey", value);
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
                OnPropertyChanged(nameof(TimerColor));
            } 
        }
        
        public ICommand ProfileClick
        {
            get;
            set;
        }

        public ICommand GraphClick
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
            FindProjects();
            TimerColor = TimerColorUpdater;
            ProfileClick = new Command(ToProfile);
            AddProjectClick = new Command(AddProject);
            TimerClick = new Command(TriggerTimer);
            GraphClick = new Command(GraphProject);
            TimerInstance.Timer.HomeViewModel = this;
            TimerValue = TimerInstance.Timer.GetCurrentTotalTime().ToString("hh':'mm':'ss'.'ff");
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
        
        public async void GraphProject()
        {
            try
            {
                ProjectGraphPage projectGraphPage = new ProjectGraphPage();
                await NavigationService.PushAsync(projectGraphPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        public void TriggerTimer()
        {
            // Stop
            if (TimerInstance.Timer.Started)
            {
                TimerInstance.Timer.Stop();
                TimerColor = TimerColorUpdater;
                Debug.WriteLine("Current: " + TimerInstance.Timer.CurrentTime);
                TimerValue = TimerInstance.Timer.GetTotalTime().ToString("hh':'mm':'ss'.'ff");
            }
            // Start
            else
            {
                TimerInstance.Timer.Start();
                TimerColor = TimerColorUpdater;
            }
        }
    }
}