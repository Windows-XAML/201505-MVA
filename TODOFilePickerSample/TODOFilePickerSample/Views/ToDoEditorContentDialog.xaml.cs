using System;
using TODOFilePickerSample.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TODOFilePickerSample.Views
{
    public sealed partial class ToDoEditorContentDialog : ContentDialog
    {
        public ToDoEditorContentDialog()
        {
            this.InitializeComponent();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            // NOte, in DP7 (BUILD tools drop), CameraCaptureUI is only in the Desktop extension SDK.
            // At RTM, it is planned to be in Mobile extension SDK as well
            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Media.Capture.CameraCaptureUI"))
            {
                TakePictureButton.Visibility = Visibility.Collapsed;
            }
            base.OnGotFocus(e);
        }

        public event EventHandler<TodoItemViewModel> DeleteTodoItemClicked;

        private void DeleteItemClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DeleteTodoItemClicked != null)
            {
                DeleteTodoItemClicked(this, (TodoItemViewModel)this.DataContext);
            }
        }
    }
}
