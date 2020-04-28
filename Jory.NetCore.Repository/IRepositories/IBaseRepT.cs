using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Jory.NetCore.Core.Interfaces;
using Jory.NetCore.Model.Entities;

namespace Jory.NetCore.Repository.IRepositories
{
    public interface IBaseRep<T, in TKey>: IRepository,IDisposable where T :BaseEntity<TKey>
    {
        #region 查询

        T GetById(params TKey[] ids);

        Task<T> GetByIdAsync(params TKey[] ids);

        T Get(Expression<Func<T, bool>> whereLambda);

        IQueryable<T> GetList();

        IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda);

        (IQueryable<T>,int) GetPagedList(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, bool>> orderByLambda);

        #endregion

        #region 插入

        int Insert(T model, IDbTransaction transaction =null);

        Task<int> InsertAsync(T model, IDbTransaction transaction = null);

        int InsertList(List<T> models, IDbTransaction transaction = null);

        Task<int> InsertListAsync(List<T> models, IDbTransaction transaction = null);

        #endregion

        #region 更新

        bool Update(T model, IDbTransaction transaction = null);

        Task<bool> UpdateAsync(T model, IDbTransaction transaction = null);

        int Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda);

        Task<int> UpdateAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda);

        #endregion

        #region 删除

        bool Delete(T model, IDbTransaction transaction = null);

        Task<bool> DeleteAsync(T model, IDbTransaction transaction = null);

        int Delete(params TKey[] ids);

        Task<int> DeleteAsync(params TKey[] ids);

        int Delete(Expression<Func<T, bool>> whereLambda);
        Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda);
        #endregion

        bool IsExist(params TKey[] ids);

        bool IsExist(Expression<Func<T, bool>> whereLambda);

        int SaveChanges();
    }
}
