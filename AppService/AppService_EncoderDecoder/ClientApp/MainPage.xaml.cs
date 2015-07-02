using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ClientApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        
        private AppServiceConnection serviceConnection;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonEncode_Click(object sender, RoutedEventArgs e)
        {
            textBoxOutput.Text=  await CallServiceAsync("Encode");
        }

        private async void ButtonDecode_Click(object sender, RoutedEventArgs e)
        {
            
            textBoxOutput.Text = await CallServiceAsync("Decode");
        }

        private async Task<string> CallServiceAsync(string command)
        {
            if (serviceConnection == null)
            {
                // create a new service connection 
                serviceConnection = new AppServiceConnection();

                // provide app service name specified in appxmanifest
                serviceConnection.AppServiceName = "EndcoderDecoderService";

                // package family name of the ServerApp
                serviceConnection.PackageFamilyName = "02977d55-465f-411a-8c54-3b82442099a2_e7qah2kqbxs60";

                // open connection
                var status = await serviceConnection.OpenAsync();

                // handle the cases where the appservice connection fails
                if (status != AppServiceConnectionStatus.Success)
                {
                    Debug.WriteLine("Failed to open connection: " + status.ToString());
                    return "Error";
                }
            }

            // create a bag of values
            var message = new ValueSet();
            message.Add("cmd", command);
            message.Add("txt", textBox.Text);

            // send the message
            AppServiceResponse response = await serviceConnection.SendMessageAsync(message);
            if (response.Status == AppServiceResponseStatus.Success)
            {
                var result = response.Message["result"] as string;
                await new MessageDialog(result).ShowAsync();
                return result;
            }
            else
            {
                Debug.WriteLine("Message send failed!");
                return "Error";
            }
        }        
    }
}
