using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Template10
{
    sealed partial class App : Common.BootStrapper
    {
        public App() : base()
        {
            InitializeComponent();
        }

        public override Task OnLaunchedAsync(ILaunchActivatedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.MainPage));
            return Task.FromResult<object>(null);
        }
    }
}
