using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExpPanelRenderer
{
    public partial class App : Application
    {
        public App()
        {
            // CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            // customCulture.NumberFormat.NumberDecimalSeparator = ".";
            //
            // System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            
            InitializeComponent();

            MainPage = new AppShell();

            Startup.Init();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
