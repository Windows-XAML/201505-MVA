using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Template10
{
    public sealed class MyUpdateBadgeTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
            {
                Debug.WriteLine("Background task aborted (cost is high)");
            }
            else
            {
                // handle canceled
                taskInstance.Canceled += (s, e) =>
                {
                    Debug.WriteLine("Background task canceled");
                };

                // perform task
                var deferral = taskInstance.GetDeferral();
                try
                {
                    var seed = (int)DateTime.Now.Ticks;
                    var random = new Random(seed);
                    var value = random.Next(1, 50);
                    UpdateTile(value);

                    Debug.WriteLine("Background task complete: " + value.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Background task error: " + ex.Message);
                }
                finally
                {
                    deferral.Complete();
                }

            }
        }

        public static void UpdateTile(int value)
        {
            var type = BadgeTemplateType.BadgeNumber;
            var xml = BadgeUpdateManager.GetTemplateContent(type);
            var elements = xml.GetElementsByTagName("badge");
            var element = elements[0] as XmlElement;
            element.SetAttribute("value", value.ToString());

            var updater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            var notification = new BadgeNotification(xml);
            updater.Update(notification);

            Debug.WriteLine("Background task badge updated: " + value.ToString());

            var template = ToastTemplateType.ToastText01;
            xml = ToastNotificationManager.GetTemplateContent(template);
            var text = xml.CreateTextNode(string.Format("Badge updated to {0}", value));
            elements = xml.GetElementsByTagName("text");
            elements[0].AppendChild(text);

            var toast = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);

            Debug.WriteLine("Background task toast shown: " + value.ToString());
        }
    }
}
