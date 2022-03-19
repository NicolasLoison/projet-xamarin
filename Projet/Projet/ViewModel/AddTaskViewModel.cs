using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Projects;
using Xamarin.Forms;

namespace Projet
{
    public class AddTaskViewModel : ViewModelBase
    {
        private string _name, _errorMessage;
        private Project _project;
        
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(_errorMessage));
            }
        }

        public ICommand AddClick
        {
            get;
            set;
        }
        
        public AddTaskViewModel(Project project)
        {
            _project = project;
            _errorMessage = "";
            _name = "";
            AddClick = new Command(AddTask);
        }
        
        public async void AddTask()
        {
            if (Name.Length > 0)
            {
                ErrorMessage = "";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                AddTaskRequest request = new AddTaskRequest(Name);
                StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                HttpResponseMessage response = await client.PostAsync(new Uri(Urls.CREATE_TASK.Replace("{projectId}", _project.Id.ToString())), content);
                if (response.IsSuccessStatusCode)
                {
                    await NavigationService.PopAsync();
                    ProjectPage page = Application.Current.MainPage.Navigation.NavigationStack.Last() as ProjectPage;
                    ProjectViewModel projectViewModel = page.BindingContext as ProjectViewModel;
                    projectViewModel.FindTasks();
                }
            }
            else
            {
                ErrorMessage = "Please enter a name for your task";
            }
        }
    }
}