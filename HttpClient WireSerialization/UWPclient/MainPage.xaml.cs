using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPclient
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


        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            HttpClient webclient = new HttpClient();
            // Get the atom+xml feed
            var result = await webclient.GetStringAsync(new Uri("http://localhost:1494/Northwind.svc/Suppliers"));
            await new MessageDialog("Received payload of " + result.Length + " characters").ShowAsync();

            ATOMCount.Text = result.Length.ToString();
        }

        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            HttpClient webclient = new HttpClient();
            // Get the 'traditional' (i.e. verbose) JSON feed
            webclient.DefaultRequestHeaders.Accept.TryParseAdd("application/json;odata=verbose");
            var result = await webclient.GetStringAsync(new Uri("http://localhost:1494/Northwind.svc/Suppliers"));
            await new MessageDialog("Received payload of " + result.Length + " characters").ShowAsync();

            JSONVerboseCount.Text = result.Length.ToString();
        }

        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            HttpClient webclient = new HttpClient();
            // Get the JSON Light feed (default in WCF Data services 5.1)
            webclient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
            var result = await webclient.GetStringAsync(new Uri("http://localhost:1494/Northwind.svc/Suppliers"));
            await new MessageDialog("Received payload of " + result.Length + " characters").ShowAsync();

            JSONLiteCount.Text = result.Length.ToString();
        }

        private async void button5_Click(object sender, RoutedEventArgs e)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            filter.AutomaticDecompression = true;
            HttpClient webclient = new HttpClient(filter);

            // Get the custom JSON Light feed Zipped service method
            var result = await webclient.GetStringAsync(new Uri("http://localhost:1494/Northwind.svc/GetSuppliersLiteZip"));
            await new MessageDialog("Received payload of " + result.Length + " characters").ShowAsync();

            JSONZipCount.Text = result.Length.ToString();
        }
    }
}
