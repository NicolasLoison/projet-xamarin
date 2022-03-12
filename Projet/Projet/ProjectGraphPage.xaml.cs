using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microcharts;
using Newtonsoft.Json;
using Projet.Model;
using SkiaSharp;
using TimeTracker.Dtos;
using TimeTracker.Dtos.Authentications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Task = Projet.Model.Task;

namespace Projet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectGraphPage : ContentPage
    {
        public ProjectGraphPage()
        {
            InitializeComponent();
            BindingContext = new ProjectGraphViewModel();
        }
    }
}