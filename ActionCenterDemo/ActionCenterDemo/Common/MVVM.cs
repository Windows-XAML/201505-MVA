using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace ActionCenterDemo.Common
{
  // this is an MVVM abstraction, allowing for any MVVM framework to be used
  // the default implementation is MVVM light, but it is not required

  // command is an implementation ICommand for buttons
  public class Command
      : GalaSoft.MvvmLight.Command.RelayCommand
  {
    public Command(Action execute) : base(execute) { }
    public Command(Action execute, Func<bool> canExecute) : base(execute, canExecute) { }
  }

  // command<T> is an implementation of ICommand with parameters for buttons
  public class Command<T>
      : GalaSoft.MvvmLight.Command.RelayCommand<T>
  {
    public Command(Action<T> execute) : base(execute) { }
    public Command(Action<T> execute, Func<T, bool> canExecute) : base(execute, canExecute) { }
  }

  // viewmodelbase enables To/From, called by NavigationService
  public abstract class ViewModelBase
      : GalaSoft.MvvmLight.ViewModelBase
  {
    public virtual void OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state) { }
    public virtual void OnNavigatedFrom(Dictionary<string, object> state, bool suspending) { }
  }

  // imessage is what messenger sends
  public interface IMessage { }

  // message is a generic implementation of imessage
  public class Message<T>
      : IMessage
  { public T Payload { get; set; } }

  // use messenger to communicate between viewmodels
  public class Messenger : GalaSoft.MvvmLight.Messaging.Messenger
  {
    public void Subscribe<T>(object recipient, Action<T> action)
        where T : IMessage
    { base.Register<T>(recipient, action); }

    public void Unsubscribe<T>(object recipient)
        where T : IMessage
    { base.Unregister<T>(recipient); }

    public void Publish<T>(T message)
        where T : IMessage
    { base.Send<T>(message); }
  }
}
