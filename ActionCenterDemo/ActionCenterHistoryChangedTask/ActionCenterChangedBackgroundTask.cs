using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace ActionCenterHistoryChangedTask
{
    // Background task for a ToastNotificationHistoryChangedTrigger
    public sealed class ActionCenterChangedBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var toasts = ToastNotificationManager.History.GetHistory();
            if (toasts != null)
            {
                var count = toasts.Count();

                XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
                badgeElement.SetAttribute("value", count.ToString());

                BadgeNotification badge = new BadgeNotification(badgeXml);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

                // Update the app data
                UpdateAppData(toasts);

                taskInstance.Progress = 100;
            }
        }

        private static void UpdateAppData(IReadOnlyList<ToastNotification> toasts)
        {
            // Update the apps data as well
            var messageItemRepository = ActionCenterDemo.Repositories.MessageItemRepository.GetInstance();
            // Set all to 'Read' initially
            foreach (var messageItem in messageItemRepository.GetAllMessageItems())
            {
                messageItem.IsRead = true;
            }
            // Then reset each one still in the list to 'Unread'
            foreach (var item in toasts)
            {
                var launchAttr = item.Content.SelectSingleNode("/toast/@launch").NodeValue.ToString();
                dynamic d = Newtonsoft.Json.Linq.JObject.Parse(launchAttr);
                var msgID = (string)d.param1;

                var messageItem = messageItemRepository.Find(p => p.ID == msgID).FirstOrDefault();
                if (messageItem != null)
                {
                    messageItem.IsRead = false;
                }
            }

            // Store changes
            messageItemRepository.SaveChanges();
        }
    }
}
