using MD.PersianDateTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mohajer.Desktop.Converters
{
    public class GregorianToPersianDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return PersianDateTime.Parse((int)value).ToLongDateString();
            }
            else
            {
                if ((string)parameter == "time")
                {
                    return new PersianDateTime((DateTime)value).ToString();
                }
                else
                {
                    return new PersianDateTime((DateTime)value).ToShortDateString();
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
