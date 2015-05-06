using SimplePushDemo.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace SimplePushDemo
{
    sealed partial class App : Common.BootStrapper
    {
        public App() : base()
        {
            this.InitializeComponent();
        }

        public override async Task OnLaunchedAsync(ILaunchActivatedEventArgs e)
        {
            this.NavigationService.Navigate(typeof(Views.MainPage));

            var channelURI = await PushHandlingHelper.RegisterForPushAsync();
            Debug.WriteLine(channelURI);
        }
    }
}
