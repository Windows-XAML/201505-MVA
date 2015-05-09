using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TODOSQLiteSample.Models;
using TODOSQLiteSample.ViewModels;

namespace TODOSQLiteSample.Repositories
{
    public class TodoListRepository : TableSQLiteRepoBase<TodoList, string>
    {
        //public async Task<List<Models.TodoList>> GetAsync()
        //{
        //    return _cache ?? (_cache = await _fileService.ReadAsync<Models.TodoList>(cachekey) ?? new List<TodoList>());
        //}

        //public async Task<Models.TodoList> GetAsync(string key)
        //{
        //    return (await this.GetAsync()).FirstOrDefault(x => x.Id.Equals(key));
        //}

        //public async Task SaveAsync(List<Models.TodoList> list)
        //{
        //    await _fileService.WriteAsync<Models.TodoList>(cachekey, list);
        //}

        private TodoListRepository()
        {
        }

        static TodoListRepository defaultInstance;

        public static TodoListRepository GetDefault()
        {
            lock (typeof(TodoItemRepository))
            {
                if (defaultInstance == null)
                    defaultInstance = new TodoListRepository();
            }

            return defaultInstance;
        }

        public Models.TodoList Factory(string key = null, string title = null, IEnumerable<Models.TodoItem> items = null)
        {
            return new Models.TodoList
            {
                Id = key ?? Guid.NewGuid().ToString(),
                Title = title ?? Guid.NewGuid().ToString(),
//                Items = new System.Collections.ObjectModel.ObservableCollection<TodoItem>(items ?? new Models.TodoItem[] { }),
            };
        }

        public Models.TodoList Clone(Models.TodoList list)
        {
            return Factory
                (
                    null,
                    list.Title
//                    ,list.Items.Select(x => _todoItemRepository.Clone(x))
                );
        }

        public IEnumerable<Models.TodoList> Sample(int count = 5)
        {
            var _todoItemRepository = TodoItemRepository.GetDefault();
            var random = new Random((int)DateTime.Now.Ticks);
            foreach (var List in Enumerable.Range(1, count))
            {
                yield return Factory
                    (
                        Guid.NewGuid().ToString(),
                        "List-" + Guid.NewGuid().ToString(),
                        _todoItemRepository.Sample(random.Next(5, 30))
                    );
            }
        }

        #region SQL command configuration

        protected override string GetSelectAllSql()
        {
            return "SELECT Id, Title FROM ToDoList";
        }

        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            // Nothing to do
        }

        protected override TodoList CreateItem(ISQLiteStatement statement)
        {
            var l = new TodoList()
            {
                Id = (string)statement[0],
                Title = (string)statement[1],
            };

            Debug.WriteLine("Selected List title:" + l.Title);
            return l;
        }

        protected override string GetSelectItemSql()
        {
            return "SELECT Id, Title FROM ToDoList WHERE Id = ?";
        }

        protected override void FillSelectItemStatement(ISQLiteStatement statement, string key)
        {
            statement.Bind(1, key);
        }

        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM ToDoList WHERE Id = ?";
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, string key)
        {
            statement.Bind(1, key);
        }

        protected override string GetInsertItemSql()
        {
            return "INSERT INTO ToDoList (Id, Title) VALUES (@id, @title)";
        }

        protected override void FillInsertStatement(ISQLiteStatement statement, TodoList item)
        {
            // NOTE that named parameters have a leading "@",":" or "$".
            statement.Bind("@id", item.Id);
            statement.Bind("@title", item.Title);
        }

        protected override string GetUpdateItemSql()
        {
            return "UPDATE ToDoList SET Id = ?, Title = ? WHERE Id = ?";
        }

        protected override void FillUpdateStatement(ISQLiteStatement statement, string key, TodoList item)
        {
            // NOTE that the first host parameter has an index of 1, not 0.
            statement.Bind(1, item.Id);
            statement.Bind(2, item.Title);
            statement.Bind(3, key);
        }

        #endregion
    }
}
