using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Blank1.Triggers
{
    public class OrientationTrigger : StateTriggerBase
    {
        public OrientationTrigger()
        {
            var win = Window.Current;
            WeakEvent.Subscribe<WindowSizeChangedEventHandler>(win, nameof(win.SizeChanged), Window_SizeChanged);
            CalculateState();
        }

        private void CalculateState()
        {
            var currentOrientation = ApplicationViewOrientation.Landscape;
            var window = Window.Current;
            if (window.Bounds.Width >= window.Bounds.Height)
            { currentOrientation = ApplicationViewOrientation.Landscape; }
            else { currentOrientation = ApplicationViewOrientation.Portrait; }
            SetActive(currentOrientation == orientation);
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            CalculateState();
        }

        private ApplicationViewOrientation orientation;
        public ApplicationViewOrientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    CalculateState();
                }
            }
        }
    }
}
