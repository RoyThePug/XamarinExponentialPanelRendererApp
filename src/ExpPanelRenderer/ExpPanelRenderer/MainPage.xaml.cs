
using ExpPanelRenderer.ViewModel;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace ExpPanelRenderer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            BindingContext = Startup.ServiceProvider.GetService<MainViewModel>();
        }
    }
}
