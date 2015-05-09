using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TODOSQLiteSample.Models
{
    public class TodoList : Mvvm.BindableBase
    {
        private string _Id = Guid.NewGuid().ToString();
        public string Id { get { return _Id; } set { Set(ref _Id, value); } }

        private string _title;
        public string Title { get { return _title; } set { Set(ref _title, value); } }
    }
}
