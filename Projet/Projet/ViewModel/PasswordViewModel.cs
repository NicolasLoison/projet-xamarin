using System;
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
using TimeTracker.Dtos.Authentications.Credentials;
using Xamarin.Forms;

namespace Projet
{
    public class PasswordViewModel : ViewModelBase
    {
        private string _errorMessage;
        
        public string ErrorMessage
        {
            get => _errorMessage;
            set 
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(_errorMessage)); // Notify that there was a change on this property
            }
        }
        
        public string CurrentPassword
        {
            get;
            set;
        }

        public string NewPassword
        {
            get;
            set;
        }

        public ICommand SaveClick
        {
            get;
            set;
        }

        public User User
        {
            get => UserInstance.User;
            set => SetProperty(ref UserInstance.User, value);
        }
        
        public PasswordViewModel()
        {
            SaveClick = new Command(SaveChanges);
            _errorMessage = "";
        }
        
        public async void SaveChanges()
        {
            if (CurrentPassword == NewPassword)
            {
                ErrorMessage = "Entrez un nouveau mot de passe différent de l'actuel";
            }
            else
            {
                ErrorMessage = "";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(User.TokenType, User.AccessToken);
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
                    SetPasswordRequest passwordRequest = new SetPasswordRequest(CurrentPassword, NewPassword);
                    content = new StringContent(JsonConvert.SerializeObject(passwordRequest),
                        Encoding.UTF8, "application/json");

                    HttpMethod method = new HttpMethod("PATCH");
                    HttpRequestMessage request = new HttpRequestMessage(method, Urls.SET_PASSWORD)
                    {
                        Content = content
                    };
                    response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.ReasonPhrase);
                        task = response.Content.ReadAsStringAsync();
                        Response<SetUserProfileRequest> data =
                            JsonConvert.DeserializeObject<Response<SetUserProfileRequest>>(task.Result);
                        if (data.ErrorCode == ErrorCodes.WEAK_PASSWORD)
                        {
                            ErrorMessage = data.ErrorMessage;
                        }
                        else
                        {
                            ErrorMessage = "";
                            User.Password = NewPassword;
                            await NavigationService.PopAsync(false);
                            await NavigationService.PushAsync(new UserProfilePage());
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                    }
                }
            }
        }
    }
}