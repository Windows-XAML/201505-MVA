using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SQLiteDemo.ViewModels
{
    public class CustomersViewModel : TableViewModelBase<Customer, long>
    {
        private CustomersViewModel() { }

        static CustomersViewModel instance;
        public static CustomersViewModel GetDefault()
        {
            lock (typeof(CustomersViewModel))
            {
                if (instance == null)
                    instance = new CustomersViewModel();
            }

            return instance;
        }

        protected override string GetSelectAllSql()
        {
            return "SELECT Id, Name, City, Contact FROM Customer";
        }

        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            // nothing to do
        }

        protected override Customer CreateItem(ISQLiteStatement statement)
        {
            var c = new Customer(
              (long)statement[0],
              (string)statement[1],
              (string)statement[2],
              (string)statement[3]);
            Debug.WriteLine("Selected Customer name:" + c.Name);

            return c;
        }

        protected override string GetSelectItemSql()
        {
            return "SELECT Id, Name, City, Contact FROM Customer WHERE Id = ?";
        }

        protected override void FillSelectItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }

        protected override string GetInsertItemSql()
        {
            return "INSERT INTO Customer (Name, City, Contact) VALUES (@name, @city, @contact)";
        }

        protected override void FillInsertStatement(ISQLiteStatement statement, Customer item)
        {
            // NOTE that named parameters have a leading "@",":" or "$".
            statement.Bind("@name", item.Name);
            statement.Bind("@city", item.City);
            statement.Bind("@contact", item.Contact);
        }

        protected override string GetUpdateItemSql()
        {
            return "UPDATE Customer SET Name = ?, City = ?, Contact = ? WHERE Id = ?";
        }

        protected override void FillUpdateStatement(ISQLiteStatement statement, long key, Customer item)
        {
            // NOTE that the first host parameter has an index of 1, not 0.
            statement.Bind(1, item.Name);
            statement.Bind(2, item.City);
            statement.Bind(3, item.Contact);
            statement.Bind(4, key);
        }

        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM Customer WHERE Id = ?";
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }
    }
}
