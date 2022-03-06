using System;
using System.Windows.Input;
using Projet.Model;
using Storm.Mvvm;
using Xamarin.Forms;

namespace Projet
{
    public class HomeModelView : ViewModelBase
    {
        // private string _nom, _prenom, _mail, _motDePasse;

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
        
        public User User
        {
            get;
            set;
        }

        public HomeModelView(User user)
        {
            User = user;
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
            try
            {
                Model.Projet p = new Model.Projet(1, "Projet de Test", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam et pretium sapien. Mauris ac justo eros. Nam accumsan ligula nunc, quis iaculis ligula feugiat a. Ut egestas nibh interdum sem porttitor, a vestibulum purus malesuada. Curabitur convallis, felis id mollis rhoncus, urna turpis ornare velit, nec consequat nisi lacus nec neque. Morbi varius faucibus metus sit amet vestibulum. Sed in varius tellus, ac imperdiet diam. Morbi iaculis leo enim, vel mattis risus tempus tincidunt. Fusce vitae felis dolor. Vestibulum rhoncus dapibus nisi vitae sagittis. Sed facilisis sit amet risus eu pharetra.");
                Task t = new Task(1, "Ecrire le code à la ligne 202");
                Task t2 = new Task(2, "Nettoyer et commenter le code");
                t.Times.Add(new Timer(1, DateTime.Now, DateTime.Today));
                t2.Times.Add(new Timer(1, DateTime.Now, DateTime.MaxValue));
                p.Tasks.Add(t);
                p.Tasks.Add(t2);
                await NavigationService.PushAsync(new ProjectPage(p));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
    }
}