using Jory.NetCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Jory.NetCore.Core.Enums;

namespace Jory.NetCore.Repository.IRepositories
{
    public interface IBaseRep : IRepository, IDisposable
    {
        #region Property

        /// <summary>
        /// 超时时长，默认240s
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        DbTransaction Transaction { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DbCategory DbCategory { get; set; }

        #endregion

        #region Transaction

        #region Sync

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>IRepository</returns>
        IBaseRep BeginTrans();

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();

        #endregion

        #region Async

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>IBaseRep</returns>
        Task<IBaseRep> BeginTransAsync();

        #endregion

        #endregion

        #region ExecuteBySql

        #region Sync

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回受影响行数</returns>
        int ExecuteBySql(string sql);

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        int ExecuteBySql(string sql, params object[] parameter);

        /// <summary>
        /// 执行sql存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        ///  <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        int ExecuteByProc(string procName, params DbParameter[] parameter);

        /// <summary>
        /// 执行sql存储过程查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IEnumerable<T> ExecuteByProc<T>(string procName, params DbParameter[] parameter);

        #endregion

        #region Async

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回受影响行数</returns>
        Task<int> ExecuteBySqlAsync(string sql);

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        Task<int> ExecuteBySqlAsync(string sql, params object[] parameter);

        /// <summary>
        /// 执行sql存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        ///  <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        Task<int> ExecuteByProcAsync(string procName, params DbParameter[] parameter);

        /// <summary>
        /// 执行sql存储过程查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ExecuteByProcAsync<T>(string procName, params DbParameter[] parameter);

        #endregion

        #endregion

        #region Insert

        #region Sync

        /// <summary>
        /// 插入单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要插入的实体</param>
        /// <returns>返回受影响行数</returns>
        int Insert<T>(T entity) where T : class;

        /// <summary>
        /// 插入多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要插入的实体集合</param>
        /// <returns>返回受影响行数</returns>
        int Insert<T>(IEnumerable<T> entities) where T : class;

        #endregion

        #region Async

        /// <summary>
        /// 插入单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要插入的实体</param>
        /// <returns>返回受影响行数</returns>
        Task<int> InsertAsync<T>(T entity) where T : class;

        /// <summary>
        /// 插入多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要插入的实体集合</param>
        /// <returns>返回受影响行数</returns>
        Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : class;

        #endregion

        #endregion

        #region Delete

        #region Sync

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>返回受影响行数</returns>
        int Delete<T>() where T : class;

        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要删除的实体</param>
        /// <returns>返回受影响行数</returns>
        int Delete<T>(T entity) where T : class;

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要删除的实体集合</param>
        /// <returns>返回受影响行数</returns>
        int Delete<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns>返回受影响行数</returns>
        int Delete<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="keyValues">主键值</param>
        /// <returns>返回受影响行数</returns>
        int Delete<T>(params object[] keyValues) where T : class;

        #endregion

        #region Async

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>返回受影响行数</returns>
        Task<int> DeleteAsync<T>() where T : class;

        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要删除的实体</param>
        /// <returns>返回受影响行数</returns>
        Task<int> DeleteAsync<T>(T entity) where T : class;

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要删除的实体集合</param>
        /// <returns>返回受影响行数</returns>
        Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns>返回受影响行数</returns>
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="keyValues">主键值</param>
        /// <returns>返回受影响行数</returns>
        Task<int> DeleteAsync<T>(params object[] keyValues) where T : class;

        #endregion

        #endregion

        #region Update

        #region Sync

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        int Update<T>(T entity) where T : class;

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要更新的实体集合</param>
        /// <returns>返回受影响行数</returns>
        int Update<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        int Update<T>(Expression<Func<T, bool>> predicate, T entity) where T : class;

        #endregion

        #region Async

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        Task<int> UpdateAsync<T>(T entity) where T : class;

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要更新的实体集合</param>
        /// <returns>返回受影响行数</returns>
        Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        Task<int> UpdateAsync<T>(Expression<Func<T, bool>> predicate, T entity) where T : class;

        #endregion

        #endregion

        #region FindObject

        #region Sync

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果对象</returns>
        object FindObject(string sql);

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果对象</returns>
        object FindObject(string sql, params DbParameter[] parameter);

        #endregion

        #region Async

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果对象</returns>
        Task<object> FindObjectAsync(string sql);

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果对象</returns>
        Task<object> FindObjectAsync(string sql, params DbParameter[] parameter);

        #endregion

        #endregion

        #region FindEntity

        #region Sync

        /// <summary>
        /// 根据主键查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="keyValues">主键值</param>
        /// <returns>返回实体</returns>
        T FindEntity<T>(params object[] keyValues) where T : class;

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        T FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        TS FindEntity<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回实体</returns>
        T FindEntityBySql<T>(string sql);

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        T FindEntityBySql<T>(string sql, params DbParameter[] parameter);

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        T FindEntityBySql<T>(string sql, params object[] parameter) where T : class;

        #endregion

        #region Async

        /// <summary>
        /// 根据主键查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="keyValues">主键值</param>
        /// <returns>返回实体</returns>
        Task<T> FindEntityAsync<T>(params object[] keyValues) where T : class;

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        Task<T> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        Task<TS> FindEntityAsync<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate)
            where T : class;

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回实体</returns>
        Task<T> FindEntityBySqlAsync<T>(string sql);

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        Task<T> FindEntityBySqlAsync<T>(string sql, params DbParameter[] parameter);

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        Task<T> FindEntityBySqlAsync<T>(string sql, params object[] parameter) where T : class;

