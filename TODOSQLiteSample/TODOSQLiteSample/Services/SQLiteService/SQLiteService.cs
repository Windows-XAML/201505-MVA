using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TODOSQLiteSample.Services.SQLiteService
{
    class SQLiteService
    {
        public static SQLiteConnection conn;

        public static void LoadDatabase()
        {
            // Get a reference to the SQLite database
            conn = new SQLiteConnection("SQLiteTODO.db");

            // NOTE - The Id character is actually a GUID which is 36 characters long.
            // Here we speacify it as VARCHAR(36) - but actually SQLite does not impose a length
            // limit on columns, nor rigidly enforce data types. 
            // Could have specified CHARACTER(10), VARCHAR(255), TEXT - all would work for a GUID.
            // SQLite just stores these as 'TEXT' - see http://www.sqlite.org/datatype3.html#expraff
            string sql = @"CREATE TABLE IF NOT EXISTS
                                ToDoList (Id      VARCHAR( 36 ) PRIMARY KEY NOT NULL,
                                          Title   VARCHAR( 140 ) 
                            );";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }

            sql = @"CREATE TABLE IF NOT EXISTS
                                ToDoItem (Id         VARCHAR( 36 ) PRIMARY KEY NOT NULL,
                                         Title       VARCHAR( 140 ),
                                         DueDate     DATETIME,
                                         Details     VARCHAR( 140 ),
                                         IsFavorite  BOOLEAN,
                                         IsComplete  BOOLEAN,
                                         ListId      CHAR( 36 ),
                                         FOREIGN KEY(ListId) REFERENCES ToDoList(Id) ON DELETE CASCADE 
                            )";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }

            // Turn on Foreign Key constraints
            sql = @"PRAGMA foreign_keys = ON";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }
        }


        //public async Task<List<T>> ReadAsync<T>(string key)
        //{
        //    try { return await _helper.ReadFileAsync<List<T>>(key, FileHelper.StorageStrategies.Roaming); }
        //    catch { return new List<T>(); }
        //}

        //public async Task WriteAsync<T>(string key, List<T> items)
        //{
        //    await _helper.WriteFileAsync(key, items, FileHelper.StorageStrategies.Roaming);
        //}
    }
}
