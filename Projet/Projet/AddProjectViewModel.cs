using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TimeTracker.Dtos.Projects;
using Xamarin.Forms;
using Task = Projet.Model.Task;

namespace Projet
{
    public class AddProjectViewModel : ViewModelBase
    {
        private string _errorMessage, _name, _description;
        public string ErrorMessage
        {
            get => _errorMessage;
            set 
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(_errorMessage)); // Notify that there was a change on this property
            }
        }
        
        public String Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public String Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ICommand AddClick
        {
            get;
            set;
        }
        
        public AddProjectViewModel()
        {
            _description = "";
            _name = "";
            AddClick = new Command(AddProject);
        }
        
        public async void AddProject()
        {
            if (_name.Length > 0)
            {
                _errorMessage = "";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                AddProjectRequest request = new AddProjectRequest();
                request.Description = Description;
                request.Name = Name;
                StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                HttpResponseMessage response = await client.PostAsync(new Uri(Urls.ADD_PROJECT), content);
                if (response.IsSuccessStatusCode)
                {
                    await NavigationService.PushAsync(new HomePage());
                }
            }
            else
            {
                _errorMessage = "Please enter a name for your project";
                Console.WriteLine("PASSSSE");
            }
            
        }
    }
}