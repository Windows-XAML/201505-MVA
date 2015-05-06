using System;
using Windows.UI.Xaml.Media.Imaging;

namespace TODOFilePickerSample.Models
{
    public class TodoItem : Mvvm.BindableBase
    {
        private string _Key = default(string);
        public string Key { get { return _Key; } set { Set(ref _Key, value); } }

        private string _Title = default(string);
        public string Title { get { return _Title; } set { Set(ref _Title, value); } }

        private DateTime _DueDate = default(DateTime);
        public DateTime DueDate { get { return _DueDate; } set { Set(ref _DueDate, value); } }

        private bool _IsComplete = default(bool);
        public bool IsComplete { get { return _IsComplete; } set { Set(ref _IsComplete, value); } }

        private string _Details = default(string);
        public string Details { get { return _Details; } set { Set(ref _Details, value); } }

        private bool _IsFavorite = default(bool);
        public bool IsFavorite { get { return _IsFavorite; } set { Set(ref _IsFavorite, value); } }

        private Uri _ImageUri = default(Uri);
        public Uri ImageUri { get { return _ImageUri; }
            set
            {
                if (!object.Equals(_ImageUri, value))
                {
                    SetImage(value);
                }
                Set(ref _ImageUri, value);
            }
        }

        private BitmapImage _ImageSource;
        public BitmapImage ImageSource { get { return _ImageSource; } set { Set(ref _ImageSource, value); } }

        private async void SetImage(Uri targetImageUri)
        {
            if (targetImageUri == null)
            {
                ImageSource = null;
            }
            else
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(targetImageUri);
                var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var img = new BitmapImage();
                img.SetSource(fileStream);
                ImageSource = img;
            }
        }

    }
}
