using CortanaTodo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services;
using Template10.Services.FileService;

namespace CortanaTodo.Services
{
    /// <summary>
    /// A service for loading and saving Todo items and lists.
    /// </summary>
    public class TodoService
    {
        #region Static Version
        #region Constants
        private const string ListFileName = "ListData.json";
        #endregion // Constants

        #region Member Variables
        static private TodoService defaultInstance;
        #endregion // Member Variables

        #region Public Methods
        /// <summary>
        /// Gets the TodoService singleton.
        /// </summary>
        /// <returns>
        /// The TodoService singleton.
        /// </returns>
        static public TodoService GetDefault()
        {
            if (defaultInstance == null)
            {
                defaultInstance = new TodoService();
            }
            return defaultInstance;
        }
        #endregion // Public Methods
        #endregion // Static Version


        #region Instance Version

        #region Member Variables
        private ObservableCollection<TodoList> cache;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="TodoService"/> instance.
        /// </summary>
        private TodoService()
        {

        }
        #endregion // Constructors

        #region Public Methods
        /// <summary>
        /// Deletes the specified list.
        /// </summary>
        /// <param name="list">
        /// The list to delete.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public async Task DeleteAsync(TodoList list)
        {
            // Validate
            if (list == null) throw new ArgumentNullException("list");

            // Only proceed if lists are loaded
            if (cache != null)
            {
                // If found, remove and save
                if (cache.Contains(list))
                {
                    cache.Remove(list);
                    await SaveAllAsync();
                }
            }
        }

        /// <summary>
        /// Loads all lists.
        /// </summary>
        /// <param name="useCache">
        /// <c>true</c> if the cache should be used when available; otherwise false.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the result of the operation.
        /// </returns>
        public async Task<ObservableCollection<TodoList>> LoadListsAsync(bool useCache = true)
        {
            // If cache loaded and cache okay, just return it
            if ((useCache) && (cache != null))
            {
                return cache;
            }

            // Either no cache or cache not allowed

            // Try to load from disk
            cache = await FileHelper.ReadFileAsync<ObservableCollection<TodoList>>(ListFileName, FileHelper.StorageStrategies.Roaming);

            // If still null, that means the file doesn't exist. Create a new list.
            if (cache == null)
            {
                var defaultList = new TodoList()
                {
                    Title = "New List"
                };
                var defaultItem = new TodoItem()
                {
                    Title = "New Item"
                };

                defaultList.Items.Add(defaultItem);

                cache = new ObservableCollection<TodoList>()
                {
                    defaultList
                };
            }

            /*
            cache = new ObservableCollection<TodoList>()
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
                    Title = "Home Supplies",
                    Items = new ObservableCollection<TodoItem>()
                    {
                        new TodoItem() { Title = "Item A" },
                        new TodoItem() { Title = "Item B" },
                        new TodoItem() { Title = "Item C" },
                        new TodoItem() { Title = "Item D" },
                    }
                },
            };
            */
            return cache;
        }

        /// <summary>
        /// Savs the specified list.
        /// </summary>
        /// <param name="list">
        /// The list to save.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public async Task SaveAsync(TodoList list)
        {
            // Validate
            if (list == null) throw new ArgumentNullException("list");

            // Get the lists (loading if necessary)
            var lists = await LoadListsAsync();

            // If not in the lists, add
            if (!lists.Contains(list))
            {
                lists.Add(list);
            }

            // Save all
            await SaveAllAsync();
        }

        /// <summary>
        /// Saves all lists.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public async Task SaveAllAsync()
        {
            // If cache not loaded, ignore
            if (cache == null) { return; }

            // Try to save to disk
            await FileHelper.WriteFileAsync<ObservableCollection<TodoList>>(ListFileName, cache, FileHelper.StorageStrategies.Roaming);
        }
        #endregion // Public Methods
        #endregion // Instance Version
    }
}
