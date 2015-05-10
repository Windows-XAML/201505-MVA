using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppServicesDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += (s, e) =>
            {
                var currentPackage = Windows.ApplicationModel.Package.Current;
                var packageFamilyName = currentPackage.Id.FamilyName;

                new MessageDialog("PFN = " + packageFamilyName).ShowAsync();
                Debug.WriteLine("PFN = " + packageFamilyName);
            };
        }
    }
}
