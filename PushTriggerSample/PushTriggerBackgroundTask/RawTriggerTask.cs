using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Notifications;

namespace PushTriggerBackgroundTask
{
    public sealed class RawTriggerTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var _deferral = taskInstance.GetDeferral();

            // Get the background task details
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string taskName = taskInstance.Task.Name;

            Debug.WriteLine("Background " + taskName + " starting...");

            // Store the content received from the notification so it can be retrieved from the UI.
            RawNotification notification = (RawNotification)taskInstance.TriggerDetails;
            settings.Values[taskName] = notification.Content;

            // Pop up a toast to notify the user
            DeliverToast("test", "testid");

            Debug.WriteLine("Background " + taskName + " completed!");
        }

        private void DeliverToast(string todoTitle, string todoID)
        {
            // Send the Toast Notification informing the user of a new message
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(todoTitle));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");

            ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"" + todoID + "\",\"param2\":\"0\"}");

            ToastNotification toast = new ToastNotification(toastXml);

            // Tag the Toast with the data item ID
            // Note that Toasts sent from servers set the Tag through an HTTP Header
            toast.Tag = msgItem.ID;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
