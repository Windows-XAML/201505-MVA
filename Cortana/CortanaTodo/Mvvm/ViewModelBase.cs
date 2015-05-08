using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Services;
using Template10.Services.Lifecycle;
using Template10.Services.NavigationService;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;

namespace Template10.Mvvm
{
    public abstract class ViewModelBase : BindableBase, INavigationAware, ILifecycleAware
    {
        public virtual Task HandleResumeAsync(object e)
        {
            // Nothing by default
            return TaskHelper.CompletedTask;
        }

        public virtual Task HandleSuspendAsync(SuspendingEventArgs e)
        {
            // Nothing by default
            return TaskHelper.CompletedTask;
        }

        public virtual void OnNavigating(object sender, NavigatingCancelEventArgs e) { /* nothing by default */ }

        public virtual void OnNavigated(object sender, NavigationEventArgsEx e) { /* nothing by default */ }

    }
}