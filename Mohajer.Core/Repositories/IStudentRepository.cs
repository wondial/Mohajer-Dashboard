using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Core.Repositories
{
    public interface IStudentRepository : IRepository<Student, int>
    {
        IEnumerable<Food> GetFoods(int id);
    }
}
