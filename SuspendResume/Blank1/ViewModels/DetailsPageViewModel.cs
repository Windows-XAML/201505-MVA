using Blank1.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Blank1.ViewModels
{
    class DetailsPageViewModel : ViewModelBase
    {
        public override void OnNavigatedTo(string parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            try
            {
                FirstName = state["FirstName"]?.ToString();
                LastName = state["LastName"]?.ToString();
                Email = state["Email"]?.ToString();
            }
            finally { state.Clear(); }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                state["FirstName"] = FirstName;
                state["LastName"] = LastName;
                state["Email"] = Email;
            }
            return base.OnNavigatedFromAsync(state, suspending);
        }

        private string _FirstName = default(string);
        public string FirstName { get { return _FirstName; } set { Set(ref _FirstName, value); } }

        private string _LastName = default(string);
        public string LastName { get { return _LastName; } set { Set(ref _LastName, value); } }

        private string _Email = default(string);
        public string Email { get { return _Email; } set { Set(ref _Email, value); } }
    }
}
