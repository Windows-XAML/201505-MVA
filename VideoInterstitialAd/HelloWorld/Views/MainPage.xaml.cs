using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Template10.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            ViewModel = DataContext as ViewModels.MainPageViewModel;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar= true;
            Window.Current.SetTitleBar(AppTitle);
        }

        ViewModels.MainPageViewModel ViewModel { get; set; }

        private async void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            TodoEditorDialog.DataContext = e.ClickedItem;
            await TodoEditorDialog.ShowAsync();
        }

        private async void List_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var data = textBlock.DataContext as ViewModels.TodoListViewModel;
            ListEditorDialog.DataContext = data.TodoList;
            ListEditorDialog.SecondaryButtonCommand = ViewModel.RemoveListCommand;
            ListEditorDialog.SecondaryButtonCommandParameter = data;
            await ListEditorDialog.ShowAsync();
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Key == Windows.System.VirtualKey.Enter 
                && !string.IsNullOrEmpty(textBox.Text)
                && textBox.Text.Length > 3)
            {
                e.Handled = true;
                var list = textBox.DataContext as ViewModels.TodoListViewModel;
                list.AddCommand.Execute(textBox.Text);
                textBox.Text = string.Empty;
                textBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }
    }
}
