using SynonymsService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            };
        }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
      //// Call the synonyms service
      //SynonymApi api = new SynonymApi("kv/Q1cvCsXV1kf+kTshGAp1JOobmT6AsvEO4prq6zb4=");

      //  var synonyms = await api.GetSynonymsAsync("seatac");

      //  if ((synonyms != null) && (synonyms.Count() > 0))
      //  {
      //    //Set a result to return to the caller 
      //    new MessageDialog("Result =" + synonyms.First()).ShowAsync();
      //  }
     }
  }
}
