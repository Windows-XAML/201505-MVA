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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace VisibleBoundsDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Color? DefaultTitleBarButtonsBGColor;
        private Color? DefaultTitleBarBGColor;
        public MainPage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBoundsChanged += MainPage_VisibleBoundsChanged;

            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            DefaultTitleBarBGColor = viewTitleBar.BackgroundColor;
            DefaultTitleBarButtonsBGColor = viewTitleBar.ButtonBackgroundColor;
        }

        void MainPage_VisibleBoundsChanged(Windows.UI.ViewManagement.ApplicationView sender, object args)
        {
            ReportVisibleBounds();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            ReportVisibleBounds();

        }

        private void ReportVisibleBounds()
        {
            var vb = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds;

            TopTextBox.Text = String.Format("{0:0.000}", vb.Top);
            BottomTextBox.Text = String.Format("{0:0.000}", vb.Bottom);
            HeightTextBox.Text = String.Format("{0:0.000}", vb.Height);
            WidthTextBox.Text = String.Format("{0:0.000}", vb.Width);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Bottom AppBar shows on Desktop and Mobile
            if (ShowAppBarRadioButton != null)
            {
                if (ShowAppBarRadioButton.IsChecked.HasValue && (ShowAppBarRadioButton.IsChecked.Value == true))
                {
                    commandBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    commandBar.Opacity = 1;
                }
                else
                {
                    commandBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }

            if (ShowOpaqueAppBarRadioButton != null)
            {
                if (ShowOpaqueAppBarRadioButton.IsChecked.HasValue && (ShowOpaqueAppBarRadioButton.IsChecked.Value == true))
                {
                    commandBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    commandBar.Background.Opacity = 0;
                }
                else
                {
                    commandBar.Background.Opacity = 1;
                }
            }
        }

        private void StatusBarHiddenCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // StatusBar is Mobile only
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var ignore = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }
        }

        private void StatusBarHiddenCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // StatusBar is Mobile only
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var ignore = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            }
        }

        private void StatusBarBackgroundCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // StatusBar is Mobile only
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundColor = Windows.UI.Colors.Blue;
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = 1;
            }
        }

        private void StatusBarBackgroundCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // StatusBar is Mobile only
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = 0;
            }
        }

        private void ExtendViewButton_Checked(object sender, RoutedEventArgs e)
        {
            // You can extend your view into the TitleBar.
            // Has no effect on Mobile or when FullScreen on tablet, but has effect when running windowed on Desktop
            bool extendView = ExtendViewCheckBox.IsChecked.HasValue && (ExtendViewCheckBox.IsChecked.Value == true) ? true : false;
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = extendView;
        }

        private void ColourTitleBarButtons_Checked(object sender, RoutedEventArgs e)
        {
            // You can change properties of the TitleBar.
            // Has no effect on Mobile or when FullScreen on tablet since there is no TitleBar visible, 
            // but has effect when running windowed on Desktop
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;

            if (ColourTitleBarButtonsCheckBox.IsChecked.HasValue && (ColourTitleBarButtonsCheckBox.IsChecked.Value == true))
            {
                viewTitleBar.ButtonBackgroundColor = Colors.Yellow;
                viewTitleBar.BackgroundColor = Colors.Transparent;
            }
            else
            {
                viewTitleBar.ButtonBackgroundColor = DefaultTitleBarButtonsBGColor;
                viewTitleBar.BackgroundColor = DefaultTitleBarBGColor;
            }

        }
    }
}
