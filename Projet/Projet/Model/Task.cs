using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using Storm.Mvvm;
using TimeTracker.Dtos;
using Xamarin.Forms;

namespace Projet.Model
{
    public class Task : ViewModelBase
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

        public List<Timer> Times
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
        
        public ProjectViewModel View
        {
            get;
            set;
        }

        public Task(int id, string name)
        {
            Id = id;
            Name = name;
            Times = new List<Timer>();
            PageClick = new Command(ToPage);
            DeleteClick = new Command(DeleteProject);
        }

        public int GetTotalHours()
        {
            int total = 0;
            foreach (Timer t in Times)
            {
                total += t.GetTotalHours();
            }

            return total;
        }
        
        public int GetTotalMinutes()
        {
            int total = 0;
            foreach (Timer t in Times)
            {
                total += t.GetTotalMinutes();
            }

            return total;
        }
        
        public async void ToPage()
        {
            try
            {
                await NavigationService.PushAsync(new TaskPage(this));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public async void DeleteProject()
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this task?", "Yes", "No");
            if (!answer) return;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.DeleteAsync(new Uri(Urls.DELETE_TASK.Replace("{projectId}", View.Project.Id.ToString()).Replace("{taskId}", Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                View.Tasks.Remove(this);
            }
        }
    }
}