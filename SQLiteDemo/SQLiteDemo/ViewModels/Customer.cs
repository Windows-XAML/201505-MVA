
namespace SQLiteDemo.ViewModels
{
  public class Customer : ViewModelBase
  {
    #region Properties

    private long id = -1;
    public long Id
    {
      get { return id; }
      set { SetProperty(ref id, value); }
    }

    private string name = string.Empty;
    public string Name
    {
      get { return name; }
      set { if (SetProperty(ref name, value)) IsDirty = true; }
    }

    private string city = string.Empty;
    public string City
    {
      get { return city; }
      set { if (SetProperty(ref city, value)) IsDirty = true; }
    }

    private string contact = string.Empty;
    public string Contact
    {
      get { return contact; }
      set { if (SetProperty(ref contact, value)) IsDirty = true; }
    }

    private bool isDirty = false;
    public bool IsDirty
    {
      get { return isDirty; }
      set { SetProperty(ref isDirty, value); }
    }

    #endregion "Properties"

    internal Customer()
    { 
    }

    internal Customer(long id, string name, string city, string contact)
    {
      this.id = id;
      this.name = name;
      this.city = city;
      this.contact = contact;
      this.isDirty = false;
    }

    public bool IsNew { get { return Id < 0; } }
  }
}
