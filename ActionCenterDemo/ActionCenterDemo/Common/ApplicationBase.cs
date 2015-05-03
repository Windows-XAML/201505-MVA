using System;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;

namespace ActionCenterDemo.Common
{
  // ApplicationBase is a drop-in replacement of Application
  // - OnInitializeAsync is the first in the pipeline, if launching
  // - OnLaunched[*]Async is required, and second in the pipeline
  // - OnActivated[*]Async is first in the pipeline, if activating
  // - NavigationService is an automatic property of this class
  public abstract partial class ApplicationBase : Application
  {
    public ApplicationBase()
    {
      this.Resuming += (s, e) => HandleResuming(s, e);
      this.Suspending += async (s, e) =>
      {
        var deferral = e.SuspendingOperation.GetDeferral();
        this.NavigationService.Suspending();
        await HandleSuspendingAsync(s, e);
        deferral.Complete();
      };
    }

    public Services.NavigationService NavigationService { get; private set; }

    protected virtual void HandleResuming(object s, object e) { }
    protected virtual Task HandleSuspendingAsync(object s, SuspendingEventArgs e) { return Task.FromResult<object>(null); }

    public Frame RootFrame { get; set; }
    protected Func<SplashScreen, Page> SplashFactory { get; set; }
    protected virtual Task OnInitializeAsync() { return Task.FromResult<object>(null); }

    protected virtual Task OnActivatedAsync(IActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnActivatedByProtocolAsync(ProtocolActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnActivatedByPrimaryTileAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnActivatedBySecondaryTileAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnActivatedByToastNotificationAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }

    protected override async void OnActivated(IActivatedEventArgs e)
    {
      switch (e.Kind)
      {
        // See http://go.microsoft.com/fwlink/?LinkID=288842
        case ActivationKind.Launch:
          var args = e as LaunchActivatedEventArgs;
          if (args.TileId == "App" && !args.Arguments.Any())
          { await OnActivatedByPrimaryTileAsync(args); }
          else if (args.TileId == "App" && args.Arguments.Any())
          { await OnActivatedBySecondaryTileAsync(args); }
          else
          { await OnActivatedByToastNotificationAsync(args); }
          break;
        case ActivationKind.Protocol:
        case ActivationKind.ProtocolForResults:
          await OnActivatedByProtocolAsync(e as ProtocolActivatedEventArgs);
          break;
        default:
          break;
      }
      // this is to handle any other type of activation
      await this.OnActivatedAsync(e);
      Window.Current.Activate();
    }

    protected virtual Task OnLaunchedAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnLaunchedByProtocolAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnLaunchedByPrimaryTileAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnLaunchedBySecondaryTileAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }
    protected virtual Task OnLaunchedByToastNotificationAsync(LaunchActivatedEventArgs e) { return Task.FromResult<object>(null); }

    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
      if (this.SplashFactory != null)
      {
        Window.Current.Content = this.SplashFactory(e.SplashScreen);
        Window.Current.Activate();
        Window.Current.Content = null;
      }

      this.RootFrame = this.RootFrame ?? new Frame();
      this.RootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
      this.NavigationService = new Services.NavigationService(this.RootFrame);

      // the user may override to set custom content
      await OnInitializeAsync();

      // if the user didn't set custom content, use frame
      if (Window.Current.Content == null)
      { Window.Current.Content = this.RootFrame; }

      if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
      {
        try { /* TODO: restore state */ }
        catch { /* TODO: handle fail */ }
      }
      else
      {
        switch (e.Kind)
        {
          // See http://go.microsoft.com/fwlink/?LinkID=288842
          case ActivationKind.Launch:
            if (e.TileId == "App" && !e.Arguments.Any())
            { await OnLaunchedByPrimaryTileAsync(e); }
            else if (e.TileId == "App" && e.Arguments.Any())
            { await OnLaunchedByToastNotificationAsync(e); }
            else
            { await OnLaunchedBySecondaryTileAsync(e); }
            break;
          case ActivationKind.ProtocolForResults:
          case ActivationKind.Protocol:
            await OnLaunchedByProtocolAsync(e);
            break;
          default:
            break;
        }
        // this is to handle any other type of launch
        await this.OnLaunchedAsync(e);
      }
      Window.Current.Activate();
    }
  }
}
