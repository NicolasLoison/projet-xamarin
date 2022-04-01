using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microcharts;
using Microcharts.Forms;
using Newtonsoft.Json;
using Projet.Model;
using SkiaSharp;
using Storm.Mvvm;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;
using Task = Projet.Model.Task;

namespace Projet
{
    public class ProjectGraphViewModel : ViewModelBase
    {
        public bool _working;


        public bool Working
        {
            get => _working;
            set
            {
                SetProperty(ref _working, value);
                OnPropertyChanged(nameof(Working));
            }
        }

        private Chart _chart;

        public Chart ProjectChart
        {
            get => _chart;
            set
            {
                SetProperty(ref _chart, value);
                OnPropertyChanged(nameof(ProjectChart));
            }
        }

        public ProjectGraphViewModel()
        {
            Working = false;
            FindTasks();
        }

        private string getRandColor()
        {
            Random rnd = new Random();
            string hexOutput = String.Format("{0:X}", rnd.Next(0, 0xFFFFFF));
            while (hexOutput.Length < 6)
                hexOutput = "0" + hexOutput;
            return "#" + hexOutput;
        }

        public async void FindTasks()
        {
            Working = true;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Urls.HOST);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
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
                foreach (Project p in UserInstance.User.Projets)
                {
                    response =
                        await client.GetAsync(new Uri(Urls.LIST_TASKS.Replace("{projectId}", p.Id.ToString())));
                    if (response.IsSuccessStatusCode)
                    {
                        task = response.Content.ReadAsStringAsync();
                        Response<List<Task>> projectTasks =
                            JsonConvert.DeserializeObject<Response<List<Task>>>(task.Result);
                        List<Task> tasks = new List<Task>(projectTasks.Data);
                        p.Tasks = tasks;
                    }
                }

                List<ChartEntry> entries = new List<ChartEntry>();
                foreach (Project p in UserInstance.User.Projets)
                {
                    double time = p.GetTotalMinutes();
                    if (time > 0)
                    {
                        var color = SKColor.Parse(getRandColor());
                        entries.Add(new ChartEntry((float) time)
                        {
                            Label = p.Name,
                            ValueLabel = time.ToString("F1"),
                            Color = color,
                            ValueLabelColor = color,
                        });
                    }

                }

                ProjectChart = new DonutChart {Entries = entries, LabelTextSize = 30f};
                Working = false;
            }
        }
    }
}