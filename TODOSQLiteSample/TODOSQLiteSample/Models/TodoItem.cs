using System;

namespace TODOSQLiteSample.Models
{
    public class TodoItem : Mvvm.BindableBase
    {
        private string _Id = default(string);
        public string Id { get { return _Id; } set { Set(ref _Id, value); } }

        private string _Title = default(string);
        public string Title { get { return _Title; } set { Set(ref _Title, value); } }

        private DateTime _DueDate = default(DateTime);
        public DateTime DueDate { get { return _DueDate; } set { Set(ref _DueDate, value); } }

        private bool _IsComplete = default(bool);
        public bool IsComplete { get { return _IsComplete; } set { Set(ref _IsComplete, value); } }

        private string _Details = default(string);
        public string Details { get { return _Details; } set { Set(ref _Details, value); } }

        private bool _IsFavorite = default(bool);
        public bool IsFavorite { get { return _IsFavorite; } set { Set(ref _IsFavorite, value); } }

        private string _ListId = default(string);
        public string ListId { get { return _ListId; } set { Set(ref _ListId, value); } }
    }
}