        #endregion

        #endregion

        #region IQueryable

        #region Sync

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>返回集合</returns>
        IQueryable<T> Queryable<T>() where T : class;

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        IQueryable<TS> Queryable<T, TS>(Expression<Func<T, TS>> selector) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        IQueryable<T> Queryable<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        IQueryable<T> Queryable<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        IQueryable<TS> Queryable<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate)
            where T : class;

        /// <summary>
        /// 根据条件查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        IQueryable<TS> Queryable<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class;

        #endregion

        #region Async

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        Task<IQueryable<T>> QueryableAsync<T>() where T : class;

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        Task<IQueryable<TS>> QueryableAsync<T, TS>(Expression<Func<T, TS>> selector) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        Task<IQueryable<T>> QueryableAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        Task<IQueryable<T>> QueryableAsync<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        Task<IQueryable<TS>> QueryableAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        Task<IQueryable<TS>> QueryableAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class;

        #endregion

        #endregion

        #region FindList

        #region Sync

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>() where T : class;

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector) where T : class;

        /// <summary>
        /// 查询全部并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>(Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes)
            where T : class;

        /// <summary>
        /// 查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate)
            where T : class;

        /// <summary>
        /// 根据条件查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>(string sql);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>(string sql, params DbParameter[] parameter);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        IEnumerable<T> FindList<T>(string sql, params object[] parameter) where T : class;

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合和总记录数</returns>
        (IEnumerable<T> list, long total) FindList<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, int pageSize, int pageIndex, params OrderByDirection[] orderTypes)
            where T : class;

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合和总记录数</returns>
        (IEnumerable<TS> list, long total) FindList<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField, int pageSize, int pageIndex,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        (List<T> list, long total) FindList<T>(string sql, string orderField, bool isAscending, int pageSize,
            int pageIndex);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        (List<T> list, long total) FindList<T>(string sql, DbParameter[] parameter, string orderField, bool isAscending,
            int pageSize, int pageIndex);

        /// <summary>
        /// with语法分页查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        (List<T> list, long total) FindListByWith<T>(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        #endregion

        #region Async

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>() where T : class;

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector) where T : class;

        /// <summary>
        /// 查询全部并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>(Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 根据条件查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>(string sql);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>(string sql, params DbParameter[] parameter);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        Task<IEnumerable<T>> FindListAsync<T>(string sql, params object[] parameter) where T : class;

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合和总记录数</returns>
        Task<(IEnumerable<T> list, long total)> FindListAsync<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, int pageSize, int pageIndex, params OrderByDirection[] orderTypes)
            where T : class;

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合和总记录数</returns>
        Task<(IEnumerable<TS> list, long total)> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField, int pageSize, int pageIndex,
            params OrderByDirection[] orderTypes) where T : class;

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        Task<(List<T> list, long total)> FindListAsync<T>(string sql, string orderField, bool isAscending, int pageSize,
            int pageIndex);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        Task<(List<T> list, long total)> FindListAsync<T>(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        /// <summary>
        /// with语法分页查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        Task<(List<T> list, long total)> FindListByWithAsync<T>(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        #endregion

        #endregion

        #region FindTable

        #region Sync

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataTable</returns>
        DataTable FindTable(string sql);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回DataTable</returns>
        DataTable FindTable(string sql, params DbParameter[] parameter);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        (DataTable table, long total) FindTable(string sql, string orderField, bool isAscending, int pageSize,
            int pageIndex);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        (DataTable table, long total) FindTable(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        /// <summary>
        /// with语法分页查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        (DataTable table, long total) FindTableByWith(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        #endregion

        #region Async

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataTable</returns>
        Task<DataTable> FindTableAsync(string sql);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回DataTable</returns>
        Task<DataTable> FindTableAsync(string sql, params DbParameter[] parameter);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        Task<(DataTable table, long total)> FindTableAsync(string sql, string orderField, bool isAscending,
            int pageSize, int pageIndex);

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        Task<(DataTable table, long total)> FindTableAsync(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        /// <summary>
        /// with语法分页查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        Task<(DataTable table, long total)> FindTableByWithAsync(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex);

        #endregion

        #endregion

        #region FindMultiple

        #region Sync

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果集</returns>
        List<List<T>> FindMultiple<T>(string sql);

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果集</returns>
        List<List<T>> FindMultiple<T>(string sql, params DbParameter[] parameter);

        #endregion

        #region Async

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果集</returns>
        Task<List<List<T>>> FindMultipleAsync<T>(string sql);

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果集</returns>
        Task<List<List<T>>> FindMultipleAsync<T>(string sql, params DbParameter[] parameter);

        #endregion

        #endregion
    }
}
