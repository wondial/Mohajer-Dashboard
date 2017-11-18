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
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (FoodStatus)value;

            switch (result)
            {
                case FoodStatus.Reservable:
                    return "قابل رزور";
                case FoodStatus.NotReservable:
                    return "غیر قابل رزرو";
                case FoodStatus.ReserverdAndUnchangeable:
                    return "رزرو شده و غیر قابل تغییر";
                case FoodStatus.ReserverdAndChangeable:
                    return "رزرو شده و قابل تغییر";
                case FoodStatus.ToBeReserved:
                    return "رزرو خواهد شد";
                case FoodStatus.ToBeUnreserved:
                    return "رزرو لغو خواهد شد";
                default:
                    return "مشخص نیست";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
