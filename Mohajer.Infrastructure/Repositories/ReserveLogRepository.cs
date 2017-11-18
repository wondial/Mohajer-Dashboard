using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Mohajer.Infrastructure.Repositories
{
    public class ReserveLogRepository : Repository<ReserveLog, int>, IReserveLogRepository
    {
        public override ReserveLog FindOne(int key) => _collection.FindById(key);

        public IEnumerable<ReserveLog> Get(int numbers)
        {
            return _collection.Include(p => p.Food).FindAll().OrderByDescending(p => p.TimeStamp).Take(numbers);
        }

        public override void Insert(params ReserveLog[] values) => _collection.Insert(values);
        public override void Remove(params int[] keys) => _collection.Delete(p => keys.Contains(p.Id));
        public override void Update(params ReserveLog[] values) => _collection.Update(values);
    }
}