using Caliburn.Micro;
using MD.PersianDateTime;
using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mohajer.Desktop.ViewModels
{
    public class SelectableFoodViewModel : PropertyChangedBase
    {
        public float Price { get; set; }

        public SelectableFoodViewModel(Food food)
        {
            Food = food;

            if (food.Status == FoodStatus.ReserverdAndChangeable || food.Status == FoodStatus.ToBeReserved || food.Status  == FoodStatus.ToBeUnreserved)
                IsSelected = true;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        public Food Food { get; set; }
    }
}
