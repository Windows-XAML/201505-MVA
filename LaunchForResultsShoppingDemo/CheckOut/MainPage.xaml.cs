
namespace CheckOut
{
    using Windows.ApplicationModel;
    using Windows.UI.Xaml.Controls;

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine(Package.Current.Id.FamilyName);
        }
    }
}
