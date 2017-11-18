using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Core.Repositories
{
    public interface IActiveStudent
    {
        Student Current { get; }
        void InsertFood(IEnumerable<Food> foods);

        void ClearAllInformation();
    }
}
