using System;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;

namespace Template10.Common
{
    // BootStrapper is a drop-in replacement of Application
    // - OnInitializeAsync is the first in the pipeline, if launching
    // - OnLaunchedAsync is required, and second in the pipeline
    // - OnActivatedAsync is first in the pipeline, if activating
    // - NavigationService is an automatic property of this class
    public abstract class BootStrapper : Application
    {
        /// <summary>
        /// Event to allow views and viewmodels to intercept the Hardware/Shell Back request and 
        /// implement their own logic, such as closing a dialog. In your event handler, set the
        /// Handled property of the BackRequestedEventArgs to true if you do not want a Page
        /// Back navigation to occur.
        /// </summary>
        public event EventHandler<Windows.UI.Core.BackRequestedEventArgs> BackRequested;

        public BootStrapper()
        {
            Resuming += (s, e) =>
            {
                OnResuming(s, e);
            };
            Suspending += async (s, e) =>
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                NavigationService.Suspending();
                await OnSuspendingAsync(s, e);
                deferral.Complete();
            };
            this.Dispatcher = Window.Current.Dispatcher;
        }

        #region properties

        public Frame RootFrame { get; set; }
        public CoreDispatcher Dispatcher { get; set; }
        public Services.NavigationService.NavigationService NavigationService { get; private set; }
        protected Func<SplashScreen, Page> SplashFactory { get; set; }

        #endregion

        #region activated

        protected override async void OnActivated(IActivatedEventArgs e) { await InternalActivatedAsync(e); }
        protected override async void OnCachedFileUpdaterActivated(CachedFileUpdaterActivatedEventArgs args) { await InternalActivatedAsync(args); }
        protected override async void OnFileActivated(FileActivatedEventArgs args) { await InternalActivatedAsync(args); }
        protected override async void OnFileOpenPickerActivated(FileOpenPickerActivatedEventArgs args) { await InternalActivatedAsync(args); }
        protected override async void OnFileSavePickerActivated(FileSavePickerActivatedEventArgs args) { await InternalActivatedAsync(args); }
        protected override async void OnSearchActivated(SearchActivatedEventArgs args) { await InternalActivatedAsync(args); }
        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args) { await InternalActivatedAsync(args); }

        private async Task InternalActivatedAsync(IActivatedEventArgs e)
        {
            await OnActivatedAsync(e);
            Window.Current.Activate();
        }

        #endregion

        protected override void OnLaunched(LaunchActivatedEventArgs e) { InternalLaunchAsync(e as ILaunchActivatedEventArgs); }

        private async void InternalLaunchAsync(ILaunchActivatedEventArgs e)
        {
            UIElement splashScreen = default(UIElement);
            if (SplashFactory != null)
            {
                splashScreen = SplashFactory(e.SplashScreen);
                Window.Current.Content = splashScreen;
            }

            RootFrame = RootFrame ?? new Frame();
            RootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
            NavigationService = new Services.NavigationService.NavigationService(RootFrame);

            // the user may override to set custom content
            await OnInitializeAsync();

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                try { /* TODO: restore state */ }
                finally { await OnLaunchedAsync(e); }
            }
            else
            {
                await OnLaunchedAsync(e);
            }

            // if the user didn't already set custom content use rootframe
            if (Window.Current.Content == splashScreen)
            {
                Window.Current.Content = RootFrame;
            }
            if (Window.Current.Content == null)
            {
                Window.Current.Content = RootFrame;
            }
            Window.Current.Activate();

            // Hook up the default Back handler
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        /// <summary>
        /// Default Hardware/Shell Back handler overrides standard Back behavior that navigates to previous app
        /// in the app stack to instead cause a backward page navigation.
        /// Views or Viewodels can override this behavior by handling the BackRequested event and setting the
        /// Handled property of the BackRequestedEventArgs to true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (BackRequested != null)
            {
                BackRequested(this, e);
            }

            if (!e.Handled)
            {
                if (RootFrame.CanGoBack)
                {
                    RootFrame.GoBack();
                    e.Handled = true;
                }
            }
        }

        #region overrides

        public virtual Task OnInitializeAsync() { return Task.FromResult<object>(null); }
        public virtual Task OnActivatedAsync(IActivatedEventArgs e) { return Task.FromResult<object>(null); }
        public abstract Task OnLaunchedAsync(ILaunchActivatedEventArgs e);
        protected virtual void OnResuming(object s, object e) { }
        protected virtual Task OnSuspendingAsync(object s, SuspendingEventArgs e) { return Task.FromResult<object>(null); }

        #endregion
    }
}
