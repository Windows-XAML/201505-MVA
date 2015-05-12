using Blank1.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Blank1.ViewModels
{
    class Page2ViewModel: ViewModelBase
    {
        public Page2ViewModel()
        {
            this.Parameter = "Set from constructor";
        }

        public override void OnNavigatedTo(string parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            this.Parameter = parameter?.ToString() ?? "Empty";
        }

        private string _Parameter = default(string);
        public string Parameter { get { return _Parameter; } set { Set(ref _Parameter, value); } }
    }
}
