using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Jory.NetCore.Model.Entities;
using Jory.NetCore.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Jory.NetCore.Repository.Repositories
{
    public class BaseRep<T, TKey> : IBaseRep<T, TKey> where T:BaseEntity<TKey>
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRep(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public bool Delete(T model, IDbTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Delete(params TKey[] ids)
        {
            throw new NotImplementedException();
        }

        public int Delete(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(T model, IDbTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(params TKey[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T Get(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public T GetById(params TKey[] ids)
        {
            return _dbSet.FirstOrDefault(x => ids.Contains(x.Id));
        }

        public Task<T> GetByIdAsync(params TKey[] ids)
        {
            return _dbSet.FirstOrDefaultAsync(x => ids.Contains(x.Id));
        }

        public IQueryable<T> GetList()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            return _dbSet.Where(whereLambda);
        }

        public (IQueryable<T>, int) GetPagedList(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, bool>> orderByLambda)
        {
            var queryable = _dbSet.Where(whereLambda).OrderBy(orderByLambda).Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
            var rowCount = _dbSet.Where(whereLambda).Count();
            return (queryable, rowCount);
        }

        public int Insert(T model, IDbTransaction transaction = null)
        {
            _dbSet.Add(model);
            return _dbContext.SaveChanges();
        }

        public Task<int> InsertAsync(T model, IDbTransaction transaction = null)
        {
            _dbSet.AddAsync(model);
            return _dbContext.SaveChangesAsync();
        }

        public int InsertList(List<T> models, IDbTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertListAsync(List<T> models, IDbTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(params TKey[] ids)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public bool Update(T model, IDbTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(T model, IDbTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda)
        {
            throw new NotImplementedException();
        }
    }
}
