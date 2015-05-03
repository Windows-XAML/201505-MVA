using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace ActionCenterDemo
{
    sealed partial class App : Common.ApplicationBase
    {
        public App() : base()
        {
            this.InitializeComponent();
        }

        protected override Task OnLaunchedAsync(LaunchActivatedEventArgs e)
        {
            if (e.Arguments == string.Empty)
            {
                // first launch navigation
                this.NavigationService.Navigate(typeof(Views.MainPage));
            }
            else
            {
                dynamic d = Newtonsoft.Json.Linq.JObject.Parse(e.Arguments);
                if (d.type == "toast")
                {
                    this.NavigationService.Navigate(typeof(Views.DetailPage), (string)d.param1);
                }
            }
            return base.OnLaunchedAsync(e);
        }
    }
}
