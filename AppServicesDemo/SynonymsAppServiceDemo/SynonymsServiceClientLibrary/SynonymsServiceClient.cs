using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace SynonymsServiceClientLibrary
{
    public class SynonymsServiceClient
    {
        AppServiceConnection synonymsServiceConnection = null;

        public event EventHandler<AppServiceClosedEventArgs> ServiceClosed;

        public async Task<SynonymsServiceResponse> GetSynonymsAsync(string term)
        {
            SynonymsServiceResponse callresponse = new SynonymsServiceResponse()
            {
                Status = AppServiceResponseStatus.Unknown,
                Synonyms = null,
            };

            List<string> synonyms = new List<string>();

            await EnsureConnectionToSynonymsService();

            //Send data to the service 
            var message = new ValueSet();
            message.Add("Command", "GetSynonym");
            message.Add("Term", term);

            //Send a message  
            AppServiceResponse response = await synonymsServiceConnection.SendMessageAsync(message);
            if (response.Status == AppServiceResponseStatus.Success)
            {
                if (response.Message.ContainsKey("Result"))
                {
                    synonyms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((string)response.Message["Result"]);
                }
                else if (response.Message.ContainsKey("Error"))
                {
                    throw new Exception("Error:" + response.Message["Error"]);
                }
            }

            callresponse.Status = response.Status;
            callresponse.Synonyms = synonyms;

            return callresponse;
        }

        private async System.Threading.Tasks.Task EnsureConnectionToSynonymsService()
        {
            if (this.synonymsServiceConnection == null)
            {
                synonymsServiceConnection = new AppServiceConnection();

                // See the appx manifest of the AppServicesDemp app for this value
                synonymsServiceConnection.AppServiceName = "MicrosoftDX-SynonymsService";
                // Use the Windows.ApplicationModel.Package.Current.Id.FamilyName API in the 
                // provider app to get this value
                synonymsServiceConnection.PackageFamilyName = "82a987d5-4e4f-4cb4-bb4d-700ede1534ba_nsf9e2fmhb1sj";

                AppServiceConnectionStatus connectionStatus = await synonymsServiceConnection.OpenAsync();
                if (connectionStatus == AppServiceConnectionStatus.Success)
                {
                    synonymsServiceConnection.ServiceClosed += (s, serviceClosedEventArgs) =>
                    {
                        if (ServiceClosed != null)
                        {
                            ServiceClosed(this, serviceClosedEventArgs);
                        }
                    };
                }
                else
                {
                    //Drive the user to store to install the app that provides 
                    //the app service 
                    throw new NotImplementedException("Service not installed on this device");
                }
            }
        }

        public async Task CloseSynonymsServiceAsync()
        {
            await EnsureConnectionToSynonymsService();

            //Send data to the service 
            var message = new ValueSet();
            message.Add("Command", "Quit");

            //Send a message  
            await synonymsServiceConnection.SendMessageAsync(message);

            ClearSynonymServiceConnection();
        }
        private void ClearSynonymServiceConnection()
        {
            this.synonymsServiceConnection.Dispose();
            this.synonymsServiceConnection = null;
        }
    }
}
