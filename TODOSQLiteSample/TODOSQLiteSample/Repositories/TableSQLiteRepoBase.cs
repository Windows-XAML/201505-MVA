using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TODOSQLiteSample.Services.SQLiteService;

namespace TODOSQLiteSample.Repositories
{
    public abstract class TableSQLiteRepoBase<TItemType, TKeyType>
    {
        protected abstract string GetSelectAllSql();
        protected abstract void FillSelectAllStatement(ISQLiteStatement statement);

        protected abstract TItemType CreateItem(ISQLiteStatement statement);

        protected abstract string GetSelectItemSql();
        protected abstract void FillSelectItemStatement(ISQLiteStatement statement, TKeyType key);

        protected abstract string GetDeleteItemSql();
        protected abstract void FillDeleteItemStatement(ISQLiteStatement statement, TKeyType key);

        protected abstract string GetInsertItemSql();
        protected abstract void FillInsertStatement(ISQLiteStatement statement, TItemType item);

        protected abstract string GetUpdateItemSql();
        protected abstract void FillUpdateStatement(ISQLiteStatement statement, TKeyType key, TItemType item);

        protected DateTime lastModifiedTime = DateTime.MaxValue;
        public virtual DateTime Timestamp
        {
            get { return lastModifiedTime; }
            protected set { lastModifiedTime = value; }
        }

        private ISQLiteConnection sqlConnection
        {
            get
            {
                return SQLiteService.conn;
            }
        }

        public ObservableCollection<TItemType> GetAllItems()
        {
            var items = new ObservableCollection<TItemType>();
            using (var statement = sqlConnection.Prepare(GetSelectAllSql()))
            {
                FillSelectAllStatement(statement);
                while (statement.Step() == SQLiteResult.ROW)
                {
                    var item = CreateItem(statement);
                    items.Add(item);
                }
            }
            Timestamp = DateTime.Now;

            return items;
        }

        public TItemType GetItem(TKeyType key)
        {
            using (var statement = sqlConnection.Prepare(GetSelectItemSql()))
            {
                FillSelectItemStatement(statement, key);

                if (statement.Step() == SQLiteResult.ROW)
                {
                    var item = CreateItem(statement);
                    Timestamp = DateTime.Now;
                    return item;
                }
            }

            throw new ArgumentOutOfRangeException("key not found");
        }

        public void InsertItem(TItemType item)
        {
            using (var statement = sqlConnection.Prepare(GetInsertItemSql()))
            {
                FillInsertStatement(statement, item);
                statement.Step();
            }
            Timestamp = DateTime.Now;
        }

        public void UpdateItem(TKeyType key, TItemType item)
        {
            using (var statement = sqlConnection.Prepare(GetUpdateItemSql()))
            {
                FillUpdateStatement(statement, key, item);
                statement.Step();
            }
            Timestamp = DateTime.Now;
        }

        public void DeleteItem(TKeyType key)
        {
            using (var statement = sqlConnection.Prepare(GetDeleteItemSql()))
            {
                FillDeleteItemStatement(statement, key);
                statement.Step();
            }
            Timestamp = DateTime.Now;
        }
    }
}
