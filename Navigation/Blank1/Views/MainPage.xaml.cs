using Windows.UI.Xaml.Controls;

namespace Blank1.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            this.DataContextChanged += (s, e) => ViewModel = DataContext as ViewModels.MainPageViewModel;
        }

        // strongly-typed view models enable x:bind
        public ViewModels.MainPageViewModel ViewModel { get; set; }

private void Goto2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
{
    var app = App.Current as Common.BootStrapper;
    var nav = app.NavigationService;
    nav.Navigate(typeof(Views.Page2), parameterTextBox.Text);
}
    }
}