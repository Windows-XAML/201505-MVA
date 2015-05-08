using System;
using Template10.Models;

namespace CortanaTodo.Models
{
    /// <summary>
    /// Represents an item in a <see cref="TodoList"/>.
    /// </summary>
    public class TodoItem : DataObject
    {
        private DateTime? dueDate;
        /// <summary>
        /// Gets or sets the date and time the item is due.
        /// </summary>
        public DateTime? DueDate
        {
            get
            {
                return dueDate;
            }
            set
            {
                Set(ref dueDate, value);
            }
        }

        private string title;
        /// <summary>
        /// Gets or sets the Title of the item.
        /// </summary>
        /// <value>
        /// The Title of the item.
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


        private bool isComplete;
        /// <summary>
        /// Gets or sets a value that indicates if the item is complete.
        /// </summary>
        /// <value>
        /// <c>true</c> if the item is complete; otherwise false.
        /// </value>
        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                Set(ref isComplete, value);
            }
        }
    }
}
