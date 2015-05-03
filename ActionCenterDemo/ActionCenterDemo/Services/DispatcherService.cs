using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCenterDemo.Services
{
    public static class DispatchService
    {
        public static async void InvokeAsync(Action action)
        {
            var dispatchObject = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            if (dispatchObject == null || dispatchObject.HasThreadAccess)
            {
                action();
            }
            else
            {
                await dispatchObject.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => action.Invoke());
            }
        }
    }
}
