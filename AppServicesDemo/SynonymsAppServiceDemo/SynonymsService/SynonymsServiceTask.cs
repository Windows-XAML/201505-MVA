using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace SynonymsService
{
  // Background task for an App Service
  public sealed class SynonymsServiceTask : IBackgroundTask
  {
    private static BackgroundTaskDeferral _serviceDeferral;

    //#error You need a bing api key
    static readonly string BING_KEY = "kv/Q1cvCsXV1kf+kTshGAp1JOobmT6AsvEO4prq6zb4=";

    public void Run(IBackgroundTaskInstance taskInstance)
    {
      // Associate a cancellation handler with the background task.
      taskInstance.Canceled += TaskInstance_Canceled;

      // Get the deferral object from the task instance 
      _serviceDeferral = taskInstance.GetDeferral();

      var appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
      if (appService != null &&
          appService.Name == "MicrosoftDX-SynonymsService")
      {
        //ValidateCaller(appService.CallerPackageFamilyName);
        appService.AppServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
      }
    }

    private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
      var message = args.Request.Message;

      string command = (string)message["Command"];

      switch (command)
      {
        case "GetSynonym":
          {
            var messageDeferral = args.GetDeferral();

            string term = (string)message["Term"];

            // Call the synonyms service
            SynonymApi api = new SynonymApi(BING_KEY);

            var returnMessage = new ValueSet();

            try
            {
              var synonyms = await api.GetSynonymsAsync(term);

              if ((synonyms != null) && (synonyms.Count() > 0))
              {
                //Set a result to return to the caller
                //Serialize the IEnumerable<string> to Json so we can insert into ValueSet
                returnMessage.Add("Result", Newtonsoft.Json.JsonConvert.SerializeObject(synonyms));
              }
              else
              {
                // Report an error back to the caller
                returnMessage.Add("Error", "No results found");
              }
            }
            catch (Exception ex)
            {
              // Error accessing the service
              // Report an error back to the caller
              returnMessage.Add("Error", "Synonyms Service not available " + ex.Message + " term=" + term);
            }

            var responseStatus = await args.Request.SendResponseAsync(returnMessage);
            messageDeferral.Complete();
            break;
          }

        case "Quit":
          {
            //Service was asked to quit. Give us service deferral 
            //so platform can terminate the background task 
            _serviceDeferral.Complete();
            break;
          }
      }
    }

    private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
    {
      // Maybe do something...
    }
  }
}
