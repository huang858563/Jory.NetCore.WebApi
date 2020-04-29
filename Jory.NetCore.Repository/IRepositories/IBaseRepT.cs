using Jory.NetCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Jory.NetCore.Model.Models;

namespace Jory.NetCore.Repository.IRepositories
{
    public interface IBaseRep<TEntity> : IRepository where TEntity : class, new()
    {
        Task<TEntity> QueryById(object objId, bool blnUseCache = false);

        Task<List<TEntity>> QueryByIDs(object[] lstIds);

        Task<int> Add(TEntity model, Expression<Func<TEntity, object>> insertColumns = null);

        Task<int> Add(List<TEntity> listEntity);

        Task<bool> DeleteById(object id);

        Task<bool> Delete(TEntity model);

        Task<bool> DeleteByIds(object[] ids);

        Task<bool> Update(TEntity entity, string strWhere);

        Task<bool> Update(object operateAnonymousObjects);

        Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null,
            string strWhere = "");

        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string strWhere);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFields);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression,
            Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);

        Task<List<TEntity>> Query(string strWhere, string strOrderByFields);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFields);
        Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds);

        Task<List<TEntity>> Query(
            Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize,
            string strOrderByFields);

        Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFields);


        Task<PagedList<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1,
            int intPageSize = 20, string strOrderByFields = null);

        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
    }
}
