using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SQLiteDemo.ViewModels
{
  public class ProjectsViewModel : TableViewModelBase<Project, long>
  {
    private ProjectsViewModel(long customerId)
    {
      CustomerId = customerId;
    }

    private ProjectsViewModel()
    {
      CustomerId = -1;
    }

    static Dictionary<long, ProjectsViewModel> instances = new Dictionary<long, ProjectsViewModel>();
    static ProjectsViewModel defaultInstance;

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

    public static ProjectsViewModel GetForCustomerId(long customerId)
    {
      lock (typeof(ProjectsViewModel))
      {
        if (instances.ContainsKey(customerId) != true)
          instances[customerId] = new ProjectsViewModel(customerId);
      }

      return instances[customerId];
    }

    public static ProjectsViewModel GetDefault()
    {
      lock (typeof(ProjectsViewModel))
      {
        if (defaultInstance == null)
          defaultInstance = new ProjectsViewModel();
      }

      return defaultInstance;
    }

    public long CustomerId { get; private set; }

    protected override string GetSelectAllSql()
    {
      if (CustomerId < 0)
        return @"SELECT Id, CustomerId, Name, Description, DueDate 
                           FROM Project
                           ORDER BY DueDate";
      else
        return @"SELECT Id, CustomerId, Name, Description, DueDate 
                           FROM Project
                           WHERE CustomerId = ?
                           ORDER BY DueDate";
    }

    protected override void FillSelectAllStatement(ISQLiteStatement statement)
    {
      if (CustomerId < 0)
        return;

      statement.Bind(1, CustomerId);
    }

    protected override Project CreateItem(ISQLiteStatement statement)
    {
      Project project = new Project(
          (long)statement[0],
          (long)statement[1],
          (string)statement[2],
          (string)statement[3],
          DateTime.Parse((string)statement[4])
        );

      Debug.WriteLine("Selected Project name:" + project.Name);
      return project;
    }

    protected override string GetSelectItemSql()
    {
      return @"SELECT Id, CustomerId, Name, Description, DueDate 
                           FROM Project
                           WHERE Id = ?";
    }

    protected override void FillSelectItemStatement(ISQLiteStatement statement, long key)
    {
      statement.Bind(1, key);
    }

    protected override string GetInsertItemSql()
    {
      return "INSERT INTO Project (CustomerId, Name, Description, DueDate) VALUES (?, ?, ?, ?)";
    }

    protected override void FillInsertStatement(ISQLiteStatement statement, Project item)
    {
      statement.Bind(1, item.CustomerId);
      statement.Bind(2, item.Name);
      statement.Bind(3, item.Description);
      statement.Bind(4, item.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    protected override string GetUpdateItemSql()
    {
      return "UPDATE Project SET CustomerId = ?, Name = ?, Description = ?, DueDate = ? WHERE Id = ?";
    }

    protected override void FillUpdateStatement(ISQLiteStatement statement, long key, Project item)
    {
      FillInsertStatement(statement, item);
      statement.Bind(5, key);
    }

    protected override string GetDeleteItemSql()
    {
      return "DELETE FROM Project WHERE Id = ?";
    }

    protected override void FillDeleteItemStatement(ISQLiteStatement statement, long key)
    {
      statement.Bind(1, key);
    }
  }
}
