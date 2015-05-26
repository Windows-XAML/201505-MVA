using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The RoamingSettingsDemo Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RoamingSettingsDemo.Views
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetBackgroundFromSettings();

            Windows.Storage.ApplicationData.Current.DataChanged += RoamingDataChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.DataChanged -= RoamingDataChanged;
        }

        private void RoamingDataChanged(Windows.Storage.ApplicationData sender, object args)
        {
            // Something has changed in the roaming data or settings
            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                () => SetBackgroundFromSettings()
            );
        }

        private void SetBackgroundFromSettings()
        {
            // Get the roaming settings
            Windows.Storage.ApplicationDataContainer roamingSettings =
                   Windows.Storage.ApplicationData.Current.RoamingSettings;

            if (roamingSettings.Values.ContainsKey("PreferredBgColor"))
            {
                var colorName = roamingSettings.Values["PreferredBgColor"].ToString();

                if (colorName == "Green")
                {
                    MainGrid.Background = new SolidColorBrush(Colors.Green);
                    GreenRadioButton.IsChecked = true;
                }
                else if (colorName == "Red")
                {
                    MainGrid.Background = new SolidColorBrush(Colors.Red);
                    RedRadioButton.IsChecked = true;
                }
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (GreenRadioButton.IsChecked.HasValue && (GreenRadioButton.IsChecked.Value == true))
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["PreferredBgColor"] = "Green";
            }
            else
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["PreferredBgColor"] = "Red";
            }

            SetBackgroundFromSettings();
        }
    }
}
