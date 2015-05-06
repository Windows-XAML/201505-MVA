using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODOFilePickerSample.Models;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace TODOFilePickerSample.ViewModels
{
    public class TodoItemViewModel : Mvvm.ViewModelBase
    {
        public TodoItemViewModel(Models.TodoItem todo)
        {
            this.TodoItem = todo;
        }

        private Models.TodoItem _TodoItem = default(Models.TodoItem);
        public Models.TodoItem TodoItem { get { return _TodoItem; } set { Set(ref _TodoItem, value); } }

        bool _busy = false;
        public bool Busy { get { return _busy; } set { Set(ref _busy, value); } }

        #region Commands

        Mvvm.Command _SelectPictureCommand = default(Mvvm.Command);
        public Mvvm.Command SelectPictureCommand { get { return _SelectPictureCommand ?? (_SelectPictureCommand = new Mvvm.Command(ExecuteSelectPictureCommand, CanExecuteSelectPictureCommand)); } }
        private bool CanExecuteSelectPictureCommand() { return !Busy; }
        private async void ExecuteSelectPictureCommand()
        {
            try
            {
                Busy = true;

                
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");

                StorageFile file = await openPicker.PickSingleFileAsync();

                if (file != null)
                {
                    // Copy the file into local folder
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    // Save in the ToDoItem
                    TodoItem.ImageUri = new Uri("ms-appdata:///local/" + file.Name);
                }
            }
            finally { Busy = false; }
        }


        Mvvm.Command _TakePictureCommand = default(Mvvm.Command);
        public Mvvm.Command TakePictureCommand { get { return _TakePictureCommand ?? (_TakePictureCommand = new Mvvm.Command(ExecuteTakePictureCommand, CanExecuteSelectPictureCommand)); } }
        private bool CanExecuteTakePictureCommand() { return !Busy && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Media.Capture.CameraCaptureUI"); }
        private async void ExecuteTakePictureCommand()
        {
            try
            {
                Busy = true;

                StorageFile file = null;

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Media.Capture.CameraCaptureUI"))
                {
                    // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
                    CameraCaptureUI dialog = new CameraCaptureUI();
                    Size aspectRatio = new Size(16, 9);
                    dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;

                    file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                }

                if (file != null)
                {
                    // Copy the file into local folder
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    // Save in the ToDoItem
                    TodoItem.ImageUri = new Uri("ms-appdata:///local/" + file.Name);
                }
            }
            finally { Busy = false; }
        }

        #endregion
    }
}
