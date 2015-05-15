using System;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace MyBackground
{
    public sealed class MyTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            var argument = details?.Argument?.ToString();
            if (!details?.Argument?.Equals("Submit") ?? false)
                return;
            object todoTitle = null;
            if (!details.UserInput.TryGetValue("Title", out todoTitle))
                return;
            var container = ApplicationData.Current.LocalSettings.CreateContainer("Values", ApplicationDataCreateDisposition.Always);
            container.Values.Add(Guid.NewGuid().ToString(), todoTitle?.ToString() ?? "Empty");
            ApplicationData.Current.SignalDataChanged();
        }
    }
}
