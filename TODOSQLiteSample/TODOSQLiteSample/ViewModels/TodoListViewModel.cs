using System;
using System.Collections.ObjectModel;
using SQLitePCL;
using TODOSQLiteSample.Models;
using System.Diagnostics;
using TODOSQLiteSample.Repositories;

namespace TODOSQLiteSample.ViewModels
{
    public class TodoListViewModel : Mvvm.ViewModelBase
    {
        public TodoListViewModel(Models.TodoList list)
        {
            this.TodoList = list;

            var repo = TodoItemRepository.GetForToDoListId(list.Id);
            var todoItems = repo.GetAllItems();
            foreach (var item in todoItems)
            {
                this.Items.Add(new ViewModels.TodoItemViewModel(item));
            }
        }

        private Models.TodoList _TodoList = default(Models.TodoList);
        public Models.TodoList TodoList { get { return _TodoList; } set { Set(ref _TodoList, value); } }

        private ObservableCollection<ViewModels.TodoItemViewModel> _Items = new ObservableCollection<TodoItemViewModel>();
        public ObservableCollection<ViewModels.TodoItemViewModel> Items { get { return _Items; } private set { Set(ref _Items, value); } }

        #region Track Selected Item

        private ViewModels.TodoItemViewModel _SelectedItem = default(ViewModels.TodoItemViewModel);
        public ViewModels.TodoItemViewModel SelectedItem { get { return _SelectedItem; } set { Set(ref _SelectedItem, value); SelectedItemIsSelected = (value != null); } }


        private bool _SelectedItemIsSelected = default(bool);
        public bool SelectedItemIsSelected { get { return _SelectedItemIsSelected; } set { Set(ref _SelectedItemIsSelected, value); } }

#endregion

#region Commands
        /// <summary>
        /// Creates a new ToDoItem and adds it to the current ToDoList
        /// </summary>
        Mvvm.Command<string> _AddCommand = default(Mvvm.Command<string>);
        public Mvvm.Command<string> AddCommand { get { return _AddCommand ?? (_AddCommand = new Mvvm.Command<string>(ExecuteAddCommand, CanExecuteAddCommand)); } }
        private bool CanExecuteAddCommand(string title) { return true; }
        private void ExecuteAddCommand(string title)
        {
            try
            {
                var index = this.Items.IndexOf(this.SelectedItem);
                var item = new TodoItemViewModel(TodoItemRepository.GetDefault().Factory(title: title));
                // Set the ListId
                item.TodoItem.ListId = TodoList.Id;

                // Add to repo
                TodoItemRepository.GetForToDoListId(TodoList.Id).InsertItem(item.TodoItem);

                // Insert into ViewModel collection
                this.Items.Insert((index > -1) ? index : 0, item);
                this.SelectedItem = item;
            }
            catch { this.SelectedItem = null; }
        }


        Mvvm.Command<ViewModels.TodoItemViewModel> _UpdateItemCommand = default(Mvvm.Command<ViewModels.TodoItemViewModel>);
        public Mvvm.Command<ViewModels.TodoItemViewModel> UpdateItemCommand { get { return _UpdateItemCommand ?? (_UpdateItemCommand = new Mvvm.Command<ViewModels.TodoItemViewModel>(ExecuteUpdateItemCommand, CanExecuteUpdateItemCommand)); } }
        private bool CanExecuteUpdateItemCommand(ViewModels.TodoItemViewModel item) { return true; }
        private void ExecuteUpdateItemCommand(ViewModels.TodoItemViewModel item)
        {
            try
            {
                TodoItemRepository.GetForToDoListId(TodoList.Id).UpdateItem(item.TodoItem.Id, item.TodoItem);
            }
            catch { }
        }

        /// <summary>
        /// Removes a ToDoItem from the current list
        /// </summary>
        Mvvm.Command<Models.TodoItem> _RemoveCommand = default(Mvvm.Command<Models.TodoItem>);
        public Mvvm.Command<Models.TodoItem> RemoveCommand { get { return _RemoveCommand ?? (_RemoveCommand = new Mvvm.Command<Models.TodoItem>(ExecuteRemoveCommand, CanExecuteRemoveCommand)); } }
        private bool CanExecuteRemoveCommand(Models.TodoItem param) { return this.SelectedItem != null; }
        private void ExecuteRemoveCommand(Models.TodoItem param)
        {
            try
            {
                var index = this.Items.IndexOf(this.SelectedItem);
                // Delete from repo
                TodoItemRepository.GetForToDoListId(TodoList.Id).DeleteItem(this.SelectedItem.TodoItem.Id);
                this.Items.Remove(this.SelectedItem);
                this.SelectedItem = this.Items[index];
            }
            catch { this.SelectedItem = null; }
        }

#endregion

    }
}
