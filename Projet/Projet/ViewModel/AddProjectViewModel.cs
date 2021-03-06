using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TimeTracker.Dtos.Projects;
using Xamarin.Forms;

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
            Description = "";
            Name = "";
            AddClick = new Command(AddProject);
        }

        public async void AddProject()
        {
            if (Name.Length > 0)
            {
                ErrorMessage = "";
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
                    AddProjectRequest request = new AddProjectRequest(Name, Description);
                    content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                        "application/json");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                    response = await client.PostAsync(new Uri(Urls.ADD_PROJECT), content);
                    if (response.IsSuccessStatusCode)
                    {
                        await NavigationService.PopAsync();
                        HomePage page = Application.Current.MainPage.Navigation.NavigationStack.Last() as HomePage;
                        HomeViewModel viewModel = page.BindingContext as HomeViewModel;
                        viewModel.FindProjects();
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                    }
                }
                else
                {
                    ErrorMessage = "Please enter a name for your project";
                }
            }
        }
    }
}