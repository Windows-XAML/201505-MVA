
namespace Shop.ViewModels
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation.Collections;  
    using Windows.System;

    class MainPageViewModel : CanNotifyPropertyChanged
    {
        private ObservableCollection<Product> products;

        public Windows.UI.Xaml.Controls.Frame Frame { set; private get; }


        public MainPageViewModel()
        {
            this.CreateSampleProducts();

            this.CheckOutCommand = new Command(async () => await this.CheckOut(), this.CanCheckOut);
        }

        public Command CheckOutCommand { get; }

        public ObservableCollection<Product> AvailableProducts
        {
            get
            {
                return this.products;
            }

            set
            {
                this.SetProperty(ref this.products, value);
            }
        }

        private void CreateSampleProducts()
        {
            this.AvailableProducts = new ObservableCollection<Product>();

            // In the tradition of all the greatest demos, we're using hardcoded sample data
            // In a "real" app this would almost certainly come from a web service ;)

            this.AddProduct("Widget", 1.99);
            this.AddProduct("Sprocket", 2.99);
            this.AddProduct("Contraption", 5.99);
            this.AddProduct("Gizmo", 9.99);
        }

        private void AddProduct(string name, double price)
        {
            this.AvailableProducts.Add(new Product(() => this.CheckOutCommand.RaiseCanExecuteChanged()) { Name = name, Price = price });
        }

        private bool CanCheckOut()
        {
            return this.AvailableProducts.Any(p => p.IsSelected);
        }

        private async Task CheckOut()
        {
            var checkoutAppUri = new Uri("checkoutdemo:");

            // We want a specific app to perform our checkout operation, not just any that implements the protocol we're using for launch
            var options = new LauncherOptions();
            options.TargetApplicationPackageFamilyName = "7d7072f5-7a53-47f6-9932-97647aad9550_z5jhcbv2wstvg";

            var transactionId = Guid.NewGuid().ToString();

            var inputData = new ValueSet();

            // Our collection is too complicated for ValueSet to understand so we serialize it first
            var serializedSelectedItems = JsonConvert.SerializeObject(this.AvailableProducts.Where(p => p.IsSelected).Select(p => new { Description = p.Name, UnitPrice = p.Price, Quantity = 1 }).ToList());

            inputData["Items"] = serializedSelectedItems;
            inputData["Transaction"] = transactionId;

            var response = await Launcher.LaunchUriForResultsAsync(checkoutAppUri, options, inputData);
            if (response.Status == LaunchUriStatus.Success)
            {
                Frame.Navigate(typeof(Receipt), response.Result);
            }
            else
            {
                // TODO: handle failure to launch...
            }
        }
    }
}
