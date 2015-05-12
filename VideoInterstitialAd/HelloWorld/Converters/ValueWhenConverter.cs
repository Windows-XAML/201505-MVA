using System;
using Windows.UI.Xaml.Data;

namespace Template10.Converters
{
    class ValueWhenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value.Equals(When))
                    return Value;
                return Otherwise;
            }
            catch
            {
                return Otherwise;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object Value { get; set; }
        public object Otherwise { get; set; }
        public object When { get; set; }
    }
}
