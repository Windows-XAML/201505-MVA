using Windows.UI.Xaml.Controls;

namespace Blank10.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            this.DataContextChanged += (s, e) => ViewModel = DataContext as ViewModels.MainPageViewModel;
            Windows.UI.Xaml.Window.Current.SizeChanged += (s, e) =>
            {
                this.Text00.Text = string.Format("Screen size: {0}x{1}", (int)e.Size.Width, (int)e.Size.Height);
            };
        }

        // strongly-typed view models enable x:bind
        public ViewModels.MainPageViewModel ViewModel { get; set; }
    }
}