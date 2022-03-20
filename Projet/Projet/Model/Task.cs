using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Projects;
using Xamarin.Forms;

namespace Projet.Model
{
    public class Task : ViewModelBase
    {
        public int IndexInProject
        {
            get;
            set;
        }
        
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
        
        public async void ModifyTask(TaskViewModel taskViewModel)
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Confirm changes", "Do you really want to confirm changes?", "Yes", "No");
            if (answer)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                AddTaskRequest modifyTask = new AddTaskRequest(taskViewModel.EntryName);
                StringContent content = new StringContent(JsonConvert.SerializeObject(modifyTask), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(new Uri(Urls.UPDATE_TASK
                    .Replace("{projectId}", View.Project.Id.ToString())
                    .Replace("{taskId}", Id.ToString())), content);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Modify task error: " + response.ReasonPhrase);
                }
                else
                {
                    Name = taskViewModel.EntryName;
                    taskViewModel.Task = this;
                    View.Tasks[IndexInProject] = this;
                    taskViewModel.SaveName = Name;
                }
            }
            else
            {
                Name = taskViewModel.SaveName;
                taskViewModel.EntryName = Name;
            }
            taskViewModel.Editing = false;
        }
        
        public async void DeleteTask()
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this task?", "Yes", "No");
            if (!answer) return;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.DeleteAsync(new Uri(Urls.DELETE_TASK
                .Replace("{projectId}", View.Project.Id.ToString())
                .Replace("{taskId}", Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                View.Tasks.Remove(this);
                for (int i = 0; i < View.Tasks.Count; i++)
                {
                    View.Tasks[i].IndexInProject = i;
                }
                await NavigationService.PopAsync(false);
            }
        }
    }
}