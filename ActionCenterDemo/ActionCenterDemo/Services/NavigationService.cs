using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ActionCenterDemo.Services
{
  public class NavigationService
  {
    NavigationFacade _frame;

    string LastNavigationParameter { get; set; /* TODO: persist */ }

    string LastNavigationType { get; set; /* TODO: persist */ }

    public NavigationService(Frame frame)
    {
      _frame = new NavigationFacade(frame);
      _frame.Navigating += (s, e) => NavigateFrom(false);
      _frame.Navigated += (s, e) => NavigateTo(e.NavigationMode, e.Parameter);
    }

    void NavigateFrom(bool suspending)
    {
      var page = _frame.Content as FrameworkElement;
      if (page != null)
      {
        var viewmodel = page.DataContext as Common.ViewModelBase;
        if (viewmodel != null)
        {
          // TODO: get existing state
          viewmodel.OnNavigatedFrom(null, suspending);
        }
      }
    }

    void NavigateTo(NavigationMode mode, string parameter)
    {
      LastNavigationParameter = parameter;
      LastNavigationType = _frame.Content.GetType().FullName;

      if (mode == NavigationMode.New)
      {
        // TODO: clear existing state
      }

      var page = _frame.Content as FrameworkElement;
      if (page != null)
      {
        var viewmodel = page.DataContext as Common.ViewModelBase;
        if (viewmodel != null)
        {
          // TODO: fetch state
          viewmodel.OnNavigatedTo(parameter, mode, null);
        }
      }
    }

    public bool Navigate(Type page, string parameter = null)
    {
      if (page == null)
        throw new ArgumentNullException("page");
      if (page.FullName.Equals(LastNavigationType) && parameter == LastNavigationParameter)
        return false;
      return _frame.Navigate(page, parameter);
    }

    public void RestoreSavedNavigation() { /* TODO */ }

    public void GoBack() { _frame.GoBack(); }

    public bool CanGoBack { get { return _frame.CanGoBack; } }

    public void GoForward() { _frame.GoForward(); }

    public bool CanGoForward { get { return _frame.CanGoForward; } }

    public void ClearHistory() { _frame.SetNavigationState("1,0"); }

    public void Suspending() { NavigateFrom(true); }

    public void Show(SettingsFlyout flyout, string parameter = null)
    {
      if (flyout == null)
        throw new ArgumentNullException("flyout");
      var viewmodel = flyout.DataContext as Common.ViewModelBase;
      if (viewmodel != null)
        viewmodel.OnNavigatedTo(parameter, NavigationMode.New, null);
      flyout.Show();
    }

    public Type CurrentPageType { get { return _frame.CurrentPageType; } }

    public string CurrentPageParam { get { return _frame.CurrentPageParam; } }
  }

  public class NavigationFacade
  {
    public NavigationFacade(Frame frame)
    {
      _frame = frame;
      _navigatedEventHandlers = new List<EventHandler<NavigationEventArgs>>();
    }

    #region frame facade

    readonly Frame _frame;

    public bool Navigate(Type page, string parameter) { return _frame.Navigate(page, parameter); }

    public void SetNavigationState(string state) { _frame.SetNavigationState(state); }

    public string GetNavigationState() { return _frame.GetNavigationState(); }

    public int BackStackDepth { get { return _frame.BackStackDepth; } }

    public bool CanGoBack { get { return _frame.CanGoBack; } }

    public void GoBack() { _frame.GoBack(); }

    public bool CanGoForward { get { return _frame.CanGoForward; } }

    public void GoForward() { _frame.GoForward(); }

    public object Content { get { return _frame.Content; } }

    public Type CurrentPageType { get; internal set; }

    public string CurrentPageParam { get; internal set; }

    public object GetValue(DependencyProperty dp) { return _frame.GetValue(dp); }

    public void SetValue(DependencyProperty dp, object value) { _frame.SetValue(dp, value); }

    public void ClearValue(DependencyProperty dp) { _frame.ClearValue(dp); }

    #endregion

    readonly List<EventHandler<NavigationEventArgs>> _navigatedEventHandlers;
    public event EventHandler<NavigationEventArgs> Navigated
    {
      add
      {
        if (!_navigatedEventHandlers.Contains(value))
        {
          _navigatedEventHandlers.Add(value);
          if (_navigatedEventHandlers.Count == 1)
            _frame.Navigated += FacadeNavigatedEventHandler;
        }
      }

      remove
      {
        if (_navigatedEventHandlers.Contains(value))
        {
          _navigatedEventHandlers.Remove(value);
          if (_navigatedEventHandlers.Count == 0)
            _frame.Navigated -= FacadeNavigatedEventHandler;
        }
      }
    }

    void FacadeNavigatedEventHandler(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
    {
      foreach (var handler in _navigatedEventHandlers)
      {
        var args = new NavigationEventArgs()
        {
          NavigationMode = e.NavigationMode,
          Parameter = (e.Parameter == null) ? string.Empty : e.Parameter.ToString()
        };
        handler(this, args);
      }
      this.CurrentPageType = e.SourcePageType;
      this.CurrentPageParam = e.Parameter as String;
    }

    readonly List<EventHandler> _navigatingEventHandlers = new List<EventHandler>();
    public event EventHandler Navigating
    {
      add
      {
        if (!_navigatingEventHandlers.Contains(value))
        {
          _navigatingEventHandlers.Add(value);
          if (_navigatingEventHandlers.Count == 1)
            _frame.Navigating += FacadeNavigatingCancelEventHandler;
        }
      }
      remove
      {
        if (_navigatingEventHandlers.Contains(value))
        {
          _navigatingEventHandlers.Remove(value);
          if (_navigatingEventHandlers.Count == 0)
            _frame.Navigating -= FacadeNavigatingCancelEventHandler;
        }
      }
    }

    private void FacadeNavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e)
    {
      foreach (var handler in _navigatingEventHandlers)
      {
        handler(this, new EventArgs());
      }
    }
  }

  public class NavigationEventArgs : EventArgs
  {
    public NavigationMode NavigationMode { get; set; }

    public string Parameter { get; set; }
  }
}

