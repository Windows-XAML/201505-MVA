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

namespace ProtocolHandlerDemo
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

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new LauncherOptions();
            options.TargetApplicationPackageFamilyName = "b864e1fb-d8ed-4e8a-a0ba-1a530b180b71_nsf9e2fmhb1sj";
            await Windows.System.Launcher.LaunchUriAsync(new Uri("w10techpreview:?SomeData=123"), options);
        }
    }
}
