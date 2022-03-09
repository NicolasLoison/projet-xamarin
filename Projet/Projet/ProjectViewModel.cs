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

        public Task ChosenTask
        {
            get => _chosenTask;
            set => SetProperty(ref _chosenTask, value);
        }

        public Project Project
        {
            get => _project;
            set => SetProperty(ref _project, value);
        }

        public ICommand TaskClick { get; set; }
        public ICommand AddTaskClick { get; set; }
        public ICommand DeleteTaskClick { get; set; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            FindTasks();
            TaskClick = new Command(ToTask);
            AddTaskClick = new Command(AddTask);
            DeleteTaskClick = new Command(DeleteTask);
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
            }
        }

        public async void ToTask()
        {
            if (ChosenTask == null) return;
            try
            {
                TaskPage taskPage = new TaskPage(ChosenTask);
                await NavigationService.PushAsync(taskPage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
        
        public async void DeleteTask()
        {
            if (ChosenTask == null) return;
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this task?", "Yes", "No");
            if (!answer) return;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.DeleteAsync(new Uri(Urls.DELETE_TASK.Replace("{projectId}", Project.Id.ToString()).Replace("{taskId}", ChosenTask.Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                Tasks.Remove(ChosenTask);
                ChosenTask = null;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase);
            }
        }
    }
}