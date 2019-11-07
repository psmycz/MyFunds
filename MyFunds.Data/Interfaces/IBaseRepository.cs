using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFunds.Data.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        bool Exist(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
        TEntity GetById(int id);
        TEntity Insert(TEntity entity);
        void Detach(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(int id);
        void Save();
    }
}
