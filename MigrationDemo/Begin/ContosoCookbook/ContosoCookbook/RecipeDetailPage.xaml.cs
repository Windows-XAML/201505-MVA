using ContosoCookbook.Common;
using ContosoCookbook.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Pivot Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoCookbook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecipeDetailPage : Page
    {
        private RecipeDataItem item;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private FileOpenPickerContinuationEventArgs _filePickerEventArgs = null;
        public FileOpenPickerContinuationEventArgs FilePickerEvent
        {
            get { return _filePickerEventArgs; }
            set
            {
                _filePickerEventArgs = value;
                StorageFile pickedFile = null;
                if (_filePickerEventArgs.Files.Count > 0)
                {
                    pickedFile = _filePickerEventArgs.Files[0];
                }
                FileOpenPicker_Continuation(pickedFile);
            }
        }

        public RecipeDetailPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            item = await RecipeDataSource.GetItemAsync((String)e.NavigationParameter);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Item"] = item;

            // Is recipe already pinned?
            if (SecondaryTile.Exists(item.UniqueId))
                btnPinToStart.Icon = new SymbolIcon(Symbol.UnPin);

            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        private async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            var deferral = args.Request.GetDeferral();

            try
            {
                Uri photoFileUri = new Uri(new Uri("ms-appdata:///local/"), item.UserPhotos[0].Title);
                var photoFile = await StorageFile.GetFileFromApplicationUriAsync(photoFileUri);

                request.Data.Properties.Title = "I've been baking!";
                request.Data.Properties.Description = "This was my attempt at making " + item.ShortTitle;

                // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
                // since the target app may only support one or the other.
                List<IStorageItem> imageItems = new List<IStorageItem>();
                imageItems.Add(photoFile);
                request.Data.SetStorageItems(imageItems);

                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(photoFile);
                // It is recommended that you always add a thumbnail image any time you're sharing an image
                request.Data.Properties.Thumbnail = imageStreamRef;
                request.Data.SetBitmap(imageStreamRef);

                // Set Text to share for those targets that can't accept images
                request.Data.SetText("I just made " + item.ShortTitle + " from Contoso Cookbook!");
            }
            finally
            {
                deferral.Complete();
            }
        }


        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void btnPinToStart_Click(object sender, RoutedEventArgs e)
        {
            // Is recipe already pinned?
            if (SecondaryTile.Exists(item.UniqueId))
            {
                var tile = new SecondaryTile(item.UniqueId);
                await tile.RequestDeleteAsync();
                btnPinToStart.Icon = new SymbolIcon(Symbol.Pin);
            }
            else
            {
                var uri = new Uri(item.TileImagePath.AbsoluteUri);

                var tile = new SecondaryTile(
                        item.UniqueId,              // Tile ID
                        item.ShortTitle,            // Tile short name
                        item.UniqueId,              // Activation argument
                        uri,                        // Tile logo URI
                        TileSize.Square150x150
                    );

                await tile.RequestCreateAsync();
                btnPinToStart.Icon = new SymbolIcon(Symbol.UnPin);
            }
        }

        private async void btnReminderTimer_Click(object sender, RoutedEventArgs e)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            // Make sure notifications are enabled
            if (notifier.Setting != NotificationSetting.Enabled)
            {
                var dialog = new MessageDialog("Notifications are currently disabled");
                await dialog.ShowAsync();
                return;
            }

            // Get a toast template and insert a text node containing a message
            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var element = template.GetElementsByTagName("text")[0];
            element.AppendChild(template.CreateTextNode("Reminder!"));

            // Schedule the toast to appear 30 seconds from now
            var date = DateTimeOffset.Now.AddSeconds(30);
            var stn = new ScheduledToastNotification(template, date);
            notifier.AddToSchedule(stn);
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            if (item.UserPhotos == null || item.UserPhotos.Count == 0)
            {
                var i = new MessageDialog("You must insert a picture first!").ShowAsync();
            }
            else
            {
                DataTransferManager.ShowShareUI();
            }
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
#if WINDOWS_APP
            StorageFile file = await openPicker.PickSingleFileAsync();
            FileOpenPicker_Continuation(file);
#elif WINDOWS_PHONE_APP
            // Call the FileOpenPicker in Windows Phone mode
            openPicker.PickSingleFileAndContinue();
#endif
        }
        private async void FileOpenPicker_Continuation(StorageFile file)
        {
            if (file != null)
            {
                var destFile = await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                var userPhoto = new UserPhotoDataItem() { Title = destFile.Name};
                userPhoto.SetImage(destFile.Path);
                item.UserPhotos.Add(userPhoto);
            }
        }
    }
}
