using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Template10.Views
{
    public sealed partial class Shell : Page
    {
        private Frame contentFrame;

        public Shell(Frame frame)
        {
            this.contentFrame = frame;
            this.InitializeComponent();
            this.ShellSplitView.Content = frame;
            var update = new Action(() =>
            {
                // update radiobuttons after frame navigates
                var type = frame.CurrentSourcePageType;
                foreach (var radioButton in AllRadioButtons(this))
                {
                    var target = radioButton.CommandParameter as NavType;
                    if (target == null)
                        continue;
                    radioButton.IsChecked = target.Type.Equals(type);
                }
                this.ShellSplitView.IsPaneOpen = false;
                this.BackCommand.RaiseCanExecuteChanged();
            });
            frame.Navigated += (s, e) => update();
            this.Loaded += (s, e) => update();
            this.DataContext = this;
        }

        // back
        Mvvm.Command _backCommand;
        public Mvvm.Command BackCommand { get { return _backCommand ?? (_backCommand = new Mvvm.Command(ExecuteBack, CanBack)); } }
        private bool CanBack()
        {
            return this.contentFrame.CanGoBack;
        }
        private void ExecuteBack()
        {
            this.contentFrame.GoBack();
        }

        // menu
        Mvvm.Command _menuCommand;
        public Mvvm.Command MenuCommand { get { return _menuCommand ?? (_menuCommand = new Mvvm.Command(ExecuteMenu)); } }
        private void ExecuteMenu()
        {
            this.ShellSplitView.IsPaneOpen = !this.ShellSplitView.IsPaneOpen;
        }

        // nav
        Mvvm.Command<NavType> _navCommand;

        public Mvvm.Command<NavType> NavCommand { get { return _navCommand ?? (_navCommand = new Mvvm.Command<NavType>(ExecuteNav)); } }
        private void ExecuteNav(NavType navType)
        {
            var type = navType.Type;

            this.contentFrame.Navigate(navType.Type);
            // when we nav home, clear history
            if (type.Equals(typeof(Views.MainPage)))
            {
                this.contentFrame.BackStack.Clear();
                if(this.BackCommand != null)
                {
                    this.BackCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // utility
        public List<RadioButton> AllRadioButtons(DependencyObject parent)
        {
            var list = new List<RadioButton>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is RadioButton)
                {
                    list.Add(child as RadioButton);
                    continue;
                }
                list.AddRange(AllRadioButtons(child));
            }
            return list;
        }

        // prevent check
        private void DontCheck(object s, RoutedEventArgs e)
        {
            // don't let the radiobutton check
            (s as RadioButton).IsChecked = false;
        }
    }

    public class NavType
    {
        public Type Type { get; set; }
        public string Parameter { get; set; }
    }
}
