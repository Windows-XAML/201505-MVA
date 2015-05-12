using Windows.UI.Xaml.Controls;

namespace Blank1.Views
{
    public sealed partial class Page2 : Page
    {
        public Page2()
        {
            this.InitializeComponent();
        }

        public void GoBack()
        {
            (App.Current as Common.BootStrapper).NavigationService.GoBack();
        }
    }
}
