using Xamarin.Forms;

namespace Projet
{
    public partial class InscriptionPage : ContentPage
    {
        public InscriptionPage()
        {
            InitializeComponent();
            BindingContext = new InscriptionModelView();
        }
        
    }
}