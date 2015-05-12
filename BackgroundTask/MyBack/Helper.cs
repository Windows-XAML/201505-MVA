using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace MyBack
{
    public static class Helper
    {
public static void SendToast(string message)
{
    // build toast
    var template = ToastTemplateType.ToastText01;
    var xml = ToastNotificationManager.GetTemplateContent(template);
    var elements = xml.GetElementsByTagName("text");
    var text = xml.CreateTextNode(message);
    elements[0].AppendChild(text);
    var toast = new ToastNotification(xml);
    ToastNotificationManager.CreateToastNotifier().Show(toast);

}

        public static void UpdateBadge(int content)
        {
            // build badge
            var type = BadgeTemplateType.BadgeNumber;
            var xml = BadgeUpdateManager.GetTemplateContent(type);
            var elements = xml.GetElementsByTagName("badge");
            var element = elements[0] as Windows.Data.Xml.Dom.XmlElement;
            element.SetAttribute("value", content.ToString());

            // send to lock screen
            var updator = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            var notification = new BadgeNotification(xml);
            updator.Update(notification);
        }
    }
}
