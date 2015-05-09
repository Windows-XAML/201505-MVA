using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SQLiteDemo.Converters
{
    class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = "";

            if (value is DateTime)
            {
                DateTime theDate = (DateTime)value;
                result = string.Format("{0:d}", theDate);
            }
            else
            {
                DateTime theDate = DateTime.Now;
                result = string.Format("{0:d}", theDate);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToDateTime(value);
        }
    }
}
