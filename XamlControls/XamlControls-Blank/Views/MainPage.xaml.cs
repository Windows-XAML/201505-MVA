using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Samples.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Models.MenuItem;
            if (item.Key == 1)
                this.Frame.Navigate(typeof(Views.LayoutControls));
            else if (item.Key == 2)
                this.Frame.Navigate(typeof(Views.InputControls));
            else if (item.Key == 3)
                this.Frame.Navigate(typeof(Views.ItemsControls));
            else if (item.Key == 4)
                this.Frame.Navigate(typeof(Views.GeometryControls));
            else if (item.Key == 4)
                this.Frame.Navigate(typeof(Views.Transforms));
        }
    }
}
