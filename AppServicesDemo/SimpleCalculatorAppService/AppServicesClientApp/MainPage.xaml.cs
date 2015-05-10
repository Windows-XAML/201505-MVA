using System;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppServicesClientApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AppServiceConnection connection = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void CallAppService()
        {
            await EnsureConnectionToService();

            //Send data to the service 
            var message = new ValueSet();
            message.Add("Command", "CalcSum");
            message.Add("Value1", Int32.Parse(Value1.Text));
            message.Add("Value2", Int32.Parse(Value2.Text));

            //Send a message  
            AppServiceResponse response = await connection.SendMessageAsync(message);
            if (response.Status == AppServiceResponseStatus.Success)
            {
                int sum = (int)response.Message["Result"];
                ResultTextBox.Text = sum.ToString();
            }
        }

        private async System.Threading.Tasks.Task EnsureConnectionToService()
        {
            if (this.connection == null)
            {
                connection = new AppServiceConnection();

                // See the appx manifest of the AppServicesDemp app for this value
                connection.AppServiceName = "microsoftDX-appservicesdemo";
                // Use the Windows.ApplicationModel.Package.Current.Id.FamilyName API in the 
                // provider app to get this value
                connection.PackageFamilyName = "82a987d5-4e4f-4cb4-bb4d-700ede1534ba_nsf9e2fmhb1sj";

                AppServiceConnectionStatus connectionStatus = await connection.OpenAsync();
                if (connectionStatus == AppServiceConnectionStatus.Success)
                {
                    connection.ServiceClosed += OnServiceClosed;
                }
                else
                {
                    //Drive the user to store to install the app that provides 
                    //the app service 
                }
            }
        }

        private void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            new MessageDialog("Service closed").ShowAsync();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            CallAppService();
        }

        private async void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            await EnsureConnectionToService();

            //Send data to the service 
            var message = new ValueSet();
            message.Add("Command", "Quit");

            //Send a message  
            await connection.SendMessageAsync(message);

            ClearServiceConnection();
        }

        private void ClearServiceConnection()
        {
            this.connection.Dispose();
            this.connection = null;
        }
    }
}
