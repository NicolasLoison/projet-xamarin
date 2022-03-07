using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Projet.Model;
using Storm.Mvvm;
using Xamarin.Forms;

namespace Projet
{
    public class HomeModelView : ViewModelBase
    {
        // private string _nom, _prenom, _mail, _motDePasse;
        private Model.Projet _chosenProject;
        private ObservableCollection<Model.Projet> _projets = new ObservableCollection<Model.Projet>();
        
        static ISettings AppSettings => CrossSettings.Current;
        public static string JsonProjects;
        public static string SavedProjects 
        {
            get => AppSettings.GetValueOrDefault("MySettingKey", JsonProjects);
            set => AppSettings.AddOrUpdateValue("MySettingKey", value);
        }
        
        public ICommand ProfileClick
        {
            get;
            set;
        }
        public ICommand ProjectClick
        {
            get;
            set;
        }
        public ObservableCollection<Model.Projet> Projets
        {
            get => _projets;
            set => SetProperty(ref _projets, value);
        }
        
        public Model.Projet ChosenProject
        {
            get => _chosenProject;
            set => SetProperty(ref _chosenProject, value);
        }
        
        public User User
        {
            get;
            set;
        }

        public HomeModelView(User user)
        {
            User = user;
            Model.Projet p = new Model.Projet(1, "Projet 1", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam et pretium sapien. Mauris ac justo eros. Nam accumsan ligula nunc, quis iaculis ligula feugiat a. Ut egestas nibh interdum sem porttitor, a vestibulum purus malesuada. Curabitur convallis, felis id mollis rhoncus, urna turpis ornare velit, nec consequat nisi lacus nec neque. Morbi varius faucibus metus sit amet vestibulum. Sed in varius tellus, ac imperdiet diam. Morbi iaculis leo enim, vel mattis risus tempus tincidunt. Fusce vitae felis dolor. Vestibulum rhoncus dapibus nisi vitae sagittis. Sed facilisis sit amet risus eu pharetra.");
            Task t = new Task(1, "Ecrire le code à la ligne 202");
            t.Times.Add(new Timer(1, DateTime.Now, DateTime.Today));
            
            Model.Projet p2 = new Model.Projet(2, "Projet 2", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam et pretium sapien. Mauris ac justo eros. Nam accumsan ligula nunc, quis iaculis ligula feugiat a. Ut egestas nibh interdum sem porttitor, a vestibulum purus malesuada. Curabitur convallis, felis id mollis rhoncus, urna turpis ornare velit, nec consequat nisi lacus nec neque. Morbi varius faucibus metus sit amet vestibulum. Sed in varius tellus, ac imperdiet diam. Morbi iaculis leo enim, vel mattis risus tempus tincidunt. Fusce vitae felis dolor. Vestibulum rhoncus dapibus nisi vitae sagittis. Sed facilisis sit amet risus eu pharetra.");
            Task t2 = new Task(2, "Nettoyer et commenter le code");
            t2.Times.Add(new Timer(1, DateTime.Now, DateTime.MaxValue));
            
            p.Tasks.Add(t);
            p2.Tasks.Add(t2);

            _projets.Add(p);
            _projets.Add(p2);
            ProfileClick = new Command(ToProfile);
            ProjectClick = new Command(ToProject);
        }

        public async void ToProfile()
        {
            try
            {
                await NavigationService.PushAsync(new UserProfilePage(User));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public async void ToProject()
        {
            if (ChosenProject == null) return;
            try
            {
                ProjectPage projectPage = new ProjectPage(ChosenProject);
                await NavigationService.PushAsync(projectPage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        public async void DeleteTask()
        {
            if (ChosenProject == null) return;
            bool answer = await App.Current.MainPage.DisplayAlert("Delete", "Do you really want to delete this project?", "Yes", "No");
            if (!answer) return;
            Projets.Remove(ChosenProject);
            ChosenProject = null;
        }
    }
}