using Mohajer.Core.Models;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mohajer.Infrastructure.Repositories
{
    public class StudentRepository : Repository<Student, int>, IStudentRepository
    {
        public override Student FindOne(int key) => _collection.FindById(key);

        public IEnumerable<Food> GetFoods(int id) => _collection.Include(p => p.Foods).FindById(id).Foods;

        public override void Insert(params Student[] values) => _collection.Insert(values);
        public override void Remove(params int[] keys) => _collection.Delete(p => keys.Contains(p.UserName));
        public override void Update(params Student[] values) => _collection.Update(values);
    }
}
