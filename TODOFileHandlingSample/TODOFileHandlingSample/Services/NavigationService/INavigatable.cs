using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace TODOFileHandlingSample.Services.NavigationService
{
    public interface INavigatable
    {
        Task OnNavigatedToAsync(string parameter, NavigationMode mode, Dictionary<string, object> state);
        Task OnNavigatedFromAsync(Dictionary<string, object> state, bool suspending);
    }
}
