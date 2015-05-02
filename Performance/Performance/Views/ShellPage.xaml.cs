using Windows.UI.Xaml.Controls;

namespace Template10.Views
{
    public sealed partial class ShellPage : Page
    {
        Services.NavigationService.NavigationService _navigationService;
        public ShellPage(Frame frame)
        {
            this.InitializeComponent();
            this.MySplitView.Content = frame;
            _navigationService = (App.Current as Common.BootStrapper).NavigationService;
        }

        private void Rav_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService.Navigate(typeof(Views.RandomAccessPage));
        }

        private void Iav_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService.Navigate(typeof(Views.IncrementalAccessPage));
        }

        private void Dpr_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService.Navigate(typeof(Views.DeferedPhasePage));
        }
    }
}
