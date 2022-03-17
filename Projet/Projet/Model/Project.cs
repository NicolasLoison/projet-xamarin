using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using Storm.Mvvm;
using TimeTracker.Dtos;
using Xamarin.Forms;

namespace Projet.Model
{
    public class Project : ViewModelBase
    {
        public int Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int TotalSeconds
        {
            get;
            set;
        }

        public List<Task> Tasks
        {
            get;
            set;
        }

        public ICommand PageClick
        {
            get;
            set;
        }
        
        public ICommand DeleteClick
        {
            get;
            set;
        }

        public HomeViewModel View
        {
            get;
            set;
        }
        
        public Project(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            TotalSeconds = 0;
            Tasks = new List<Task>();
            PageClick = new Command(ToPage);
            DeleteClick = new Command(DeleteProject);
        }

        public int GetTotalHours()
        {
            int total = 0;
            foreach (Task t in Tasks)
            {
                total += t.GetTotalHours();
            }

            return total;
        }
        
        public int GetTotalMinutes()
        {
            int total = 0;
            foreach (Task t in Tasks)
            {
                total += t.GetTotalMinutes();
            }

            return total;
        }

        
        public async void ToPage()
        {
            try
            {
                await NavigationService.PushAsync(new ProjectPage(this));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public async void DeleteProject()
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this project?", "Yes", "No");
            if (!answer) return;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.DeleteAsync(new Uri(Urls.DELETE_PROJECT.Replace("{projectId}", Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                await NavigationService.PushAsync(new HomePage());
            }
        }
    }
}