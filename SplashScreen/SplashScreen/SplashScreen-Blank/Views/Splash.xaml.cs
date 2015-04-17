using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Sample.Views
{
    public sealed partial class Splash : Page
    {
        public Splash(Windows.ApplicationModel.Activation.SplashScreen splashScreen,
               Nullable<Color> splashBackground = null, Uri splashImage = null)
        {
            this.InitializeComponent();

            // defaults
            if (splashBackground == null)
                splashBackground = Colors.Red;
            this.Background = new SolidColorBrush(splashBackground.Value);
            if (splashImage == null)
                splashImage = new Uri("ms-appx:///Assets/SplashScreen.png");
            SplashImage.ImageOpened += (s, e) => Window.Current.Activate();
            SplashImage.ImageFailed += (s, e) => Window.Current.Activate();
            SplashImage.Source = new BitmapImage(splashImage);

            // setup resize
            Action resize = () =>
            {
                SplashImage.Stretch = Stretch.Uniform;
                SplashImage.Height = splashScreen.ImageLocation.Height;
                SplashImage.Width = splashScreen.ImageLocation.Width;
                var t = new TranslateTransform
                {
                    X = splashScreen.ImageLocation.Left,
                    Y = splashScreen.ImageLocation.Top,
                };
                SplashImage.RenderTransform = t;
            };

            // init
            Window.Current.SizeChanged += (s, e) => resize();
            resize.Invoke();
        }
    }
}
