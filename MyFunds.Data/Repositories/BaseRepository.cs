using Microsoft.EntityFrameworkCore;
using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFunds.Data.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected MyFundsDbContext Context;
        protected DbSet<TEntity> Table;

        public BaseRepository(MyFundsDbContext context)
        {
            Context = context;
            Table = Context.Set<TEntity>();
        }

        public void Delete(int id)
        {
            TEntity existing = Table.Find(id);
            Table.Remove(existing);
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        public TEntity GetById(int id)
        {
            return Table.Find(id);
        }

        public TEntity Insert(TEntity entity)
        {
            Table.Add(entity);
            return entity;
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public void Detach(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public TEntity Update(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate);
        }

        public bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Any(predicate);
        }

    }
}
