using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Projet.Model;
using Storm.Mvvm;
using Xamarin.Forms;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Accounts;
using TimeTracker.Dtos.Authentications;

namespace Projet
{
    public class UserProfileViewModel : ViewModelBase
    {
        private string _nom, _prenom, _mail, _motDePasse;
        private string _accessToken, _refreshToken, _tokenType;

        public User User
        {
            get => UserInstance.User;
            set => SetProperty(ref UserInstance.User, value);
        }
        public string LastName
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }
        public string FirstName
        {
            get => _prenom;
            set => SetProperty(ref _prenom, value);
        }
        public string Email
        {
            get => _mail;
            set => SetProperty(ref _mail, value);
        }
        public string Password
        {
            get => _motDePasse;
            set => SetProperty(ref _motDePasse, value);
        }
        
        public ICommand PasswordClick
        {
            get;
            set;
        }
        
        public ICommand SaveClick
        {
            get;
            set;
        }
        
        public ICommand HomeClick
        {
            get;
            set;
        }
        public UserProfileViewModel()
        {
            LastName = User.LastName;
            FirstName = User.FirstName;
            Email = User.Email;
            Password = User.Password;
            _accessToken = User.AccessToken;
            _refreshToken = User.RefreshToken;
            _tokenType = User.TokenType;
            PasswordClick = new Command(ToPassword);
            SaveClick = new Command(SaveChanges);
            HomeClick = new Command(ToHome);
        }

        public async void SaveChanges()
        {
            // si l'adresse mail est deja prise, afficher une pop up
            // sinon, on va vers la page Home de l'utilisateur
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_tokenType, _accessToken);
                SetUserProfileRequest profileRequest = new SetUserProfileRequest(Email, FirstName, LastName);

                content = new StringContent(JsonConvert.SerializeObject(profileRequest), Encoding.UTF8, "application/json");

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, Urls.SET_USER_PROFILE) {
                    Content = content
                };
                response = await client.SendAsync(request);
                Console.WriteLine(response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                {
                    task = response.Content.ReadAsStringAsync();
                    Response<SetUserProfileRequest> data = JsonConvert.DeserializeObject<Response<SetUserProfileRequest>>(task.Result);
                    User.Email = data.Data.Email;
                    User.FirstName = data.Data.FirstName;
                    User.LastName = data.Data.LastName;
                    await NavigationService.PushAsync(new UserProfilePage());
                }
            }
        }

        public async void ToHome()
        {
            await NavigationService.PushAsync(new HomePage());
        }
        
        public async void ToPassword()
        {
            await NavigationService.PushAsync(new PasswordPage());
        }
    }
}