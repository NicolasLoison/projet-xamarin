using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class HomeModelView : ViewModelBase
    {
        // private string _nom, _prenom, _mail, _motDePasse;
        private Project _chosenProject;
        private ObservableCollection<Project> _projets = new ObservableCollection<Project>();
        
        static ISettings AppSettings => CrossSettings.Current;
        public static string JsonProjects;
        public static string SavedProjects 
        {
            get => AppSettings.GetValueOrDefault("MySettingKey", JsonProjects);
            set => AppSettings.AddOrUpdateValue("MySettingKey", value);
        }
        
        private bool _clickable;
        
        public bool Clickable
        {
            get => _chosenProject!=null;
            set 
            {
                SetProperty(ref _clickable, value);
                OnPropertyChanged(nameof(_clickable)); // Notify that there was a change on this property
            }
        }


        public ICommand ProfileClick
        {
            get;
            set;
        }
        public ICommand ProjectClick
        {
            get;
            set;
        }

        public ICommand DeleteProjectClick
        {
            get;
            set;
        }
        public ICommand AddProjectClick
        {
            get;
            set;
        }
        public ObservableCollection<Project> Projets
        {
            get => _projets;
            set => SetProperty(ref _projets, value);
        }
        
        public Project ChosenProject
        {
            get => _chosenProject;
            set => SetProperty(ref _chosenProject, value);
        }
        
        public User User
        {
            get => UserInstance.User;
            set => SetProperty(ref UserInstance.User, value);
        }

        public HomeModelView()
        {
            //TODO Commencer un timer et l'assigner à une tache
            FindProjects();
            
            ProfileClick = new Command(ToProfile);
            ProjectClick = new Command(ToProject);
            AddProjectClick = new Command(AddProject);
            DeleteProjectClick = new Command(DeleteProject);
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
                Projets = projects;
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
        
        public async void ToProject()
        {
            if (ChosenProject == null) return;
            try
            {
                ProjectPage projectPage = new ProjectPage(ChosenProject);
                await NavigationService.PushAsync(projectPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
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
        
        public async void DeleteProject()
        {
            if (ChosenProject == null) return;
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this project?", "Yes", "No");
            if (!answer) return;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.DeleteAsync(new Uri(Urls.DELETE_PROJECT.Replace("{projectId}", ChosenProject.Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                Projets.Remove(ChosenProject);
                ChosenProject = null;
            }
        }
    }
}