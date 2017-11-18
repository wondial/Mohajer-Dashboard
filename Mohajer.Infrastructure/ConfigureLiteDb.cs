using LiteDB;
using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Infrastructure
{
    public static class ConfigureLiteDb
    {
        private static readonly BsonMapper _global = BsonMapper.Global;

        public static void Configure()
        {
            _global.Entity<Student>().Id(p => p.UserName);
            _global.Entity<Food>().Id(p => p.PersianDate);

            _global.Entity<Student>().DbRef(p => p.Foods);

            _global.Entity<ReserveLog>().DbRef(p => p.Food);
        }
    }
}
