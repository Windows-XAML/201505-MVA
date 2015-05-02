using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Template10.Models;

namespace Template10.ViewModels
{
    public class TodoItemViewModel : Mvvm.ViewModelBase
    {
        static int count = 0;
        public TodoItemViewModel(Models.TodoItem todo)
        {
            this.TodoItem = todo;
            Debug.WriteLine("TodoItemViewModel created count [{0}]", count++);
        }

        private Models.TodoItem _TodoItem = default(Models.TodoItem);
        public Models.TodoItem TodoItem { get { return _TodoItem; } set { Set(ref _TodoItem, value); } }
    }
}
