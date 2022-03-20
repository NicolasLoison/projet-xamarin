﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class TaskGraphViewModel : ViewModelBase
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
        
        public Chart TaskChart
        {
            get => _chart;
            set
            {
                SetProperty(ref _chart, value);
                OnPropertyChanged(nameof(TaskChart));
            }
        }

        public Project Project
        {
            get;
            set;
        }
        public string ChartLabel { get; set; }
        public TaskGraphViewModel(Project project)
        {
            Project = project;
            ChartLabel = "Time spent on " + Project.Name + " (minutes)";
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(UserInstance.User.TokenType, UserInstance.User.AccessToken);
            HttpResponseMessage response = await client.GetAsync(new Uri(Urls.LIST_TASKS.Replace("{projectId}", Project.Id.ToString())));
            if (response.IsSuccessStatusCode)
            {
                Task<string> task = response.Content.ReadAsStringAsync();
                Response<List<Task>> projectTasks =
                    JsonConvert.DeserializeObject<Response<List<Task>>>(task.Result);
                List<Task> tasks = new List<Task>(projectTasks.Data);
                Project.Tasks = tasks;
            }
            List<ChartEntry> entries = new List<ChartEntry>();
            foreach (Task task in Project.Tasks)
            {
                int time = task.GetTotalMinutes();
                if (time > 0)
                {
                    var color = SKColor.Parse(getRandColor());
                    entries.Add(new ChartEntry(time)
                    {
                        Color = color,
                        Label = task.Name,  
                        ValueLabel = time.ToString(),
                        ValueLabelColor = color,
                    });
                }
            }
            TaskChart = new DonutChart{Entries = entries, LabelTextSize = 30f};
            Working = false;
        }
    }
}