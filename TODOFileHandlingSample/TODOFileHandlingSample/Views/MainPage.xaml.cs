using System;
using TODOFileHandlingSample.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TODOFileHandlingSample.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = this.DataContext as ViewModels.MainPageViewModel;

        }

        ViewModels.MainPageViewModel ViewModel { get; set; }

        private async void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            //this.TodoEditorDialog.DataContext = e.ClickedItem;
            //await this.TodoEditorDialog.ShowAsync();

            var toDoListVM = MainPivot.SelectedItem as TodoListViewModel;

            var itemEditorDialog = new ToDoEditorContentDialog();
            itemEditorDialog.DataContext = e.ClickedItem;
            itemEditorDialog.SecondaryButtonCommand = toDoListVM.RemoveCommand;
            itemEditorDialog.SecondaryButtonCommandParameter = ((TodoItemViewModel)(e.ClickedItem)).TodoItem;
            await itemEditorDialog.ShowAsync();
        }

        private async void List_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var data = textBlock.DataContext as ViewModels.TodoListViewModel;

            this.ListEditorDialog.DataContext = data.TodoList;
            this.ListEditorDialog.SecondaryButtonCommand = this.ViewModel.RemoveListCommand;
            this.ListEditorDialog.SecondaryButtonCommandParameter = data;
            await this.ListEditorDialog.ShowAsync();
        }


        private TextBox NewToDoItemNameTextBox = null;

        private AppBarButton AddNewItemConfirmButton = null;

        private void AddNewItemConfirmButton_Loaded(object sender, RoutedEventArgs e)
        {
            // This button is in a data template, so we can use the Loaded event to get a reference to it
            // You can't get at controls in Data Templates in Item Templates using their name
            AddNewItemConfirmButton = sender as AppBarButton;
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            NewToDoItemNameTextBox = textBox;

            if (!string.IsNullOrEmpty(textBox.Text)
                && textBox.Text.Length > 3)
            {
                if (AddNewItemConfirmButton != null)
                    AddNewItemConfirmButton.IsEnabled = true;

                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    // Handle 'Enter' key for keyboard users
                    if (e.Key == Windows.System.VirtualKey.Enter)
                    {
                        e.Handled = true;
                        CreateNewToDoItem(textBox);
                    }
                }
            }
            else
            {
                if (AddNewItemConfirmButton != null)
                    AddNewItemConfirmButton.IsEnabled = false;
            }
        }

        private void CreateNewToDoItem(TextBox textBox)
        {
            var list = textBox.DataContext as ViewModels.TodoListViewModel;
            list.AddCommand.Execute(textBox.Text);
            textBox.Text = string.Empty;
            textBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);

            if (AddNewItemConfirmButton != null)
                AddNewItemConfirmButton.IsEnabled = false;
        }

        private void AddNewItemConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewToDoItemNameTextBox != null)
            {
                CreateNewToDoItem(NewToDoItemNameTextBox);
            }
        }

        private async void EditListButtonClicked(object sender, RoutedEventArgs e)
        {
            this.ListEditorDialog.DataContext = ((TodoListViewModel)MainPivot.SelectedItem).TodoList;
            await this.ListEditorDialog.ShowAsync();
        }

        private void DeleteListConfirmationButtonClicked(object sender, RoutedEventArgs e)
        {
            var currentList = (TodoListViewModel)MainPivot.SelectedItem;
            var vm = this.DataContext as ViewModels.MainPageViewModel;
            if (vm.RemoveListCommand.CanExecute(currentList))
            {
                vm.RemoveListCommand.Execute(currentList);
            }
        }

        private void DeleteItemClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var toDoItemVM = sender.DataContext as TodoItemViewModel;
            var toDoListVM = MainPivot.SelectedItem as TodoListViewModel;

            if (toDoListVM.RemoveCommand.CanExecute(toDoItemVM.TodoItem))
            {
                toDoListVM.RemoveCommand.Execute(toDoItemVM.TodoItem);
            }
        }
    }
}
