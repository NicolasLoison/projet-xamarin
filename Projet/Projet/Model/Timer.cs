using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;

namespace Projet.Model
{
    public class Timer : ViewModelBase
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("start_time")] public DateTime StartTime { get; set; }
        [JsonProperty("end_time")] public DateTime EndTime { get; set; }

        public string StartTimeFormat
        {
            get => Convert.ToDateTime(StartTime).ToString("dd/MM/yyyy HH:mm");
        }

        public string EndTimeFormat
        {
            get => Convert.ToDateTime(EndTime).ToString("dd/MM/yyyy HH:mm");
        }

        public string TotalTimeFormat
        {
            get
            {
                TimeSpan t = EndTime.Subtract(StartTime);
                string res = t.ToString("%d") + "d ";
                res += t.ToString("%h") + "h ";
                res += t.ToString("%m") + "min ";
                res += t.ToString("%s") + "s";
                return res;
            }
        }

        public ICommand DeleteClick { get; set; }

        public TaskViewModel View { get; set; }

        public Timer(int id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            DeleteClick = new Command(DeleteTimer);
        }

        public int GetTotalHours()
        {
            TimeSpan t = EndTime.Subtract(StartTime);
            Console.WriteLine("Timer: " + t.Hours);
            return t.Minutes;
        }

        public double GetTotalMinutes()
        {
            TimeSpan t = EndTime.Subtract(StartTime);
            Console.WriteLine("Timer: " + t.Minutes);
            return t.TotalMinutes;
        }


        public async void DeleteTimer()
        {
            bool answer =
                await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this timer?", "Yes",
                    "No");
            if (!answer) return;
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
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
                response = await client.DeleteAsync(new Uri(
                    Urls.DELETE_TIME.Replace("{projectId}", View.Task.View.Project.Id.ToString())
                        .Replace("{taskId}", View.Task.Id.ToString()).Replace("{timeId}", Id.ToString())));
                if (response.IsSuccessStatusCode)
                {
                    View.Timers.Remove(this);
                }
            }
        }
    }
}