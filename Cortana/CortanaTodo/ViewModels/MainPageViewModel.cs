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
using Windows.ApplicationModel;
using Template10.Services.NavigationService;

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
        [CommandCanExecute(CommandNames.AddItem)]
        [CommandCanExecute(CommandNames.SaveList)]
        [CommandCanExecute(CommandNames.DeleteList)]
        private bool ListCommandsEnabled()
        {
            return currentList != null;
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        public override async Task HandleSuspendAsync(SuspendingEventArgs e)
        {
            await todoService.SaveAllAsync();
            await base.HandleSuspendAsync(e);
        }

        protected override Task LoadDefaultDataAsync()
        {
            // Load data with exception handling
            return RunWithErrorHandling(async ()=>
                {
                    Lists = await todoService.LoadListsAsync();
                    if (Lists.Count > 0)
                    {
                        CurrentList = Lists[0];
                        if (CurrentList.Items.Count > 0)
                        {
                            CurrentItem = CurrentList.Items[0];
                        }
                    }
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
                    Title = "Groceries",
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
                    Title = "Home Improvement",
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

        public override void OnNavigated(object sender, NavigationEventArgsEx e)
        {
            // Pass to base first
            base.OnNavigated(sender, e);

            // The parameter could be a list name if activated by speech
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                string listName = e.Parameter;

                // Try to find the list
                var list = lists.Where((l) => l.Title.Equals(listName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                // If found, focus it
                if (list != null)
                {
                    CurrentList = list;
                }
            }
        }
        #endregion // Overrides / Event Handlers


        #region Public Methods
        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        [Command(CommandNames.AddItem)]
        public void AddItem()
        {
            if (currentList != null)
            {
                var item = new TodoItem()
                {
                    Title = "New Item"
                };

                currentList.Items.Insert(0, item);
                CurrentItem = item;
            }
        }

        /// <summary>
        /// Adds a new list.
        /// </summary>
        [Command(CommandNames.AddList)]
        public void AddList()
        {
            var list = new TodoList()
            {
                Title = "New List"
            };
            var item = new TodoItem()
            {
                Title = "New Item"
            };
            list.Items.Add(item);
            lists.Add(list);
            CurrentList = list;
            CurrentItem = item;
        }
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
        public Task SaveListAsync(TodoList list = null)
        {
            // If null, use current
            if (list == null)
            {
                list = currentList;
            }

            // Save list with exception handling
            if (list != null)
            {
                return RunWithErrorHandling(() => todoService.SaveAsync(list), TaskRunOptions.WithFailure(string.Format("Could not save {0} list", list.Title)));
            }
            else
            {
                return TaskHelper.CompletedTask;
            }
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
        [CommandCanExecuteChanged(CommandNames.AddItem)]
        [CommandCanExecuteChanged(CommandNames.DeleteList)]
        [CommandCanExecuteChanged(CommandNames.SaveList)]
        public TodoList CurrentList
        {
            get { return currentList; }
            set
            {
                var previousList = currentList;
                if (Set(ref currentList, value))
                {
                    // Was there a previously selected list? If so save it.
                    if (previousList != null)
                    {
                        var t = SaveListAsync(previousList);
                    }
                }
            }
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
