using System;
using System.Threading.Tasks;
using Template10.Common;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Template10
{
    sealed partial class App : Common.BootStrapper
    {
        public App() : base()
        {
            this.InitializeComponent();
        }

        public override Task OnInitializeAsync()
        {
            // use splitview shell
            Window.Current.Content = new Views.Shell(this.RootFrame);
            return base.OnInitializeAsync();
        }

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            this.NavigationService.Navigate(typeof(Views.MainPage));
            return Task.FromResult<object>(null);
        }
    }
}
