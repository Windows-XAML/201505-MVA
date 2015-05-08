using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Template10.Services.NavigationService
{
    public class NavigationEventArgsEx : EventArgs
    {
        public NavigationEventArgsEx()
        {

        }

        public NavigationEventArgsEx(NavigationEventArgs e)
        {
            this.NavigationMode = e.NavigationMode;
            this.Parameter = e.Parameter as string;
        }

        public NavigationMode NavigationMode { get; set; }

        public string Parameter { get; set; }
    }
}
