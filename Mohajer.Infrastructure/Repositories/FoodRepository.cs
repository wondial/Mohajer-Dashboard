using Mohajer.Core;
using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mohajer.Infrastructure.Repositories
{
    public class FoodRepository : Repository<Food, int>, IFoodRepository
    {
        private DateTime _currentWeekDate = DateTime.Now.StartOfWeek();

        public FoodRepository()
        {
        }

        public IEnumerable<Food> CurrentWeekFoods()
        {
            var startOfWeek = _currentWeekDate;
            var endOfWeek = startOfWeek.AddDays(5);

            return _collection.Find(p => p.Date >= startOfWeek && p.Date <= endOfWeek);
        }

        public IEnumerable<Food> NextWeekFoods()
        {
            var startOfWeek = _currentWeekDate.AddDays(7);
            var endOfWeek = startOfWeek.AddDays(5);

            var result = _collection.Find(p => p.Date >= startOfWeek && p.Date <= endOfWeek);

            return result;
        }

        public override Food FindOne(int key) => _collection.FindById(key);
        public IEnumerable<Food> GetRange(DateTime start, DateTime end) => _collection.Find(p => p.Date >= start && p.Date <= end);
        public override void Insert(params Food[] values) => _collection.Upsert(values);


        public override void Remove(params int[] keys) => throw new NotImplementedException();
        public override void Update(params Food[] values) => _collection.Update(values);
    }
}
