using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Template10
{
    public static class BackgroundHelper
    {
        public static IBackgroundTaskRegistration FindRegistration<T>() where T : class
        {
            return BackgroundTaskRegistration.AllTasks
                .Where(x => x.Value.Name.Equals(nameof(T)))
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        public async static Task<BackgroundTaskRegistration> Register<T>(IBackgroundTrigger trigger, IEnumerable<IBackgroundCondition> conditions = null) where T : class
        {
            await BackgroundExecutionManager.RequestAccessAsync();
            var allowed = BackgroundExecutionManager.GetAccessStatus();
            switch (allowed)
            {
                case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                    break;
                case BackgroundAccessStatus.Unspecified:
                case BackgroundAccessStatus.Denied:
                    return null;
            }

            var existing = FindRegistration<T>();
            if (existing != null)
                existing.Unregister(false);

            var task = new BackgroundTaskBuilder
            {
                Name = nameof(MyUpdateBadgeTask),
                CancelOnConditionLoss = false,
                TaskEntryPoint = typeof(T).ToString(),
            };

            task.SetTrigger(trigger);
            if (conditions != null)
            {
                foreach (var condition in conditions)
                    task.AddCondition(condition);
            }
            return task.Register();
        }

        public async static Task<bool> Unregister<T>() where T : class
        {
            await BackgroundExecutionManager.RequestAccessAsync();
            var allowed = BackgroundExecutionManager.GetAccessStatus();
            switch (allowed)
            {
                case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                    break;
                case BackgroundAccessStatus.Unspecified:
                case BackgroundAccessStatus.Denied:
                    return false;
            }

            var existing = FindRegistration<T>();
            if (existing != null)
                existing.Unregister(false);
            return true;
        }
    }
}
