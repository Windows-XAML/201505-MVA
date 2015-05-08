using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace Template10.Services.NavigationService
{
    public interface INavigationAware
    {
        void OnNavigating(object sender, NavigatingCancelEventArgs e);
        void OnNavigated(object sender, NavigationEventArgsEx e);
    }
}
