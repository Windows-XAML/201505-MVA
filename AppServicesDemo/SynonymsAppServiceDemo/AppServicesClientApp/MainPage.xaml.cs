using SynonymsServiceClientLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace AppServicesClientApp
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

        #region Call Calculator App Service
        AppServiceConnection connection = null;

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
                var cd = new ContentDialog();
                cd.Title = "Result=" + sum;
                cd.PrimaryButtonText = "OK";
                cd.PrimaryButtonClick += (s, a) => cd.Hide();
                cd.ShowAsync();
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
                    //connection.RequestReceived += OnRequestReceived;
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
        #endregion


        //#region Call Synonyms Service Directly

        //AppServiceConnection synonymsServiceConnection = null;

        //private async void GetSynonymsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.synonymsServiceConnection == null)
        //    {
        //        synonymsServiceConnection = new AppServiceConnection();

        //        // See the appx manifest of the AppServicesDemp app for this value
        //        synonymsServiceConnection.AppServiceName = "MicrosoftDX-SynonymsService";
        //        // Use the Windows.ApplicationModel.Package.Current.Id.FamilyName API in the 
        //        // provider app to get this value
        //        synonymsServiceConnection.PackageFamilyName = "82a987d5-4e4f-4cb4-bb4d-700ede1534ba_nsf9e2fmhb1sj";

        //        AppServiceConnectionStatus connectionStatus = await synonymsServiceConnection.OpenAsync();
        //        if (connectionStatus == AppServiceConnectionStatus.Success)
        //        {
        //            synonymsServiceConnection.ServiceClosed += OnServiceClosed;
        //        }
        //        else
        //        {
        //            //Drive the user to store to install the app that provides 
        //            //the app service 
        //        }
        //    }

        //    //Send data to the service 
        //    var message = new ValueSet();
        //    message.Add("Command", "GetSynonym");
        //    message.Add("Term", Term.Text);

        //    //Send a message  
        //    AppServiceResponse response = await synonymsServiceConnection.SendMessageAsync(message);

        //    List<string> synonyms = null;
        //    if (response.Status == AppServiceResponseStatus.Success)
        //    {
        //        if (response.Message.ContainsKey("Result"))
        //        {
        //            synonyms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((string)response.Message["Result"]);
        //        }
        //        else if (response.Message.ContainsKey("Error"))
        //        {
        //            throw new Exception("Error:" + response.Message["Error"]);
        //        }
        //    }

        //    // Display outcome
        //    await DisplaySynonymsResponse(
        //        new SynonymsServiceResponse()
        //        {
        //            Status = response.Status,
        //            Synonyms = synonyms,
        //        });
        //}

        //#endregion

        #region Call Synonyms Service using Client API

        SynonymsServiceClientLibrary.SynonymsServiceClient synonymsClient =
            new SynonymsServiceClientLibrary.SynonymsServiceClient();

        private async void GetSynonymsButton_Click(object sender, RoutedEventArgs e)
        {
            SynonymsServiceClientLibrary.SynonymsServiceResponse synonymsResponse =
                await synonymsClient.GetSynonymsAsync(Term.Text);

            await DisplaySynonymsResponse(synonymsResponse);
        }

        #endregion





        private async System.Threading.Tasks.Task DisplaySynonymsResponse(SynonymsServiceClientLibrary.SynonymsServiceResponse synonymsResponse)
        {
            string synonymsOutput = "";
            if (synonymsResponse.Status == AppServiceResponseStatus.Success)
            {
                foreach (var item in synonymsResponse.Synonyms)
                {
                    synonymsOutput += "|" + item;
                }

                var cd = new ContentDialog();
                cd.Title = "Synonyms=" + synonymsOutput;
                cd.PrimaryButtonText = "OK";
                cd.PrimaryButtonClick += (s, a) => cd.Hide();
                await cd.ShowAsync();
            }
            else
            {
                await new MessageDialog("Synonyms API call failed").ShowAsync();
            }
        }
    }
}
