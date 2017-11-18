using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mohajer.Core.Models
{
    public class Food 
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = "نامشخص";
        public string SideDishTitle { get; set; } = "ندارد";
        public MealType MealType { get; set; } = MealType.Lunch;
        public MealCost MealCost { get; set; } = MealCost.None;
        public FoodStatus Status { get; set; } = FoodStatus.Unknown;
        public int PersianDate { get; set; }
        public bool IsHoliday { get; set; }
    } 


    public enum FoodStatus
    {
        Reservable,
        ReserverdAndUnchangeable,
        ReserverdAndChangeable,
        NotReservable,
        Unknown,
        ToBeReserved,
        ToBeUnreserved
    }

    public enum MealCost
    {
        High = 20,
        Normal = 19,
        Low = 18,
        None = 0
    }

    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner
    }
}
