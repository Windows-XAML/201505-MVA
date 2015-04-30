using System;
using Windows.UI.Xaml.Controls;

namespace CortanaTodo.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            //this.TodoEditorDialog.DataContext = e.ClickedItem;
            //await this.TodoEditorDialog.ShowAsync();
        }
    }
}
