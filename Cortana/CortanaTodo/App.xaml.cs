using System;
using System.Threading.Tasks;
using Template10.Services;
using Windows.ApplicationModel.Activation;

namespace CortanaTodo
{
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App() : base()
        {
            this.InitializeComponent();
        }

        public override Task OnLaunchedAsync(ILaunchActivatedEventArgs e)
        {
            this.NavigationService.Navigate(typeof(Views.MainPage));
            return TaskHelper.CompletedTask;
        }
    }
}
