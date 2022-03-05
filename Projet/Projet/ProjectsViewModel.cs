using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Newtonsoft.Json;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace Projet
{
    public class ProjectsViewModel : ViewModelBase
    {
        static ISettings AppSettings => CrossSettings.Current;
        public static string JsonProjets;
        private Model.Projet _projetChoisi;
        public static string SavedProjets 
        {
            get => AppSettings.GetValueOrDefault("MySettingKey", JsonProjets);
            set => AppSettings.AddOrUpdateValue("MySettingKey", value);
        }
        static ObservableCollection<Model.Projet> _projets = new ObservableCollection<Model.Projet>();

        public Model.Projet ProjetChoisi
        {
            get => _projetChoisi;
            set => SetProperty(ref _projetChoisi, value);
        }
        
        public ObservableCollection<Model.Projet> Projets
        {
            get => _projets;
        }
        public ICommand AddClick
        {
            get;
        }
        public ICommand DetailClick
        {
            get;
        }
        
        public ICommand DeleteClick
        {
            get;
        }
        
        public void UpdateSavedProjects()
        {
            SavedProjets = JsonConvert.SerializeObject(_projets);
        }

        public ProjectsViewModel()
        {
            if (!string.IsNullOrEmpty(SavedProjets))
            {
                _projets = JsonConvert.DeserializeObject<ObservableCollection<Model.Projet>>(SavedProjets);
            }
            else
            {
                _projets.Add(new Model.Projet("Xamarin", "Projet Time Tracker"));
                _projets.Add(new Model.Projet("Flutter", "Implémentation d'une calculette"));
                _projets.Add(new Model.Projet("TER", "Analyse de grammaire LR(1)"));
            }
            AddClick = new Command(AddTodo);
            DetailClick = new Command(ViewProject);
            DeleteClick = new Command(Delete);
        }
        
        public async void ViewProject()
        {
            if (ProjetChoisi == null) return;
            try
            {
                ProjectDetailPage projectPage = new ProjectDetailPage(this, ProjetChoisi);
                await NavigationService.PushAsync(projectPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        public async void Delete()
        {
            if (ProjetChoisi == null) return;
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this TODO?", "Yes", "No");
            if (!answer) return;
            Projets.Remove(ProjetChoisi);
            ProjetChoisi = null;
            UpdateSavedProjects();
        }

        public async void AddTodo()
        {
        //     try
        //     {
        //         AddPage addPage = new AddPage(this);
        //         await NavigationService.PushAsync(addPage);
        //     }
        //     catch(Exception e)
        //     {
        //         Console.WriteLine(e.StackTrace);
        //     }
        }
    }
}