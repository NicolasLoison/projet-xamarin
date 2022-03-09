using System;
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
    public class InscriptionModelView : ViewModelBase
    {
        private string _lastName, _firstName, _email, _password;
        
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
        
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
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
        
        public ICommand ConfirmClick
        {
            get;
        }

        public InscriptionModelView()
        {
            _firstName = "";
            _lastName = "";
            _email = "";
            _password = "";
            ConfirmClick = new Command(CreateAccount);
        }

        public async void CreateAccount()
        {
            ErrorMessage = "";
            if (FirstName.Length == 0 || LastName.Length == 0 || Password.Length == 0 || Email.Length == 0)
            {
                ErrorMessage = "Missing informations";
            }
            else
            {
                try
                {
                    // si l'adresse mail est deja prise, afficher une pop up
                    // sinon, on va vers la page Home de l'utilisateur
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(Urls.HOST);
                    CreateUserRequest createRequest = new CreateUserRequest(Urls.CLIENT_ID, Urls.CLIENT_SECRET, Email,
                        FirstName, LastName, Password);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Urls.CREATE_USER, content);

                    Task<string> task = response.Content.ReadAsStringAsync();
                    Response<LoginResponse> data = JsonConvert.DeserializeObject<Response<LoginResponse>>(task.Result);
                    if (data != null)
                    {
                        if (data.ErrorCode != ErrorCodes.EMAIL_ALREADY_EXISTS)
                        {
                            string accessToken = data.Data.AccessToken;
                            string refreshToken = data.Data.RefreshToken;
                            string tokenType = data.Data.TokenType;
                            // On met le token dans le header Authorization
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                            HttpResponseMessage response2 = await client.GetAsync(new Uri(Urls.USER_PROFILE));

                            if (response2.IsSuccessStatusCode)
                            {   
                                UserInstance.User = new User(accessToken, refreshToken, tokenType, FirstName, LastName, Email, Password);
                                await NavigationService.PushAsync(new HomePage());
                            }
                        }
                        else
                        {
                            ErrorMessage = data.ErrorMessage;
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
        }
    }
}