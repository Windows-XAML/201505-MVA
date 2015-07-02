using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;

namespace TextEncoderComponent
{
    public sealed class EncoderDecoderTask : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //Take a service deferral so the service isn't terminated
            serviceDeferral = taskInstance.GetDeferral();

            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += TaskInstance_Canceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null)
            {
                //Listen for incoming app service requests
                triggerDetails.AppServiceConnection.RequestReceived += OnRequestReceived;
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            throw new NotImplementedException();
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {

            var message = args.Request.Message;
            string cmd = message["cmd"] as string;
            string txt = message["txt"] as string;

            switch (cmd)
            {
                case "Encode":
                    {
                        var messageDeferral = args.GetDeferral();

                        string result = Encode(txt);
                        var returnMessage = new ValueSet();
                        returnMessage.Add("result", result);
                        var responseStatus = await args.Request.SendResponseAsync(returnMessage);

                        messageDeferral.Complete();
                        break;
                    }

                case "Decode":
                    {
                        var messageDeferral = args.GetDeferral();

                        string result = Decode(txt);
                        var returnMessage = new ValueSet();
                        returnMessage.Add("result", result);
                        var responseStatus = await args.Request.SendResponseAsync(returnMessage);

                        messageDeferral.Complete();
                        break;
                    }
                case "Quit":
                    {
                        serviceDeferral.Complete();
                        break;
                    }
            }
        }
        public string Encode(string txt)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(txt, BinaryStringEncoding.Utf8);
            return CryptographicBuffer.EncodeToBase64String(buffer);
        }

        public string Decode(string encodedString)
        {
            var buffer = CryptographicBuffer.DecodeFromBase64String(encodedString);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffer);
        }
    }
}
