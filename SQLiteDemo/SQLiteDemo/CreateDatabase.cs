using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    public class CreateDatabase
    {
        public static void LoadDatabase(SQLiteConnection db)
        {
            string sql = @"CREATE TABLE IF NOT EXISTS
                                Customer (Id      INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            Name    VARCHAR( 140 ),
                                            City    VARCHAR( 140 ),
                                            Contact VARCHAR( 140 ) 
                            );";
            using (var statement = db.Prepare(sql))
            {
                statement.Step();
            }

            sql = @"CREATE TABLE IF NOT EXISTS
                                Project (Id          INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                         CustomerId  INTEGER,
                                         Name        VARCHAR( 140 ),
                                         Description VARCHAR( 140 ),
                                         DueDate     DATETIME,
                                         FOREIGN KEY(CustomerId) REFERENCES Customer(Id) ON DELETE CASCADE 
                            )";
            using (var statement = db.Prepare(sql))
            {
                statement.Step();
            }

            // Turn on Foreign Key constraints
            sql = @"PRAGMA foreign_keys = ON";
            using (var statement = db.Prepare(sql))
            {
                statement.Step();
            }
        }

        public async static Task ResetDataAsync(SQLiteConnection db)
        {
            // Empty the Customer and Project tables 
            string sql = @"DELETE FROM Project";
            using (var statement = db.Prepare(sql))
            {
                statement.Step();
            }

            sql = @"DELETE FROM Customer";
            using (var statement = db.Prepare(sql))
            {
                statement.Step();
            }

            List<Task> tasks = new List<Task>();

            // Add seed customers and projects
            var cust1Task = InsertCustomer(db, "Adventure Works", "Bellevue", "Mu Han");
            tasks.Add(cust1Task.ContinueWith((id) => InsertProject(db, id.Result, "Expense Reports", "Windows Store app", DateTime.Today.AddDays(4))));
            tasks.Add(cust1Task.ContinueWith((id) => InsertProject(db, id.Result, "Time Reporting", "Windows Store app", DateTime.Today.AddDays(14))));
            tasks.Add(cust1Task.ContinueWith((id) => InsertProject(db, id.Result, "Project Management", "Windows Store app", DateTime.Today.AddDays(24))));
            await Task.WhenAll(tasks.ToArray());

            tasks = new List<Task>();
            var cust2Task = InsertCustomer(db, "Contoso", "Seattle", "David Hamilton");
            tasks.Add(cust2Task.ContinueWith((id) => InsertProject(db, id.Result, "Soccer Scheduling", "Windows Phone app", DateTime.Today.AddDays(6))));
            await Task.WhenAll(tasks.ToArray());

            tasks = new List<Task>();
            var cust3Task = InsertCustomer(db, "Fabrikam", "Redmond", "Guido Pica");
            tasks.Add(cust3Task.ContinueWith((id) => InsertProject(db, id.Result, "Product Catalog", "MVC4 app", DateTime.Today.AddDays(4))));
            tasks.Add(cust3Task.ContinueWith((id) => InsertProject(db, id.Result, "Expense Reports", "Windows Store app", DateTime.Today.AddDays(-3))));
            tasks.Add(cust3Task.ContinueWith((id) => InsertProject(db, id.Result, "Expense Reports", "Windows Phone app", DateTime.Today.AddDays(45))));
            await Task.WhenAll(tasks.ToArray());

            tasks = new List<Task>();
            var cust4Task = InsertCustomer(db, "Tailspin Toys", "Kent", "Michelle Alexander");
            tasks.Add(cust4Task.ContinueWith((id) => InsertProject(db, id.Result, "Kids Game", "Windows Store app", DateTime.Today.AddDays(60))));
            await Task.WhenAll(tasks.ToArray());
        }

        private async static Task<long> InsertCustomer(ISQLiteConnection db, string customerName, string customerCity, string customerContact)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var custstmt = db.Prepare("INSERT INTO Customer (Name, City, Contact) VALUES (?, ?, ?)"))
                    {
                        custstmt.Bind(1, customerName);
                        custstmt.Bind(2, customerCity);
                        custstmt.Bind(3, customerContact);
                        custstmt.Step();

                        Debug.WriteLine("INSERT completed OK for customer " + customerName);
                    }
                }
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

            using (var idstmt = db.Prepare("SELECT last_insert_rowid()"))
            {
                idstmt.Step();
                {
                    Debug.WriteLine("INSERT ID for customer " + customerName + ": " + (long)idstmt[0]);
                    return (long)idstmt[0];
                }
                throw new Exception("Couldn't get inserted ID");
            };
        }

        private static Task InsertProject(ISQLiteConnection db, long customerID, string name, string description, DateTime duedate)
        {
            return Task.Run(() =>
            {

                using (var projstmt = db.Prepare("INSERT INTO Project (CustomerId, Name, Description, DueDate) VALUES (?, ?, ?, ?)"))
                {

                    // Reset the prepared statement so we can reuse it.
                    projstmt.ClearBindings();
                    projstmt.Reset();

                    projstmt.Bind(1, customerID);
                    projstmt.Bind(2, name);
                    projstmt.Bind(3, description);
                    projstmt.Bind(4, duedate.ToString("yyyy-MM-dd HH:mm:ss"));

                    projstmt.Step();
                    Debug.WriteLine("INSERT Project completed OK for customer " + customerID);
                }
            }
            );
        }

    }

}
