using Windows.ApplicationModel.Background;

namespace MyBack
{
    public sealed class TimeZoneChangeTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Helper.SendToast("Time zone changed");
        }
    }
}
