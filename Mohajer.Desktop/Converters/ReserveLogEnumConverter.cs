using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mohajer.Desktop.Converters
{
    public class ReserveLogEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReserveOperation)
            {
                var temp = (ReserveOperation)value;

                switch (temp)
                {
                    case ReserveOperation.Unreserve:
                        return "لغو رزرو";
                    default:
                        return "رزرو";
                }
            }
            else
            {
                var temp = (ReserveResult)value;

                switch (temp)
                {
                    case ReserveResult.Successful:
                        return "موفق";
                    case ReserveResult.NotEnoughMoney:
                        return "اعتبار ناکافی";
                    default:
                        return "خطا در اتصال";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
