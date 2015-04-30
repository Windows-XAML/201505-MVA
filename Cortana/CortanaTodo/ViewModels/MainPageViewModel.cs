using CortanaTodo.Models;
using CortanaTodo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services;
using Windows.UI.Xaml.Navigation;

namespace CortanaTodo.ViewModels
{
    /// <summary>
    /// The ViewModel for the main page of the application which allows the management of lists and items.
    /// </summary>
    public class MainPageViewModel : ViewModel
    {
        #region Constants
        static public class CommandNames
        {
            public const string AddItem = "AddItem";
            public const string AddList = "AddList";
            public const string DeleteItem = "DeleteItem";
            public const string DeleteList = "DeleteList";
            public const string SaveList = "SaveList";
        }
        #endregion // Constants

        #region Member Variables
        private TodoService todoService;
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// Used to test if item-related commands are enabled.
        /// </summary>
        /// <returns>
        /// <c>true</c> if item-related commands are enabled; otherwise false.
        /// </returns>
        [CommandCanExecute(CommandNames.DeleteItem)]
        private bool ItemCommandsEnabled()
        {
            return currentItem != null;
        }

        /// <summary>
        /// Used to test if list-related commands are enabled.
        /// </summary>
        /// <returns>
        /// <c>true</c> if list-related commands are enabled; otherwise false.
        /// </returns>
        [CommandCanExecute(CommandNames.SaveList)]
        [CommandCanExecute(CommandNames.DeleteList)]
        private bool ListCommandsEnabled()
        {
            return currentList != null;
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        protected override Task LoadDefaultDataAsync()
        {
            // Load data with exception handling
            return RunWithErrorHandling(async ()=>
                {
                    Lists = await todoService.LoadListsAsync();
                }
            , TaskRunOptions.WithFailure("Could not load lists."));
        }

        protected override void LoadDesignData()
        {
            base.LoadDesignData();
            Lists = new ObservableCollection<TodoList>()
            {
                new TodoList()
                {
                    Title = "List A",
                    Items = new ObservableCollection<TodoItem>()
                    {
                        new TodoItem() { Title = "Item A" },
                        new TodoItem() { Title = "Item B" },
                        new TodoItem() { Title = "Item C" },
                        new TodoItem() { Title = "Item D" },
                    }
                },

                new TodoList()
                {
                    Title = "List B",
                    Items = new ObservableCollection<TodoItem>()
                    {
                        new TodoItem() { Title = "Item A" },
                        new TodoItem() { Title = "Item B" },
                        new TodoItem() { Title = "Item C" },
                        new TodoItem() { Title = "Item D" },
                    }
                },
            };
        }

        protected override void ObtainServices()
        {
            base.ObtainServices();
            todoService = TodoService.GetDefault();
        }

        protected override void OnNavigatedFrom(Dictionary<string, object> state, bool suspending)
        {
            // Save the work
            var t = SaveListAsync();
        }
        #endregion // Overrides / Event Handlers


        #region Public Methods
        /// <summary>
        /// Deletes the current Item.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        [Command(CommandNames.DeleteItem)]
        public Task DeleteItemAsync()
        {
            // Delete Item with exception handling
            return RunWithErrorHandling(async () =>
            {
                // Validate
                if (currentList == null) throw new ArgumentNullException("currentList");
                if (currentItem == null) throw new ArgumentNullException("currentItem");

                // Get index of current Item
                var index = currentList.Items.IndexOf(currentItem);

                // Find out previous but bound to 0
                index = Math.Max(index - 1, 0);

                // Remove from the list
                currentList.Items.Remove(currentItem);

                // Save the list
                await todoService.SaveAsync(currentList);

                // Set current Item to the next closest one
                if (currentList.Items.Count > index) { CurrentItem = currentList.Items[index]; }
            }
            , TaskRunOptions.WithFailure(string.Format("Could not delete {0}", currentItem.Title)));
        }

        /// <summary>
        /// Deletes the current list.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        [Command(CommandNames.DeleteList)]
        public Task DeleteListAsync()
        {
            // Delete list with exception handling
            return RunWithErrorHandling(async ()=>
                {
                    // Validate
                    if (currentList == null) throw new ArgumentNullException("currentList");

                    // Get index of current list
                    var index = lists.IndexOf(currentList);

                    // Find out previous but bound to 0
                    index = Math.Max(index - 1, 0);

                    // Attempt to delete current list
                    await todoService.DeleteAsync(currentList);

                    // Delete successful so remove from lists collection
                    lists.Remove(currentList);

                    // Set current list to the next closest one
                    if (lists.Count > index) { CurrentList = lists[index]; }
                }
                , TaskRunOptions.WithFailure(string.Format("Could not delete {0} list", currentList.Title)));
        }

        /// <summary>
        /// Saves the current list.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        [Command(CommandNames.SaveList)]
        public Task SaveListAsync()
        {
            // Save list with exception handling
            return RunWithErrorHandling(() => todoService.SaveAsync(currentList), TaskRunOptions.WithFailure(string.Format("Could not save {0} list", currentList.Title)));
        }
        #endregion // Public Methods


        #region Public Properties
        private TodoItem currentItem;
        /// <summary>
        /// Gets or sets the Item that currently has focus.
        /// </summary>
        /// <value>
        /// The Item that currently has focus.
        /// </value>
        [CommandCanExecuteChanged(CommandNames.DeleteItem)]
        public TodoItem CurrentItem
        {
            get { return currentItem; }
            set { Set(ref currentItem, value); }
        }

        private TodoList currentList;
        /// <summary>
        /// Gets or sets the list that currently has focus.
        /// </summary>
        /// <value>
        /// The list that currently has focus.
        /// </value>
        [CommandCanExecuteChanged(CommandNames.DeleteList)]
        [CommandCanExecuteChanged(CommandNames.SaveList)]
        public TodoList CurrentList
        {
            get { return currentList; }
            set { Set(ref currentList, value); }
        }

        private ObservableCollection<TodoList> lists;
        /// <summary>
        /// Gets or sets all lists being managed by the view model.
        /// </summary>
        /// <value>
        /// All lists being managed by the view model.
        /// </value>
        public ObservableCollection<TodoList> Lists
        {
            get { return lists; }
            set { Set(ref lists, value); }
        }
        #endregion // Public Properties
    }
}
