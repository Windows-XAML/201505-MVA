using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Template10.Models;

namespace CortanaTodo.Models
{
    /// <summary>
    /// Represents a named list of <see cref="TodoItem"/>s.
    /// </summary>
    public class TodoList : DataObject
    {
        private ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
        /// <summary>
        /// Gets or sets the collection of items in the list.
        /// </summary>
        /// <value>
        /// The collection of items in the list.
        /// </value>
        public ObservableCollection<Models.TodoItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                Set(ref items, value);
            }
        }

        private string title;
        /// <summary>
        /// Gets or sets the Title of the list.
        /// </summary>
        /// <value>
        /// The Title of the list.
        /// </value>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                Set(ref title, value);
            }
        }
    }
}
