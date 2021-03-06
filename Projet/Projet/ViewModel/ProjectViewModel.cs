using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        private ObservableCollection<Task> _tasks;

        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }
        
    
        public Project Project
        {
            get => _project;
            set
            {
                SetProperty(ref _project, value);
                OnPropertyChanged(nameof(Project));
            } 
        }
        
        private bool _editing;
        public bool Editing
        {
            get => _editing;
            set
            {
                SetProperty(ref _editing, value);
                OnPropertyChanged(nameof(Editing));
            } 
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

        public ICommand HomeClick
        {
            get;
            set;
        }
        
        public ICommand EditClick
        {
            get;
            set;
        }

        public ICommand ConfirmEditClick
        {
            get;
            set;
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

        public string SaveName
        {
            get;
            set;
        }

        public string SaveDescription
        {
            get;
            set;
        }

        private string _entryName, _entryDescription;
        
        public string EntryName
        {
            get => _entryName;
            set
            {
                SetProperty(ref _entryName, value);
                OnPropertyChanged(nameof(EntryName));
            }
        }

        public string EntryDescription
        {
            get => _entryDescription;
            set
            {
                SetProperty(ref _entryDescription, value);
                OnPropertyChanged(nameof(EntryDescription));
            }
        }
        
        public ProjectViewModel(Project project)
        {
            Project = project;
            SaveName = project.Name;
            SaveDescription = project.Description;
            EntryName = project.Name;
            EntryDescription = project.Description;
            
            FindTasks();
            Editing = false;
            HomeClick = new Command(ToHome);
            EditClick = new Command(TriggerEdit);
            ConfirmEditClick = new Command(ConfirmEdit);
            DeleteClick = new Command(DeleteProject);
            AddTaskClick = new Command(AddTask);
            GraphClick = new Command(GraphTask);
            TimerInstance.Timer.ProjectViewModel = this;
            TimerValue = TimerInstance.Timer.GetCurrentTotalTime().ToString("hh':'mm':'ss");
        }
        
        public async void FindTasks()
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                response =
                    await client.GetAsync(new Uri(Urls.LIST_TASKS.Replace("{projectId}", Project.Id.ToString())));
                if (response.IsSuccessStatusCode)
                {
                    task = response.Content.ReadAsStringAsync();
                    Response<List<Task>> projectTasks =
                        JsonConvert.DeserializeObject<Response<List<Task>>>(task.Result);
                    ObservableCollection<Task> tasks = new ObservableCollection<Task>(projectTasks.Data);
                    Tasks = tasks;
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        tasks[i].View = this;
                        tasks[i].IndexInProject = i;
                        tasks[i].SetTotalMinutes();
                    }
                }
            }
        }

        public void TriggerEdit()
        {
            if (Editing)
            {
                Project.ModifyProject(this);
            }
            else
            {
                Editing = true;
            }
        }

        public void ConfirmEdit()
        {
            Project.ModifyProject(this);
        }
        
        public void DeleteProject()
        {
            Project.DeleteProject();
        }
        
        public async void ToHome()
        {
            try
            {
                while (!(Application.Current.MainPage.Navigation.NavigationStack.Last() is HomePage))
                {
                    await NavigationService.PopAsync(false);
                }
                HomePage page = Application.Current.MainPage.Navigation.NavigationStack.Last() as HomePage;
                HomeViewModel viewModel = page.BindingContext as HomeViewModel;
                viewModel.FindProjects();
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