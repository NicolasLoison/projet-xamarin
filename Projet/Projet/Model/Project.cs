using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using TimeTracker.Dtos.Authentications.Credentials;
using TimeTracker.Dtos.Projects;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Projet.Model
{
    public class Project : ViewModelBase
    {
        public int IndexInHome
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
        
        public double GetTotalMinutes()
        {
            double total = 0;
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
        
        public async void ModifyProject(ProjectViewModel projectViewModel)
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Confirm changes", "Do you really want to confirm changes?", "Yes", "No");
            if (answer)
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
                    AddProjectRequest modifyProject =
                        new AddProjectRequest(projectViewModel.EntryName, projectViewModel.EntryDescription);
                    content = new StringContent(JsonConvert.SerializeObject(modifyProject), Encoding.UTF8,
                        "application/json");

                    response =
                        await client.PutAsync(new Uri(Urls.UPDATE_PROJECT.Replace("{projectId}", Id.ToString())),
                            content);
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Modify error: " + response.ReasonPhrase);
                    }
                    else
                    {
                        Name = projectViewModel.EntryName;
                        Description = projectViewModel.EntryDescription;
                        projectViewModel.Project = this;
                        View.Projects[IndexInHome] = this;
                        projectViewModel.SaveName = Name;
                        projectViewModel.SaveDescription = Description;
                    }
                }
                else
                {
                    Name = projectViewModel.SaveName;
                    Description = projectViewModel.SaveDescription;
                    projectViewModel.EntryDescription = Description;
                    projectViewModel.EntryName = Name;
                }

                projectViewModel.Editing = false;
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
                View.Projects.Remove(this);
                for (int i = 0; i < View.Projects.Count; i++)
                {
                    View.Projects[i].IndexInHome = i;
                }
                await NavigationService.PopAsync(false);
            }
        }
    }
}