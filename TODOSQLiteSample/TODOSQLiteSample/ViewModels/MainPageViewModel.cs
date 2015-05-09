using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TODOSQLiteSample.Mvvm;
using Windows.UI.Xaml.Navigation;

namespace TODOSQLiteSample.ViewModels
{
    public class MainPageViewModel : Mvvm.ViewModelBase
    {
        Repositories.TodoListRepository _todoListRepository;

        public MainPageViewModel()
        {
            _todoListRepository = Repositories.TodoListRepository.GetDefault();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime sample data
                var data = _todoListRepository.Sample().Select(x => new ViewModels.TodoListViewModel(x));
                this.TodoLists = new ObservableCollection<ViewModels.TodoListViewModel>(data);
            }
            else
            {
                // update commands
                this.PropertyChanged += (s, e) =>
                {
                    this.AddListCommand.RaiseCanExecuteChanged();
                    this.RemoveListCommand.RaiseCanExecuteChanged();
                };
            }
        }

        public override Task OnNavigatedToAsync(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            LoadCommand.Execute(null);
            return Task.FromResult<object>(null);
        }

        //public override async Task OnNavigatedFromAsync(Dictionary<string, object> state, bool suspending)
        //{
        //    throw new NotImplementedException();
        //    //await _todoListRepository.SaveAsync(this.TodoLists.Select(x => x.TodoList).ToList());
        //}

        bool _busy = false;
        public bool Busy { get { return _busy; } set { Set(ref _busy, value); } }

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
                _todoListRepository.InsertItem(item.TodoList);
                this.TodoLists.Insert(0, item);
            }
            catch { }
        }


        Mvvm.Command<ViewModels.TodoListViewModel> _UpdateListCommand = default(Mvvm.Command<ViewModels.TodoListViewModel>);
        public Mvvm.Command<ViewModels.TodoListViewModel> UpdateListCommand { get { return _UpdateListCommand ?? (_UpdateListCommand = new Mvvm.Command<ViewModels.TodoListViewModel>(ExecuteUpdateListCommand, CanExecuteUpdateListCommand)); } }
        private bool CanExecuteUpdateListCommand(ViewModels.TodoListViewModel list) { return !Busy; }
        private void ExecuteUpdateListCommand(ViewModels.TodoListViewModel list)
        {
            try
            {
                _todoListRepository.UpdateItem(list.TodoList.Id, list.TodoList);
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
                var index = this.TodoLists.IndexOf(list);
                _todoListRepository.DeleteItem(list.TodoList.Id);
                this.TodoLists.Remove(list);
            }
            catch { }
        }

        Mvvm.Command _LoadCommand = default(Mvvm.Command);
        public Mvvm.Command LoadCommand { get { return _LoadCommand ?? (_LoadCommand = new Mvvm.Command(ExecuteLoadCommand, CanExecuteLoadCommand)); } }
        private bool CanExecuteLoadCommand() { return !Busy; }
        private void ExecuteLoadCommand()
        {
            try
            {
                Busy = true;

                var data = _todoListRepository.GetAllItems();
                this.TodoLists.Clear();
                foreach (var item in data.OrderBy(x => x.Title))
                {
                    this.TodoLists.Add(new ViewModels.TodoListViewModel(item));
                }
            }
            finally { Busy = false; }
        }

        #endregion  
    }
}
