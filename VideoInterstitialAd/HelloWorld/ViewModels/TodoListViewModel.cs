using System;
using System.Collections.ObjectModel;

namespace Template10.ViewModels
{
    public class TodoListViewModel : Mvvm.ViewModelBase
    {
        Repositories.TodoItemRepository _todoItemRepository;

        public TodoListViewModel(Models.TodoList list)
        {
            _todoItemRepository = new Repositories.TodoItemRepository();

            TodoList = list;
            foreach (var item in list.Items)
            {
                Items.Add(new ViewModels.TodoItemViewModel(item));
            }
        }

        private Models.TodoList _TodoList = default(Models.TodoList);
        public Models.TodoList TodoList { get { return _TodoList; } set { Set(ref _TodoList, value); } }

        private ObservableCollection<ViewModels.TodoItemViewModel> _Items = new ObservableCollection<TodoItemViewModel>();
        public ObservableCollection<ViewModels.TodoItemViewModel> Items { get { return _Items; } private set { Set(ref _Items, value); } }

        private ViewModels.TodoItemViewModel _SelectedItem = default(ViewModels.TodoItemViewModel);
        public ViewModels.TodoItemViewModel SelectedItem { get { return _SelectedItem; } set { Set(ref _SelectedItem, value); SelectedItemIsSelected = (value != null); } }

        private bool _SelectedItemIsSelected = default(bool);
        public bool SelectedItemIsSelected { get { return _SelectedItemIsSelected; } set { Set(ref _SelectedItemIsSelected, value); } }

        Mvvm.Command<string> _AddCommand = default(Mvvm.Command<string>);
        public Mvvm.Command<string> AddCommand { get { return _AddCommand ?? (_AddCommand = new Mvvm.Command<string>(ExecuteAddCommand, CanExecuteAddCommand)); } }
        private bool CanExecuteAddCommand(string title) { return true; }
        private void ExecuteAddCommand(string title)
        {
            try
            {
                var index = Items.IndexOf(SelectedItem);
                var item = new ViewModels.TodoItemViewModel(_todoItemRepository.Factory(title: title));
                TodoList.Items.Insert((index > -1) ? index : 0, item.TodoItem);
                Items.Insert((index > -1) ? index : 0, item);
                SelectedItem = item;
            }
            catch { SelectedItem = null; }
        }

        Mvvm.Command<Models.TodoItem> _RemoveCommand = default(Mvvm.Command<Models.TodoItem>);
        public Mvvm.Command<Models.TodoItem> RemoveCommand { get { return _RemoveCommand ?? (_RemoveCommand = new Mvvm.Command<Models.TodoItem>(ExecuteRemoveCommand, CanExecuteRemoveCommand)); } }
        private bool CanExecuteRemoveCommand(Models.TodoItem param) { return SelectedItem != null; }
        private void ExecuteRemoveCommand(Models.TodoItem param)
        {
            try
            {
                var index = Items.IndexOf(SelectedItem);
                TodoList.Items.Remove(SelectedItem.TodoItem);
                Items.Remove(SelectedItem);
                SelectedItem = Items[index];
            }
            catch { SelectedItem = null; }
        }
    }
}
