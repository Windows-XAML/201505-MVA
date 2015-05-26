using System;
using ActionCenterDemo.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
using Windows.UI.Notifications;
using ActionCenterDemo.Services;

namespace ActionCenterDemo.ViewModels
{
    public class DetailPageViewModel : Common.ViewModelBase
    {
        private Repositories.MessageItemRepository _repository;

        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                LoadDesignData();
        }

        public override async void OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            Debug.WriteLine("In DetailPageViewModel OnNavigatedTo");
            _repository = Repositories.MessageItemRepository.GetInstance();
            await LoadRuntimeDataAsync(parameter);

            // Since we are viewing this item, set its status to 'Read'
            this.MessageItem.IsRead = true;
            await _repository.UpdateAsync(this.MessageItem);

            #region Remove corresponding item from Action Center
            // Also remove it from the Action Center if it is there
            //ToastNotificationManager.History.Remove(parameter);
            #endregion

            #region Sync Tile badge count with unread toasts count from Action Center 
            //// Set the Badge count on the tile
            //var toasts = ToastNotificationManager.History.GetHistory();
            //if (toasts != null)
            //{
            //    var count = toasts.Count();
            //    // Sync up the count on the tile
            //    TileServices.SetBadgeCountOnTile(count);
            //}
            #endregion
        }

        private void LoadDesignData()
        {
            this.MessageItem = new Models.MessageItem { ID = "1", Body = "Design Data Body", Title = "Design Title", IsRead = false };
        }

        private async Task LoadRuntimeDataAsync(string parameter)
        {
            _repository.GetAllMessageItems();
            this.MessageItem = _repository.Find(m => m.ID == parameter).FirstOrDefault();
        }

        private Models.MessageItem _messageItem;
        public Models.MessageItem MessageItem
        {
            get { return _messageItem; }
            set
            {
                Set(ref _messageItem, value);
            }
        }

        public Common.Command GoBackCommand
        {
            get { return new Common.Command(() => { (App.Current as Common.ApplicationBase).NavigationService.GoBack(); }); }
        }
    }
}
