using Windows.UI.Xaml.Controls;

namespace Template10.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GotoAbout(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            (App.Current as Common.BootStrapper).NavigationService.Navigate(typeof(Views.AboutPage));
        }
    }
}
