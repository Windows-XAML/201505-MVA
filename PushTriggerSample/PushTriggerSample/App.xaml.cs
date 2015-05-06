using PushTriggerSample.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace PushTriggerSample
{
    sealed partial class App : Common.BootStrapper
    {
        public App() : base()
        {
            this.InitializeComponent();
        }

        public async override Task OnLaunchedAsync(ILaunchActivatedEventArgs e)
        {
            this.NavigationService.Navigate(typeof(Views.MainPage));

            var channelURI = await PushHandlingHelper.RegisterForPushAsync();
            Debug.WriteLine(channelURI);
        }
    }
}
