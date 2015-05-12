using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace MyBack
{
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // test cost
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            switch (cost)
            {
                case BackgroundWorkCostValue.Low:
                case BackgroundWorkCostValue.Medium:
                    break;
                case BackgroundWorkCostValue.High:
                    return;
            }

            // setup cancel
            var cancel = false;
            taskInstance.Canceled += (s, e) => { cancel = true; };

            // run
            var deferral = taskInstance.GetDeferral();
            try
            {
                // retrieve arguments
                var details = taskInstance.TriggerDetails as ApplicationTriggerDetails;
                var arguments = details.Arguments as ValueSet;
                var argString = arguments["Argument"].ToString();
                var argInt = int.Parse(argString);

                // run operation
                await Task.Run(async () =>
                {
                    Helper.SendToast("Starting now");
                    for (int i = 0; i < argInt; i++)
                    {
                        // test for cancel
                        if (cancel)
                        {
                            Helper.SendToast("Canceled");
                            return;
                        }

                        // update progress
                        taskInstance.Progress = (uint)i;

                        // simulate wait
                        await Task.Delay(100);

                        // show toast just for a few
                        if (i % 10 == 0)
                        {
                            Helper.SendToast(string.Format("Number is {0}", i));
                        }
                    }
                    Helper.SendToast("All done");
                });
            }
            catch { /* TODO */ }
            finally { deferral.Complete(); }
        }
    }
}
