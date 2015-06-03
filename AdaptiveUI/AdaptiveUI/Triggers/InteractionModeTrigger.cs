using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace AdaptiveUI.Triggers
{
    class InteractionModeTrigger : StateTriggerBase
    {
        public InteractionModeTrigger()
        {
            Window.Current.SizeChanged += (s, e) => Update();
        }

        private UserInteractionMode _mode;
        public UserInteractionMode Mode
        {
            get { return _mode; }
            set { _mode = value; Update(); }
        }

        void Update()
        {
            var viewSettings = Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView();
            SetActive(viewSettings.UserInteractionMode.Equals(Mode));
        }
    }
}
