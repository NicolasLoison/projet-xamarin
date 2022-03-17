using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;
using Task = Projet.Model.Task;

namespace Projet
{
    public class ProjectViewModel : ViewModelBase
    {
        private Project _project;
        private Task _chosenTask;
        private ObservableCollection<Task> _tasks;

        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }
        

        public Project Project
        {
            get => _project;
            set => SetProperty(ref _project, value);
        }

        public ICommand DeleteClick
        {
            get;
            set;
        }
        public ICommand AddTaskClick { get; set; }

        public ICommand GraphClick
        {
            get;
            set;
        }
        
        public ProjectViewModel(Project project)
        {
            Project = project;
            FindTasks();
            DeleteClick = new Command(DeleteProject);
            AddTaskClick = new Command(AddTask);
            GraphClick = new Command(GraphTask);
        }
        
        public async void FindTasks()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.GetAsync(new Uri(Urls.LIST_TASKS.Replace("{projectId}", Project.Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                Task<string> task = response.Content.ReadAsStringAsync();
                Response<List<Task>> projectTasks =
                    JsonConvert.DeserializeObject<Response<List<Task>>>(task.Result);
                ObservableCollection<Task> tasks = new ObservableCollection<Task>(projectTasks.Data);
                Tasks = tasks;
                foreach (Task t in tasks)
                {
                    t.View = this;
                }
            }
        }

        public void DeleteProject()
        {
            Project.DeleteProject();
        }
        
        public async void AddTask()
        {
            try
            {
                AddTaskPage addTaskPage = new AddTaskPage(Project);
                await NavigationService.PushAsync(addTaskPage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public async void GraphTask()
        {
            try
            {
                TaskGraphPage taskGraphPage = new TaskGraphPage(Project);
                await NavigationService.PushAsync(taskGraphPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}