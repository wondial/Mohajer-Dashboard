using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Mohajer.Core;
using System.Threading.Tasks;
using System.Windows.Data;
using MD.PersianDateTime;

namespace Mohajer.Desktop.Converters
{
    public class StartToEndWeekDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var startDate = ((DateTime)value).StartOfWeek();
            var endDate = startDate.AddDays(6);

            var result = $"از تاریخ {new PersianDateTime(startDate).ToShortDateString()} تا {new PersianDateTime(endDate).ToShortDateString()}";

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
