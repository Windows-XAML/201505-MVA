using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

namespace PushTriggerSample.Helpers
{
    public static class PushHandlingHelper
    {
        public static async Task<string> RegisterForPushAsync()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += channel_PushNotificationReceived;
            return channel.Uri.ToString();
        }

        static void channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            string result = args.NotificationType.ToString();
            switch (args.NotificationType)
            {
                case PushNotificationType.Badge:
                    result += ": " + args.BadgeNotification.Content.GetXml();
                    break;
                case PushNotificationType.Raw:
                    result += ": " + args.RawNotification.Content;
                    break;
                case PushNotificationType.Tile:
                    result += ": " + args.TileNotification.Content.GetXml();
                    break;
                case PushNotificationType.TileFlyout:
                    result += ": " + args.TileNotification.Content.GetXml();
                    break;
                case PushNotificationType.Toast:
                    result += ": " + args.ToastNotification.Content.GetXml();
                    break;
                default:
                    break;
            }
        }
    }
}
