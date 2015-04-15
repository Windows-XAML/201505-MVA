using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Template10
{
    sealed partial class App : Common.BootStrapper
    {
        public App() : base()
        {
            this.InitializeComponent();
            this.SplashFactory = (s) => { return new Views.Splash(s); };
        }

        public override async Task OnLaunchedAsync(ILaunchActivatedEventArgs e)
        {
            // handle long-running tasks
            await Task.Delay(5000);

            // first screen
            this.NavigationService.Navigate(typeof(Views.MainPage));
        }
    }
}
