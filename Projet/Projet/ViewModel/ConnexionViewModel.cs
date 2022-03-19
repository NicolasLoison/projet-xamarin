using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Accounts;
using TimeTracker.Dtos.Authentications;
using TimeTracker.Dtos.Authentications.Credentials;
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

        // public void R()
        // {
        //     Test.Login();
        // }
        public async void Login()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.Timeout = TimeSpan.FromSeconds(1);
            LoginWithCredentialsRequest loginRequest = new LoginWithCredentialsRequest(Email, Password, Urls.CLIENT_ID, Urls.CLIENT_SECRET);
            StringContent content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(Urls.LOGIN, content);
                if (response.IsSuccessStatusCode)
                {
                    string task = await response.Content.ReadAsStringAsync();
                    Response<LoginResponse> data = JsonConvert.DeserializeObject<Response<LoginResponse>>(task);
                    string accessToken = data.Data.AccessToken;
                    string refreshToken = data.Data.RefreshToken;
                    string tokenType = data.Data.TokenType;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                    // On met le token dans le header Authorization
        
                    response = await client.GetAsync(Urls.USER_PROFILE);
                    if (response.IsSuccessStatusCode)
                    {
                        task = await response.Content.ReadAsStringAsync();
                        Response<UserProfileResponse> userData = JsonConvert.DeserializeObject<Response<UserProfileResponse>>(task);
                        string firstName = userData.Data.FirstName;
                        string lastName = userData.Data.LastName;
                        UserInstance.User = new User(accessToken, refreshToken, tokenType, firstName, lastName, Email, Password);
                        
                        await NavigationService.PushAsync(new HomePage());
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