
namespace Shop.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using Windows.ApplicationModel.Core;
    using Windows.UI.Core;

    public  class CanNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            bool result = false;

            if (!object.Equals(storage, value))
            {
                storage = value;
                this.OnPropertyChanged(propertyName);

                result = true;
            }

            return result;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            this.OnPropertyChanged(PropertyName(propertyExpression));
        }

        protected void RaiseAllPropertyChanged()
        {
            this.OnPropertyChanged(string.Empty);
        }

        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            eventHandler(this, new PropertyChangedEventArgs(propertyName));
                        });
            }
        }

        private static string PropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression != null)
            {
                var memberExpression = propertyExpression.Body as MemberExpression;

                if (memberExpression != null && memberExpression.Member != null)
                {
                    return memberExpression.Member.Name;
                }
            }

            return null;
        }
    }
}
