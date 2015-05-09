using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Template10.ViewModels
{
    public class MainPageViewModel : Mvvm.ViewModelBase
    {
        Repositories.TodoListRepository _todoListRepository;

        public MainPageViewModel()
        {
            _todoListRepository = new Repositories.TodoListRepository();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime sample data
                var data = _todoListRepository.Sample().Select(x => new ViewModels.TodoListViewModel(x));
                TodoLists = new ObservableCollection<ViewModels.TodoListViewModel>(data);
            }
            else
            {
                // update Commands
                PropertyChanged += (s, e) =>
                {
                    AddListCommand.RaiseCanExecuteChanged();
                    RemoveListCommand.RaiseCanExecuteChanged();
                    RemoveAdsCommand.RaiseCanExecuteChanged();
                    ShowVideoAdCommand.RaiseCanExecuteChanged();
                };
            }
        }

        public override void OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            LoadCommand.Execute(null);
        }

        public override void OnNavigatedFrom(Dictionary<string, object> state, bool suspending)
        {
            SaveCommand.Execute(null);
        }

        bool _busy = false;
        public bool Busy { get { return _busy; } set { Set(ref _busy, value); } }

        bool _showAd = true;
        public bool ShowAd { get { return _showAd; } set { Set(ref _showAd, value); } }

        private ObservableCollection<ViewModels.TodoListViewModel> _TodoLists = new ObservableCollection<TodoListViewModel>();
        public ObservableCollection<ViewModels.TodoListViewModel> TodoLists { get { return _TodoLists; } private set { Set(ref _TodoLists, value); } }

        #region Commands

        Mvvm.Command _AddListCommand = default(Mvvm.Command);
        public Mvvm.Command AddListCommand { get { return _AddListCommand ?? (_AddListCommand = new Mvvm.Command(ExecuteAddListCommand, CanExecuteAddListCommand)); } }
        private bool CanExecuteAddListCommand() { return !Busy; }
        private void ExecuteAddListCommand()
        {
            try
            {
                var item = new ViewModels.TodoListViewModel(_todoListRepository.Factory(title: "New List"));
                TodoLists.Insert(0, item);
                SaveCommand.Execute(null);
            }
            catch { }
        }

        Mvvm.Command<ViewModels.TodoListViewModel> _RemoveListCommand = default(Mvvm.Command<ViewModels.TodoListViewModel>);
        public Mvvm.Command<ViewModels.TodoListViewModel> RemoveListCommand { get { return _RemoveListCommand ?? (_RemoveListCommand = new Mvvm.Command<ViewModels.TodoListViewModel>(ExecuteRemoveListCommand, CanExecuteRemoveListCommand)); } }
        private bool CanExecuteRemoveListCommand(ViewModels.TodoListViewModel list) { return !Busy && list != null; }
        private void ExecuteRemoveListCommand(ViewModels.TodoListViewModel list)
        {
            try
            {
                var index = TodoLists.IndexOf(list);
                TodoLists.Remove(list);
                SaveCommand.Execute(null);
            }
            catch { }
        }

        Mvvm.Command _LoadCommand = default(Mvvm.Command);
        public Mvvm.Command LoadCommand { get { return _LoadCommand ?? (_LoadCommand = new Mvvm.Command(ExecuteLoadCommand, CanExecuteLoadCommand)); } }
        private bool CanExecuteLoadCommand() { return !Busy; }
        private async void ExecuteLoadCommand()
        {
            try
            {
                Busy = true;
                await Task.Delay(2000);
                var data = _todoListRepository.Sample(10).Select(x => new ViewModels.TodoListViewModel(x));
                TodoLists.Clear();
                foreach (var item in data.OrderBy(x => x.TodoList.Title))
                {
                    TodoLists.Add(item);
                }
            }
            finally { Busy = false; }
        }

        Mvvm.Command _SaveCommand = default(Mvvm.Command);
        public Mvvm.Command SaveCommand { get { return _SaveCommand ?? (_SaveCommand = new Mvvm.Command(ExecuteSaveCommand, CanExecuteSaveCommand)); } }
        private bool CanExecuteSaveCommand() { return true; }
        private async void ExecuteSaveCommand()
        {
            while (Busy)
            {
                await Task.Delay(100);
            }
            try
            {
                Busy = true;
                await Task.Delay(2000);
                await _todoListRepository.SaveAsync(TodoLists.Select(x => x.TodoList).ToList());
            }
            finally { Busy = false; }
        }

        Mvvm.Command _RemoveAdsCommand = default(Mvvm.Command);
        public Mvvm.Command RemoveAdsCommand { get { return _RemoveAdsCommand ?? (_RemoveAdsCommand = new Mvvm.Command(ExecuteRemoveAdsCommand, CanExecuteRemoveAdsCommand)); } }
        private bool CanExecuteRemoveAdsCommand() { return !Busy && ShowAd; }
        private async void ExecuteRemoveAdsCommand()
        {
            try
            {
                Busy = true;
                var iapService = new Services.InAppPurchaseService.InAppPurchaseService();
                ShowAd = !(await iapService.PurchaseAsync());
            }
            finally { Busy = false; }
        }

        Mvvm.Command _ShowVideoAdCommand = default(Mvvm.Command);
        public Mvvm.Command ShowVideoAdCommand { get { return _ShowVideoAdCommand ?? (_ShowVideoAdCommand = new Mvvm.Command(ExecuteShowVideoAdCommand, CanExecuteShowVideoAdCommand)); } }
        private bool CanExecuteShowVideoAdCommand() { return !Busy; }
        private void ExecuteShowVideoAdCommand()
        {
            try
            {
                Busy = true;
                var adService = new Services.AdService.AdService();
                adService.AfterShown = () => ShowAd = false;
                adService.Show();
            }
            finally { Busy = false; }
        }

        #endregion  
    }
}
