using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SerialTestUI.Converters
{
    public class BoolToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush retColor = new SolidColorBrush();
            retColor.Color = Colors.Black;
            if ((bool)value)
            {
                retColor.Color = Colors.Green;
            }
            else
            {
                retColor.Color = Colors.Black;
            }
            return retColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
