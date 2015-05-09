using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SQLiteDemo.ViewModels
{
  public class Project : ViewModelBase
  {
    #region Properties

    private long id = -1;
    public long Id
    {
      get { return id; }
      //private set { SetProperty(ref id, value); }
    }

    private long customerId = -1;
    public long CustomerId
    {
      get { return customerId; }
      //private set { SetProperty(ref customerId, value); }
    }

    private string name = string.Empty;
    public string Name
    {
      get { return name; }
      set { if (SetProperty(ref name, value)) IsDirty = true; }
    }

    private string description = string.Empty;
    public string Description
    {
      get { return description; }
      set { if (SetProperty(ref description, value)) IsDirty = true; }
    }

    private DateTime dueDate = System.DateTime.Today.AddDays(7);
    public DateTime DueDate
    {
      get { return dueDate; }
      set { if (SetProperty(ref dueDate, value)) IsDirty = true; }
    }

    private bool isDirty = false;
    public bool IsDirty
    {
      get { return isDirty; }
      set { SetProperty(ref isDirty, value); }
    }

    #endregion "Properties"

    internal Project(long customerId)
    {
      this.customerId = customerId;
    }

    public bool IsNew { get { return Id < 0; } }

    internal Project(long id, long customerId, string name, string description, DateTime dueDate)
    {
      this.id = id;
      this.customerId = customerId;
      this.name = name;
      this.description = description;
      this.dueDate = dueDate;
    }
  }
}
