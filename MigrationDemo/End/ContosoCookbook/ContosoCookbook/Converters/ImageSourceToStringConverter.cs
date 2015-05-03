using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ContosoCookbook.Converters
{
    public class ImageSourceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var file = value.ToString();
            if (!file.Contains("/")) return string.Empty;
            var start = file.IndexOf("/") + 1;
            var end = file.IndexOf(".");
            return string.Format("Created Date: {0}", file.Substring(start, (end - start)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
