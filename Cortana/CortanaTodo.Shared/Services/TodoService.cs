using CortanaTodo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services;

namespace CortanaTodo.Services
{
    /// <summary>
    /// A service for loading and saving Todo items and lists.
    /// </summary>
    public class TodoService
    {
        #region Static Version
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



        /// <summary>
        /// Initializes a new <see cref="TodoService"/> instance.
        /// </summary>
        private TodoService()
        {
            
        }

        public Task DeleteAsync(TodoList list)
        {
            return TaskHelper.CompletedTask;
        }

        public Task<ObservableCollection<TodoList>> LoadListsAsync()
        {
            var result = new ObservableCollection<TodoList>()
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
            return Task.FromResult(result);
        }

        public Task SaveAsync(TodoList list)
        {
            // TODO: If not in cache, add to it
            
            return TaskHelper.CompletedTask;
        }
    }
}
