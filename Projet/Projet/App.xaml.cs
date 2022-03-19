using System;
using Storm.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("MaterialIcons-Regular.ttf", Alias = "MatIcons")]
[assembly: ExportFont("fabrands400.ttf", Alias = "FAB")]
[assembly: ExportFont("faregular400.ttf", Alias = "FAR")]
[assembly: ExportFont("fasolid900.ttf", Alias = "FAS")]


namespace Projet
{
    public partial class App : MvvmApplication
    {
        public App() : base(() => new ConnexionPage())
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}