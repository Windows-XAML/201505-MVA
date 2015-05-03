using ActionCenterDemo.Models;
using ActionCenterDemo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Navigation;

namespace ActionCenterDemo.ViewModels
{
    class MainPageViewModel : Common.ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadDesigntimeData();
            }
            else
            {
                this.PropertyChanged += (s, e) => { this.RefreshCommand.RaiseCanExecuteChanged(); };
            }
        }

        public override async void OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            Debug.WriteLine("In MainPageViewModel OnNavigatedTo");
            await LoadRuntimeDataAsync();
            IsTaskRegistered();
        }

        private void LoadDesigntimeData()
        {
            this.MessageItems.Clear();
            this.MessageItems.Add(new Models.MessageItem { ID = "1", Body = "Design Data Body", Title = "Design Title", IsRead = false });
            this.MessageItems.Add(new Models.MessageItem { ID = "2", Body = "Design Data Body 2", Title = "Design Title 2", IsRead = true });
            this.MessageItems.Add(new Models.MessageItem { ID = "3", Body = "Design Data Body 3", Title = "Design Title 3", IsRead = false });
            this.MessageItems.Add(new Models.MessageItem { ID = "4", Body = "Design Data Body 4", Title = "Design Title 4", IsRead = true });

            this.Selected = (this.MessageItems.Any()) ? MessageItems.First() : default(Models.MessageItem);
        }

        private Repositories.MessageItemRepository _repository;

        private async Task LoadRuntimeDataAsync()
        {
            _repository = Repositories.MessageItemRepository.GetInstance();
            _repository.LoadData();
            this.MessageItems.Clear();
            foreach (var messageItem in _repository.GetAllMessageItems())
            {
                this.MessageItems.Add(messageItem);
            }

            // Set the count on the simulated tile 
            int unreadCount = (from m in this.MessageItems
                                where !m.IsRead
                                select m).Count();
            this.TileSimulationBadgeCount = unreadCount;
        }

        private Models.MessageItem _selected;
        public Models.MessageItem Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
                if (value != null)
                {
                    this.Selected = null;
                    // navigate to details page
                    if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                    {
                        var nav = (App.Current as App).NavigationService;
                        nav.Navigate(typeof(Views.DetailPage), value.ID);
                    }
                }
            }
        }

        public ObservableCollection<Models.MessageItem> MessageItems { get; } = new ObservableCollection<Models.MessageItem>();

        private bool _busy;
        public bool Busy { get { return _busy; } set { Set(ref _busy, value); } }

        private int _tileSimulationBadgeCount;
        public int TileSimulationBadgeCount { get { return _tileSimulationBadgeCount; } set { Set(ref _tileSimulationBadgeCount, value); } }

        private Common.Command _refreshCommand;
        public Common.Command RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Common.Command(async () =>
                {
                    this.Busy = true;
                    await LoadRuntimeDataAsync();
                    this.Busy = false;
                }, () => { return !this.Busy; }));
            }
        }

        private Common.Command _deliverToastCommand;
        public Common.Command DeliverToastCommand
        {
            get
            {
                return _deliverToastCommand ?? (_deliverToastCommand = new Common.Command(async () =>
                {
                    this.Busy = true;
                    // Generate a new message data item - this simulates one that has come from your cloud backend
                    var msgItem = new MessageItem()
                    {
                        ID = Guid.NewGuid().ToString().Split('-')[0].ToString(),
                        Title = "Hello Msg #" + System.Environment.TickCount,
                        Body = "This is a new message created at " + System.DateTime.Now.ToString("t"),
                        IsRead = false
                    };

                    // Send a toast notification to alert the user of the new item
                    DeliverToast(msgItem);
                    // Update the Main Tile
                    SetBadgeCountOnTileandSim();

                    // Save the new messageItem to our local storage
                    await _repository.AddAsync(msgItem);

                    // Update our list of MessageItems shown on our UI
                    await LoadRuntimeDataAsync();

                    this.Busy = false;
                }, () => { return !this.Busy; }));
            }
        }

        private void DeliverToast(MessageItem msgItem)
        {
            // Send the Toast Notification informing the user of a new message
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(msgItem.Title));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");

            ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"" + msgItem.ID + "\",\"param2\":\"0\"}");

            ToastNotification toast = new ToastNotification(toastXml);

            // Tag the Toast with the data item ID
            // Note that Toasts sent from servers set the Tag through an HTTP Header
            toast.Tag = msgItem.ID;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private Common.Command _resetAllCountsCommand;

        /// <summary>
        /// Gets the ToastNotificationManager History and ensures the badge count
        /// on the Tile simulation on the UI and on the actual Tile matches
        /// the true count of unread notifications in Action Center 
        /// </summary>
        public Common.Command ResetAllCountsCommand
        {
            get
            {
                return _resetAllCountsCommand ?? (_resetAllCountsCommand = new Common.Command(async () =>
                {
                    this.Busy = true;

                    // Set count on Tile and on our Simulation
                    SetBadgeCountOnTileandSim();

                    // Set 'Read' status of local data items to match unactioned Notifications
                    foreach (var item in MessageItems)
                    {
                        item.IsRead = true;
                    }
                    var toasts = ToastNotificationManager.History.GetHistory();
                    if (toasts != null)
                    {
                        foreach (var item in toasts)
                        {
                            var launchAttr = item.Content.SelectSingleNode("/toast/@launch").NodeValue.ToString();
                            dynamic d = Newtonsoft.Json.Linq.JObject.Parse(launchAttr);
                            var msgID = (string)d.param1;
                            var dataItem = MessageItems.Where(p => p.ID == msgID).SingleOrDefault();
                            if (dataItem != null)
                            {
                                dataItem.IsRead = false;
                            }
                        }
                    }
                    await LoadRuntimeDataAsync();
                    this.Busy = false;
                }, () => { return !this.Busy; }));
            }
        }

        private void SetBadgeCountOnTileandSim()
        {
            // Get the toast history from Notification Center
            var toasts = ToastNotificationManager.History.GetHistory();
            if (toasts != null)
            {
                var count = toasts.Count();

                // Sync up the count on the tile
                TileServices.SetBadgeCountOnTile(count);

                //...and on our simulation on the UI
                this.TileSimulationBadgeCount = count;
            }
        }

        #region REGISTER_BG_TASK
        private Common.Command _registerBGTaskCommand;

        public Common.Command RegisterBGTaskCommand
        {
            get
            {
                return _registerBGTaskCommand ?? (_registerBGTaskCommand = new Common.Command(async () =>
                {
                    this.Busy = true;
                    await Register();
                    this.Busy = false;
                }, () => { return !this.Busy; }));
            }
        }

        private readonly string ActionCenterHistoryChangedTaskName = "ActionCenterHistoryChangedTask";
        public async Task Register()
        {
            if (!IsTaskRegistered())
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = ActionCenterHistoryChangedTaskName;
                builder.TaskEntryPoint = "ActionCenterHistoryChangedTask.ActionCenterChangedBackgroundTask";
                builder.SetTrigger(new ToastNotificationHistoryChangedTrigger());

                BackgroundExecutionManager.RemoveAccess();
                BackgroundAccessStatus x = await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskRegistration mytask = builder.Register();
                mytask.Completed += Mytask_Completed;
            }
        }

        private async void Mytask_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            // Background task has completed - refresh our data
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, 
                async () => await LoadRuntimeDataAsync()
                );
        }

        public bool IsTaskRegistered()
        {
            var taskRegistered = false;
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(
                kvp => kvp.Value.Name == ActionCenterHistoryChangedTaskName);

            if (entry.Value != null)
            {
                taskRegistered = true;

                // Hook up the completed event handler again
                entry.Value.Completed += Mytask_Completed;
            }

            return taskRegistered;
        }

        #endregion
    }
}
