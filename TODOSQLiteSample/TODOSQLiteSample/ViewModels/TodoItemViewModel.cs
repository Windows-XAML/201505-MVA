using System;
using System.Collections.Generic;
using System.Linq;
using SQLitePCL;
using TODOSQLiteSample.Models;
using System.Diagnostics;

namespace TODOSQLiteSample.ViewModels
{
    public class TodoItemViewModel : Mvvm.ViewModelBase
    {
        public TodoItemViewModel(TodoItem todo)
        {
            this.TodoItem = todo;
        }

        private TodoItem _TodoItem = default(TodoItem);
        public TodoItem TodoItem { get { return _TodoItem; } set { Set(ref _TodoItem, value); } }

    }
}
