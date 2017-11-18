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
    public class MealCostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = (MealCost)value;

            switch (temp)
            {
                case MealCost.High:
                    return "پر هزینه";
                case MealCost.Normal:
                    return "متوسط هزینه";
                case MealCost.Low:
                    return "کم هزینه";
                default:
                    return "نامشخص";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
