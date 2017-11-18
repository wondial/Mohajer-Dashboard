using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Core.Repositories
{
    public interface IRepository<TEntity, TKey> : IDisposable
    {
        void Insert(params TEntity[] values);
        void Update(params TEntity[] values);
        TEntity FindOne(TKey key);
        void Remove(params TKey[] keys);
        void Clear();
    }
}
