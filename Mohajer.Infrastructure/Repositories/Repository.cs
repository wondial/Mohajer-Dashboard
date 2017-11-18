using LiteDB;
using Mohajer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mohajer.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TKey> : IRepository<TEntity,TKey> , IDisposable
    {
        protected LiteDatabase _liteDatabase = new LiteDatabase("MainDb.db");
        protected LiteCollection<TEntity> _collection;

        public Repository()
        {
            _collection = _liteDatabase.GetCollection<TEntity>();
        }

        public abstract void Insert(params TEntity[] values);

        public abstract void Update(params TEntity[] values);

        public abstract TEntity FindOne(TKey key);

        public abstract void Remove(params TKey[] keys);

        public void Dispose() => _liteDatabase.Dispose();

        public void Clear()
        {
            _collection.Delete(Query.All());
        }
    }
}
