using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace AppServicesDemoTask
{
    // Background task for an App Service
    public sealed class AppServiceTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _serviceDeferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += TaskInstance_Canceled;

            // Get the deferral object from the task instance 
            _serviceDeferral = taskInstance.GetDeferral();

            var appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (appService != null &&
                appService.Name == "microsoftDX-appservicesdemo")
            {
                //ValidateCaller(appService.CallerPackageFamilyName);
                appService.AppServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
            }
        }

        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string command = message["Command"] as string;

            switch (command)
            {
                case "CalcSum":
                    {
                        var messageDeferral = args.GetDeferral();

                        int value1 = (int)message["Value1"];
                        int value2 = (int)message["Value2"];

                        //Set a result to return to the caller 
                        int result = value1 + value2;
                        var returnMessage = new ValueSet();
                        returnMessage.Add("Result", result);
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
