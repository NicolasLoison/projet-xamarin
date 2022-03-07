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
using Xamarin.Forms.Internals;
using Newtonsoft.Json.Linq;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Accounts;
using TimeTracker.Dtos.Authentications;

namespace Projet
{
    public class InscriptionModelView : ViewModelBase
    {
        private string _nom, _prenom, _mail, _motDePasse;

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
        
        public ICommand ConfirmClick
        {
            get;
        }

        public InscriptionModelView()
        {
            ConfirmClick = new Command(CreateAccount);
        }

        public async void CreateAccount()
        {
            try
            {
                // si l'adresse mail est deja prise, afficher une pop up
                // sinon, on va vers la page Home de l'utilisateur
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Urls.HOST);
                string jsonData =
                    $@"{{""client_id"": ""{Urls.CLIENT_ID}"", ""client_secret"": ""{Urls.CLIENT_SECRET}"",""email"" : ""{Email}"",
                        ""first_name"" : ""{FirstName}"",""last_name"" : ""{LastName}"", ""password"" : ""{Password}""}}";

                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(Urls.CREATE_USER, content);

                Task<string> task = response.Content.ReadAsStringAsync();
                Response<LoginResponse> data = JsonConvert.DeserializeObject<Response<LoginResponse>>(task.Result);
                string accessToken = data.Data.AccessToken;
                string refreshToken = data.Data.RefreshToken;
                string tokenType = data.Data.TokenType;
                // On met le token dans le header Authorization
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                HttpResponseMessage response2 = await client.GetAsync(new Uri(Urls.USER_PROFILE));
                // Console.WriteLine("Resultat GET: " + response2);

                if (response2.IsSuccessStatusCode)
                {   
                    UserInstance.User = new User(accessToken, refreshToken, tokenType, FirstName, LastName, Email, Password);
                    await NavigationService.PushAsync(new HomePage());
                }
            
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}