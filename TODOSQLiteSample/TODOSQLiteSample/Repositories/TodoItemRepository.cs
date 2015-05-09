using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TODOSQLiteSample.Models;

namespace TODOSQLiteSample.Repositories
{
    public class TodoItemRepository : TableSQLiteRepoBase<TodoItem, string>
    {
        public Models.TodoItem Factory(string key = null, bool? complete = null, string title = null, DateTime? dueDate = null)
        {
            return new Models.TodoItem
            {
                Id = key ?? Guid.NewGuid().ToString(),
                IsComplete = complete ?? false,
                Title = title ?? string.Empty,
            };
        }

        public Models.TodoItem Clone(Models.TodoItem item)
        {
            return Factory
                (
                    Guid.Empty.ToString(),
                    false,
                    item.Title,
                    item.DueDate
                );
        }

        public IEnumerable<Models.TodoItem> Sample(int count = 5)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            foreach (var item in Enumerable.Range(1, count))
            {
                yield return Factory
                    (
                        Guid.NewGuid().ToString(),
                        false,
                        "Task-" + Guid.NewGuid().ToString(),
                        DateTime.Now.AddHours(random.Next(1, 200))
                    );
            }
        }

        private TodoItemRepository(string todoListId)
        {
            ToDoListId = todoListId;
        }

        private TodoItemRepository()
        {
            ToDoListId = string.Empty;
        }


        static Dictionary<string, TodoItemRepository> instances = new Dictionary<string, TodoItemRepository>();
        static TodoItemRepository defaultInstance;

        public override DateTime Timestamp
        {
            get
            {
                if (this == defaultInstance || defaultInstance == null)
                    return base.Timestamp;

                return defaultInstance.Timestamp;
            }
            protected set
            {
                if (this == defaultInstance || defaultInstance == null)
                    base.Timestamp = value;
                else
                    defaultInstance.Timestamp = value;
            }
        }

        public static TodoItemRepository GetForToDoListId(string todoListId)
        {
            lock (typeof(TodoItemRepository))
            {
                if (instances.ContainsKey(todoListId) != true)
                    instances[todoListId] = new TodoItemRepository(todoListId);
            }

            return instances[todoListId];
        }

        public static TodoItemRepository GetDefault()
        {
            lock (typeof(TodoItemRepository))
            {
                if (defaultInstance == null)
                    defaultInstance = new TodoItemRepository();
            }

            return defaultInstance;
        }

        public string ToDoListId { get; private set; }

        protected override string GetSelectAllSql()
        {
            if (string.IsNullOrEmpty(ToDoListId))
                return @"SELECT Id, Title, DueDate, Details, IsFavorite, IsComplete, ListId 
                           FROM ToDoItem
                           ORDER BY DueDate";
            else
                return @"SELECT Id, Title, DueDate, Details, IsFavorite, IsComplete, ListId 
                           FROM ToDoItem
                           WHERE ListId = ?
                           ORDER BY DueDate";
        }

        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            if (string.IsNullOrEmpty(ToDoListId))
                return;

            statement.Bind(1, ToDoListId);
        }

        protected override TodoItem CreateItem(ISQLiteStatement statement)
        {
            TodoItem todoItem = new TodoItem()
            {
                Id = (string)statement[0],
                Title = (string)statement[1],
                DueDate = DateTime.Parse((string)statement[2]),
                Details = (string)statement[3],
                IsFavorite = ((long)statement[4]) == 1 ? true : false,
                IsComplete = ((long)statement[5]) == 1 ? true : false,
                ListId = (string)statement[6],
            };

            Debug.WriteLine("Selected ToDoItem Title:" + todoItem.Title);
            return todoItem;
        }

        protected override string GetSelectItemSql()
        {
            return @"SELECT Id, Title, DueDate, Details, IsFavorite, IsComplete, ListId 
                           FROM ToDoItem
                           WHERE Id = ?";
        }

        protected override void FillSelectItemStatement(ISQLiteStatement statement, string key)
        {
            statement.Bind(1, key);
        }

        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM ToDoItem WHERE Id = ?";
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, string key)
        {
            statement.Bind(1, key);
        }

        protected override string GetInsertItemSql()
        {
            return "INSERT INTO ToDoItem (Id, Title, DueDate, Details, IsFavorite, IsComplete, ListId) VALUES (?, ?, ?, ?, ?, ?, ?)";
        }

        protected override void FillInsertStatement(ISQLiteStatement statement, TodoItem item)
        {
            statement.Bind(1, item.Id);
            statement.Bind(2, item.Title);
            statement.Bind(3, item.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
            statement.Bind(4, item.Details);
            statement.Bind(5, item.IsFavorite ? 1 : 0);
            statement.Bind(6, item.IsComplete ? 1 : 0);
            statement.Bind(7, item.ListId);
        }

        protected override string GetUpdateItemSql()
        {
            return "UPDATE ToDoItem SET Id = ?, Title = ?, DueDate = ?, Details = ?, IsFavorite = ?, IsComplete = ?, ListId = ? WHERE Id = ?";
        }

        protected override void FillUpdateStatement(ISQLiteStatement statement, string key, TodoItem item)
        {
            FillInsertStatement(statement, item);
            statement.Bind(8, key);
        }
    }
}
