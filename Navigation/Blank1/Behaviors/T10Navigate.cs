using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Blank1.Behaviors
{
    
    class T10Navigate : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            if (Target != null)
            {
                var app = App.Current as Common.BootStrapper;
                var nav = app.NavigationService;
                var type = Type.GetType(Target);
                nav.Navigate(type, Parameter ?? string.Empty);
            }
            return null;
        }

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(T10Navigate), new PropertyMetadata(null));

        public string Parameter
        {
            get { return (string)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(string), typeof(T10Navigate), new PropertyMetadata(null));
    }
}
