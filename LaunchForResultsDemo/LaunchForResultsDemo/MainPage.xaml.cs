using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LaunchForResultsDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LaunchForResultButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new LauncherOptions();
            options.TargetApplicationPackageFamilyName = "dae4aecf-5aae-497c-97b6-4122dec5af28_nsf9e2fmhb1sj";

            var launchResult = await Windows.System.Launcher.LaunchUriForResultsAsync(
                new Uri("w10jumpstart:/this_bit_is_not_used_in_this_demo"), 
                options, 
                null);

            Frame.Navigate(typeof(ContinuationPage), launchResult.Result);
        }
    }
}
