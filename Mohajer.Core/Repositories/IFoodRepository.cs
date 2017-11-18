using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Core.Repositories
{
    public interface IFoodRepository : IRepository<Food, int>
    {
        IEnumerable<Food> GetRange(DateTime start, DateTime end);
        IEnumerable<Food> CurrentWeekFoods();
        IEnumerable<Food> NextWeekFoods();
        
    }
}
