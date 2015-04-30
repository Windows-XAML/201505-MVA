using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Template10.Services.NavigationService
{
    public class NavigationService
    {
        #region Constants
        private const string EmptyNavigation = "1,0";
        #endregion // Constants

        #region Member Variables
        private readonly NavigationFacade frame;
        #endregion // Member Variables


        #region Constructors
        /// <summary>
        /// Initialzies a new <see cref="NavigationService"/> instance for the specified frame.
        /// </summary>
        /// <param name="frame">
        /// The <see cref="Frame"/> that the navigation service will wrap.
        /// </param>
        public NavigationService(Frame frame)
        {
            this.frame = new NavigationFacade(frame);
            this.frame.Navigating += (s, e) => NavigateFrom(false);
            this.frame.Navigated += (s, e) => NavigateTo(e.NavigationMode, e.Parameter);
        }
        #endregion // Constructors

        string LastNavigationParameter { get; set; /* TODO: persist */ }
        string LastNavigationType { get; set; /* TODO: persist */ }


        void NavigateFrom(bool suspending)
        {
            var page = frame.Content as FrameworkElement;
            if (page != null)
            {
                var dataContext = page.DataContext as INavigatable;
                if (dataContext != null)
                {
                    dataContext.OnNavigatedFrom(null, suspending);
                }
            }
        }

        private void NavigateTo(NavigationMode mode, string parameter)
        {
            LastNavigationParameter = parameter;
            LastNavigationType = frame.Content.GetType().FullName;

            if (mode == NavigationMode.New)
            {
                // TODO: clear existing state
            }

            var page = frame.Content as FrameworkElement;
            if (page != null)
            {
                var dataContext = page.DataContext as INavigatable;
                if (dataContext != null)
                {
                    dataContext.OnNavigatedTo(parameter, mode, null);
                }
            }
        }

        public bool Navigate(Type page, string parameter = null)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            if (page.FullName.Equals(LastNavigationType)
                && parameter == LastNavigationParameter)
                return false;
            return frame.Navigate(page, parameter);
        }

        public void RestoreSavedNavigation() { /* TODO */ }

        public void GoBack() { if (frame.CanGoBack) frame.GoBack(); }

        public bool CanGoBack { get { return frame.CanGoBack; } }

        public void GoForward() { frame.GoForward(); }

        public bool CanGoForward { get { return frame.CanGoForward; } }

        public void ClearHistory() { frame.SetNavigationState(EmptyNavigation); }

        public void Suspending() { NavigateFrom(true); }

        public void Show(SettingsFlyout flyout, string parameter = null)
        {
            if (flyout == null)
                throw new ArgumentNullException(nameof(flyout));
            var dataContext = flyout.DataContext as INavigatable;
            if (dataContext != null)
            {
                dataContext.OnNavigatedTo(parameter, NavigationMode.New, null);
            }
            flyout.Show();
        }

        public Type CurrentPageType { get { return frame.CurrentPageType; } }

        public string CurrentPageParam { get { return frame.CurrentPageParam; } }
    }
}

