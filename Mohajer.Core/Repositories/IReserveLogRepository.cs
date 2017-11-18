using Mohajer.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Core.Repositories
{
    public interface IReserveLogRepository : IRepository<ReserveLog, int>
    {
        IEnumerable<ReserveLog> Get(int numbers);
    }
}
