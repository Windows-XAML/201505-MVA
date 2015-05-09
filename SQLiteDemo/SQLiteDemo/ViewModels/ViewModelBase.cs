using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SQLiteDemo.ViewModels
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
      var handler = this.PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    protected bool SetProperty<T>(ref T current, T value, [CallerMemberName] string propertyName = "")
    {
      if (object.Equals(current, value))
        return false;

      current = value;
      RaisePropertyChanged(propertyName);
      return true;
    }
  }
}
