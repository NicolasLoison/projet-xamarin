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
        private Model.Projet _chosenProject;
        private ObservableCollection<Model.Projet> _projets = new ObservableCollection<Model.Projet>();
        
        static ISettings AppSettings => CrossSettings.Current;
        public static string JsonProjects;
        public static string SavedProjects 
        {
            get => AppSettings.GetValueOrDefault("MySettingKey", JsonProjects);
            set => AppSettings.AddOrUpdateValue("MySettingKey", value);
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
        public ObservableCollection<Model.Projet> Projets
        {
            get => _projets;
            set => SetProperty(ref _projets, value);
        }
        
        public Model.Projet ChosenProject
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
            FindProjects();
            
            ProfileClick = new Command(ToProfile);
            ProjectClick = new Command(ToProject);
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
                Response<List<Model.Projet>> userProjects =
                    JsonConvert.DeserializeObject<Response<List<Model.Projet>>>(task.Result);
                UserInstance.User.Projets = userProjects.Data;
                ObservableCollection<Model.Projet> projects = new ObservableCollection<Model.Projet>(UserInstance.User.Projets);
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
                ProjectPage projectPage = new ProjectPage(UserInstance.User, ChosenProject);
                await NavigationService.PushAsync(projectPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        public async void DeleteTask()
        {
            if (ChosenProject == null) return;
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this project?", "Yes", "No");
            if (!answer) return;
            Projets.Remove(ChosenProject);
            ChosenProject = null;
        }
    }
}