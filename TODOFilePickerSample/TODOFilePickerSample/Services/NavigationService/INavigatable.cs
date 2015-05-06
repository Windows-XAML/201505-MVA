using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace TODOFilePickerSample.Services.NavigationService
{
    public interface INavigatable
    {
        void OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state);
        void OnNavigatedFrom(Dictionary<string, object> state, bool suspending);
    }
}
