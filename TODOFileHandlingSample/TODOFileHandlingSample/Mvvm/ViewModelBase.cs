using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace TODOFileHandlingSample.Mvvm
{
    public abstract class ViewModelBase : BindableBase, Services.NavigationService.INavigatable
    {
        public virtual Task OnNavigatedToAsync(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            return Task.FromResult<object>(null);
        }

        public virtual Task OnNavigatedFromAsync(Dictionary<string, object> state, bool suspending)
        {
            return Task.FromResult<object>(null);
        }

    }
}