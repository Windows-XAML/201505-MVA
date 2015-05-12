
namespace Shop
{
    using Shop.ViewModels;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Inject the Frame ito the viewmodel so it can do navigation 
            ((MainPageViewModel)this.DataContext).Frame = this.Frame;

            // If we come back to this page it will be after showing the receipt - clear evidence of past transaction
            if (e.NavigationMode == NavigationMode.Back)
            {
                var products = (App.Current.Resources["locator"] as ViewModelLocator).Main.AvailableProducts;
                foreach (var product in products)
                {
                    product.IsSelected = false;
                }
            }
        }
    }
}
