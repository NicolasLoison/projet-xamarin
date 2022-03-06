using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Accounts;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;

namespace Projet
{
    public class ConnexionViewModel : ViewModelBase
    {
        private string _email, _password;
        private string _errorMessage;
        
        public string ErrorMessage
        {
            get => _errorMessage;
            set 
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(ErrorMessage)); // Notify that there was a change on this property
            }
        }
        
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        
        public ICommand ConnexionClick
        {
            get;
        }
        
        public ICommand InscriptionClick
        {
            get;
        }

        public ConnexionViewModel()
        {
            ErrorMessage = "";
            ConnexionClick = new Command(Login);
            InscriptionClick = new Command(Register);
        }

        public async void Login()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                string jsonData = $@"{{""login"" : ""{Email}"", ""password"" : ""{Password}"",
                                ""client_id"": ""{Urls.CLIENT_ID}"", ""client_secret"": ""{Urls.CLIENT_SECRET}""}}";
                
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(Urls.LOGIN, content);
                Task<string> task = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Response<LoginResponse> data = JsonConvert.DeserializeObject<Response<LoginResponse>>(task.Result);
                    string accessToken = data.Data.AccessToken;
                    string refreshToken = data.Data.RefreshToken;
                    string tokenType = data.Data.TokenType;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                    // On met le token dans le header Authorization
                    response = await client.GetAsync(Urls.USER_PROFILE);
                    if (response.IsSuccessStatusCode)
                    {
                        task = response.Content.ReadAsStringAsync();
                        Response<UserProfileResponse> userData = JsonConvert.DeserializeObject<Response<UserProfileResponse>>(task.Result);
                        string firstName = userData.Data.FirstName;
                        string lastName = userData.Data.LastName;
                        response = await client.GetAsync(new Uri(Urls.LIST_PROJECTS));
                        if (response.IsSuccessStatusCode)
                        {
                            task = response.Content.ReadAsStringAsync();
                            Response<List<Model.Projet>> userProjects =
                                JsonConvert.DeserializeObject<Response<List<Model.Projet>>>(task.Result);
                            List<Model.Projet> projects = userProjects.Data;
                            User user = new User(accessToken, refreshToken, tokenType, firstName, lastName, Email, Password, projects);
                            await NavigationService.PushAsync(new HomePage(user));
                        }
                        else
                        {
                            Console.WriteLine("ERREUR GET LIST PROJECTS");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERREUR GET USER PROFILE");
                    }
                }
                else
                {
                    Console.WriteLine("ERREUR IDENTIFIANTS");
                    ErrorMessage = "Identifiants erronés.";
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                ErrorMessage = "";
            }
        }
        
        public async void Register()
        {
            try
            {
                await NavigationService.PushAsync(new InscriptionPage());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}