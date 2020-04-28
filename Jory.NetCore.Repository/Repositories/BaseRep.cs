using Jory.NetCore.Core.Extensions;
using Jory.NetCore.Core.Helpers;
using Jory.NetCore.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jory.NetCore.Core.Enums;

namespace Jory.NetCore.Repository.Repositories
{
    public class BaseRep : IBaseRep
    {
        #region Field

        /// <summary>
        /// 私有数据库连接字符串
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// 私有事务对象
        /// </summary>
        private DbTransaction _transaction;

        /// <summary>
        /// 私有超时时长
        /// </summary>
        private int _commandTimeout = 240;

        #endregion

        #region Property

        /// <summary>
        /// 超时时长，默认240s
        /// </summary>
        public int CommandTimeout
        {
            get => _commandTimeout;
            set
            {
                _commandTimeout = value;
                DbContext.Database.SetCommandTimeout(_commandTimeout);
            }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString ?? DbContext.Database.GetDbConnection().ConnectionString;
            set
            {
                _connectionString = value;
                DbContext.Database.GetDbConnection().ConnectionString = _connectionString;
            }
        }

        /// <summary>
        /// 事务对象
        /// </summary>
        public DbTransaction Transaction
        {
            get => _transaction ?? DbContext.Database.CurrentTransaction?.GetDbTransaction();
            set
            {
                _transaction = value;
                DbContext.Database.UseTransaction(_transaction);
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbCategory DbCategory { get; set; }

        /// <summary>
        /// DbContext
        /// </summary>
        public DbContext DbContext { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">DbContext实例</param>
        /// <param name="dbCategory"></param>
        public BaseRep(DbContext context, DbCategory dbCategory)
        {
            DbContext = context;
            DbCategory = dbCategory;
            DbContext.Database.SetCommandTimeout(_commandTimeout);
        }

        #endregion

        #region Transaction

        #region Sync

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>IRepository</returns>
        public IBaseRep BeginTrans()
        {
            DbContext.Database.BeginTransaction();
            return this;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            DbContext.Database.CommitTransaction();
            DbContext.Database.CurrentTransaction?.Dispose();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            DbContext.Database.RollbackTransaction();
            DbContext.Database.CurrentTransaction?.Dispose();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            DbContext.Database.CloseConnection();
            DbContext.Dispose();
        }

        #endregion

        #region Async

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>IRepository</returns>
        public async Task<IBaseRep> BeginTransAsync()
        {
            await DbContext.Database.BeginTransactionAsync();
            return this;
        }

        #endregion

        #endregion

        #region ExecuteBySql

        #region Sync

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回受影响行数</returns>
        public int ExecuteBySql(string sql)
        {
            return DbContext.Database.ExecuteSqlRaw(sql);
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        public int ExecuteBySql(string sql, params object[] parameter)
        {
            return DbContext.Database.ExecuteSqlRaw(sql, parameter);
        }

        /// <summary>
        /// 执行sql存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        ///  <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        public int ExecuteByProc(string procName, params DbParameter[] parameter)
        {
            return DbContext.ExecuteProc(procName, parameter);
        }

        /// <summary>
        /// 执行sql存储过程查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteByProc<T>(string procName, params DbParameter[] parameter)
        {
            return DbContext.ExecuteProc<T>(procName, parameter);
        }

        #endregion

        #region Async

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> ExecuteBySqlAsync(string sql)
        {
            return await DbContext.Database.ExecuteSqlRawAsync(sql);
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> ExecuteBySqlAsync(string sql, params object[] parameter)
        {
            return await DbContext.Database.ExecuteSqlRawAsync(sql, parameter);
        }

        /// <summary>
        /// 执行sql存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        ///  <param name="parameter">对应参数</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> ExecuteByProcAsync(string procName, params DbParameter[] parameter)
        {
            return await DbContext.ExecuteProcAsync(procName, parameter);
        }

        /// <summary>
        /// 执行sql存储过程查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteByProcAsync<T>(string procName, params DbParameter[] parameter)
        {
            return await DbContext.ExecuteProcAsync<T>(procName, parameter);
        }

        #endregion

        #endregion

        #region Insert

        #region Sync

        /// <summary>
        ///  插入单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要插入的实体</param>
        /// <returns>返回受影响行数</returns>
        public int Insert<T>(T entity) where T : class
        {
            DbContext.Set<T>().Add(entity);
            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 插入多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要插入的实体集合</param>
        /// <returns>返回受影响行数</returns>
        public int Insert<T>(IEnumerable<T> entities) where T : class
        {
            DbContext.Set<T>().AddRange(entities);
            return DbContext.SaveChanges();
        }

        #endregion

        #region Async

        /// <summary>
        ///  插入单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要插入的实体</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> InsertAsync<T>(T entity) where T : class
        {
            await DbContext.Set<T>().AddAsync(entity);
            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 插入多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要插入的实体集合</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : class
        {
            await DbContext.Set<T>().AddRangeAsync(entities);
            return await DbContext.SaveChangesAsync();
        }

        #endregion

        #endregion

        #region Delete

        #region Sync

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>返回受影响行数</returns>
        public int Delete<T>() where T : class
        {
            var entities = FindList<T>();
            return Delete(entities);
        }

        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要删除的实体</param>
        /// <returns>返回受影响行数</returns>
        public int Delete<T>(T entity) where T : class
        {
            DbContext.Set<T>().Remove(entity);
            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要删除的实体集合</param>
        /// <returns>返回受影响行数</returns>
        public int Delete<T>(IEnumerable<T> entities) where T : class
        {
            DbContext.Set<T>().RemoveRange(entities);
            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns>返回受影响行数</returns>
        public int Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var entities = FindList(predicate);
            return Delete(entities);
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="keyValues">主键值</param>
        /// <returns>返回受影响行数</returns>
        public int Delete<T>(params object[] keyValues) where T : class
        {
            var entity = FindEntity<T>(keyValues);
            return entity != null ? Delete(entity) : 0;
        }

        #endregion

        #region Async

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>返回受影响行数</returns>
        public async Task<int> DeleteAsync<T>() where T : class
        {
            var entities = await FindListAsync<T>();
            return await DeleteAsync(entities);
        }

        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要删除的实体</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> DeleteAsync<T>(T entity) where T : class
        {
            DbContext.Set<T>().Remove(entity);
            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要删除的实体集合</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : class
        {
            DbContext.Set<T>().RemoveRange(entities);
            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var entities = await FindListAsync(predicate);
            return await DeleteAsync(entities);
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="keyValues">主键值</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> DeleteAsync<T>(params object[] keyValues) where T : class
        {
            var entity = await FindEntityAsync<T>(keyValues);
            if (entity != null)
            {
                return await DeleteAsync(entity);
            }

            return 0;
        }

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
        public int Update<T>(T entity) where T : class
        {
            DbContext.Set<T>().Attach(entity);
            var entry = DbContext.Entry(entity);
            var props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                //非null且非PrimaryKey
                if (prop.GetValue(entity, null) != null && !entry.Property(prop.Name).Metadata.IsPrimaryKey())
                {
                    entry.Property(prop.Name).IsModified = true;
                }
            }

            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要更新的实体集合</param>
        /// <returns>返回受影响行数</returns>
        public int Update<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                DbContext.Set<T>().Attach(entity);
                var entry = DbContext.Entry(entity);
                var props = entity.GetType().GetProperties();
                foreach (var prop in props)
                {
                    //非null且非PrimaryKey
                    if (prop.GetValue(entity, null) != null && !entry.Property(prop.Name).Metadata.IsPrimaryKey())
                    {
                        entry.Property(prop.Name).IsModified = true;
                    }
                }
            }

            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        public int Update<T>(Expression<Func<T, bool>> predicate, T entity) where T : class
        {
            var entities = new List<T>();
            var instances = FindList(predicate);
            //设置所有状态为未跟踪状态
            DbContext.ChangeTracker.Entries<T>().ToList().ForEach(o => o.State = EntityState.Detached);
            foreach (var instance in instances)
            {
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var isKey = property.GetCustomAttributes(typeof(KeyAttribute), false).Any();
                    if (!isKey) continue;
                    var keyVal = property.GetValue(instance);
                    if (keyVal != null)
                        property.SetValue(entity, keyVal);
                }

                //深度拷贝实体，避免列表中所有实体引用地址都相同
                entities.Add(MapperHelper<T, T>.MapTo(entity));
            }

            return Update<T>(entities);
        }

        #endregion

        #region Async

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> UpdateAsync<T>(T entity) where T : class
        {
            DbContext.Set<T>().Attach(entity);
            var entry = DbContext.Entry(entity);
            var props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                //非null且非PrimaryKey
                if (prop.GetValue(entity, null) != null && !entry.Property(prop.Name).Metadata.IsPrimaryKey())
                {
                    entry.Property(prop.Name).IsModified = true;
                }
            }

            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="entities">要更新的实体集合</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                DbContext.Set<T>().Attach(entity);
                var entry = DbContext.Entry(entity);
                var props = entity.GetType().GetProperties();
                foreach (var prop in props)
                {
                    //非null且非PrimaryKey
                    if (prop.GetValue(entity, null) != null && !entry.Property(prop.Name).Metadata.IsPrimaryKey())
                    {
                        entry.Property(prop.Name).IsModified = true;
                    }
                }
            }

            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="entity">要更新的实体</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> predicate, T entity) where T : class
        {
            var entities = new List<T>();
            var instances = await FindListAsync(predicate);
            //设置所有状态为未跟踪状态
            DbContext.ChangeTracker.Entries<T>().ToList().ForEach(o => o.State = EntityState.Detached);
            foreach (var instance in instances)
            {
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var isKey = property.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0;
                    if (isKey)
                    {
                        var keyVal = property.GetValue(instance);
                        if (keyVal != null)
                            property.SetValue(entity, keyVal);
                    }
                }

                //深度拷贝实体，避免列表中所有实体引用地址都相同
                entities.Add(MapperHelper<T, T>.MapTo(entity));
            }

            return await UpdateAsync<T>(entities);
        }

        #endregion

        #endregion

        #region FindObject

        #region Sync

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果对象</returns>
        public object FindObject(string sql)
        {
            return FindObject(sql, null);
        }

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果对象</returns>
        public object FindObject(string sql, params DbParameter[] parameter)
        {
            return DbContext.SqlQuery<Dictionary<string, object>>(sql, parameter)?.FirstOrDefault()
                ?.Select(o => o.Value).FirstOrDefault();
        }

        #endregion

        #region Async

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果对象</returns>
        public async Task<object> FindObjectAsync(string sql)
        {
            return await FindObjectAsync(sql, null);
        }

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果对象</returns>
        public async Task<object> FindObjectAsync(string sql, params DbParameter[] parameter)
        {
            return (await DbContext.SqlQueryAsync<Dictionary<string, object>>(sql, parameter))?.FirstOrDefault()
                ?.Select(o => o.Value).FirstOrDefault();
        }

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
        public T FindEntity<T>(params object[] keyValues) where T : class
        {
            return DbContext.Set<T>().Find(keyValues);
        }

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        public T FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DbContext.Set<T>().Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        public TS FindEntity<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate)
            where T : class
        {
            return DbContext.Set<T>().Where(predicate).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回实体</returns>
        public T FindEntityBySql<T>(string sql)
        {
            return FindEntityBySql<T>(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        public T FindEntityBySql<T>(string sql, params DbParameter[] parameter)
        {
            var query = DbContext.SqlQuery<T>(sql, parameter);
            if (query != null)
            {
                return query.FirstOrDefault();
            }

            return default;
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        public T FindEntityBySql<T>(string sql, params object[] parameter) where T : class
        {
            return DbContext.Set<T>().FromSqlRaw(sql, parameter).FirstOrDefault();
        }

        #endregion

        #region Async

        /// <summary>
        /// 根据主键查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="keyValues">主键值</param>
        /// <returns>返回实体</returns>
        public async Task<T> FindEntityAsync<T>(params object[] keyValues) where T : class
        {
            return await DbContext.Set<T>().FindAsync(keyValues);
        }

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        public async Task<T> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await DbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回实体</returns>
        public async Task<TS> FindEntityAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate) where T : class
        {
            return await DbContext.Set<T>().Where(predicate).Select(selector).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回实体</returns>
        public async Task<T> FindEntityBySqlAsync<T>(string sql)
        {
            return await FindEntityBySqlAsync<T>(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        public async Task<T> FindEntityBySqlAsync<T>(string sql, params DbParameter[] parameter)
        {
            var query = await DbContext.SqlQueryAsync<T>(sql, parameter);
            if (query != null)
            {
                return query.FirstOrDefault();
            }

            return default;
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回实体</returns>
        public async Task<T> FindEntityBySqlAsync<T>(string sql, params object[] parameter) where T : class
        {
            return await DbContext.Set<T>().FromSqlRaw(sql, parameter).FirstOrDefaultAsync();
        }

        #endregion

        #endregion

        #region IQueryable

        #region Sync

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        public IQueryable<T> Queryable<T>() where T : class
        {
            return DbContext.Set<T>();
        }

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        public IQueryable<TS> Queryable<T, TS>(Expression<Func<T, TS>> selector) where T : class
        {
            return DbContext.Set<T>().Select(selector);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public IQueryable<T> Queryable<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DbContext.Set<T>().Where(predicate);
        }

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public IQueryable<T> Queryable<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (!(orderField.Body is NewExpression newExpression))
                return orderTypes.FirstOrDefault() == OrderByDirection.Descending
                    ? query.OrderByDescending(orderField)
                    : query.OrderBy(orderField);
            IOrderedQueryable<T> order = null;
            for (var i = 0; i < newExpression.Members.Count; i++)
            {
                //指定排序类型
                if (i <= orderTypes.Length - 1)
                {
                    if (orderTypes[i] == OrderByDirection.Descending)
                    {
                        order = i > 0
                            ? order.ThenByDescending(newExpression.Members[i].Name)
                            : query.OrderByDescending(newExpression.Members[i].Name);
                    }
                    else
                    {
                        order = i > 0
                            ? order.OrderBy(newExpression.Members[i].Name)
                            : query.OrderBy(newExpression.Members[i].Name);
                    }
                }
                else
                {
                    order = i > 0
                        ? order.ThenBy(newExpression.Members[i].Name)
                        : query.OrderBy(newExpression.Members[i].Name);
                }
            }

            return order;

        }

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public IQueryable<TS> Queryable<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate)
            where T : class
        {
            return DbContext.Set<T>().Where(predicate).Select(selector);
        }

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
        public IQueryable<TS> Queryable<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0
                                ? order.ThenByDescending(newExpression.Members[i].Name)
                                : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0
                                ? order.ThenBy(newExpression.Members[i].Name)
                                : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0
                            ? order.ThenBy(newExpression.Members[i].Name)
                            : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return (order ?? throw new ArgumentNullException()).Select(selector);
            }
            //单个字段排序

            return orderTypes.FirstOrDefault() == OrderByDirection.Descending
                ? query.OrderByDescending(orderField).Select(selector)
                : query.OrderBy(orderField).Select(selector);
        }

        #endregion

        #region Async

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        public async Task<IQueryable<T>> QueryableAsync<T>() where T : class
        {
            return await Task.FromResult(DbContext.Set<T>());
        }

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        public async Task<IQueryable<TS>> QueryableAsync<T, TS>(Expression<Func<T, TS>> selector) where T : class
        {
            return await Task.FromResult(DbContext.Set<T>().Select(selector));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public async Task<IQueryable<T>> QueryableAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await Task.FromResult(DbContext.Set<T>().Where(predicate));
        }

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public async Task<IQueryable<T>> QueryableAsync<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0
                                ? order.ThenByDescending(newExpression.Members[i].Name)
                                : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0
                                ? order.ThenBy(newExpression.Members[i].Name)
                                : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0
                            ? order.ThenBy(newExpression.Members[i].Name)
                            : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return await Task.FromResult(order);
            }
            //单个字段排序

            if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                return await Task.FromResult(query.OrderByDescending(orderField));
            else
                return await Task.FromResult(query.OrderBy(orderField));
        }

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public async Task<IQueryable<TS>> QueryableAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate) where T : class
        {
            return await Task.FromResult(DbContext.Set<T>().Where(predicate).Select(selector));
        }

        /// <summary>
        /// 根据条查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public async Task<IQueryable<TS>> QueryableAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0
                                ? order.ThenByDescending(newExpression.Members[i].Name)
                                : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0
                                ? order.ThenBy(newExpression.Members[i].Name)
                                : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0
                            ? order.ThenBy(newExpression.Members[i].Name)
                            : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return await Task.FromResult((order ?? throw new ArgumentNullException()).Select(selector));
            }
            //单个字段排序
            else
            {
                if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                    return await Task.FromResult(query.OrderByDescending(orderField).Select(selector));
                else
                    return await Task.FromResult(query.OrderBy(orderField).Select(selector));
            }
        }

        #endregion

        #endregion

        #region FindList

        #region Sync

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>() where T : class
        {
            return DbContext.Set<T>().ToList();
        }

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        public IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector) where T : class
        {
            return DbContext.Set<T>().Select(selector).ToList();
        }

        /// <summary>
        /// 查询全部并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>(Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes)
            where T : class
        {
            var query = DbContext.Set<T>();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            if (i > 0)
                                order = order.ThenByDescending(newExpression.Members[i].Name);
                            else
                                order = query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            if (i > 0)
                                order = order.ThenBy(newExpression.Members[i].Name);
                            else
                                order = query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        if (i > 0)
                            order = order.ThenBy(newExpression.Members[i].Name);
                        else
                            order = query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return (order ?? throw new ArgumentNullException()).ToList();
            }
            //单个字段排序
            else
            {
                if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                    return query.OrderByDescending(orderField).ToList();
                else
                    return query.OrderBy(orderField).ToList();
            }
        }

        /// <summary>
        /// 查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            if (i > 0)
                                order = order.ThenByDescending(newExpression.Members[i].Name);
                            else
                                order = query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            if (i > 0)
                                order = order.ThenBy(newExpression.Members[i].Name);
                            else
                                order = query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        if (i > 0)
                            order = order.ThenBy(newExpression.Members[i].Name);
                        else
                            order = query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return (order ?? throw new ArgumentNullException()).Select(selector).ToList();
            }
            //单个字段排序
            else
            {
                if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                    return query.OrderByDescending(orderField).Select(selector).ToList();
                else
                    return query.OrderBy(orderField).Select(selector).ToList();
            }
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DbContext.Set<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            if (i > 0)
                                order = order.ThenByDescending(newExpression.Members[i].Name);
                            else
                                order = query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            if (i > 0)
                                order = order.ThenBy(newExpression.Members[i].Name);
                            else
                                order = query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        if (i > 0)
                            order = order.ThenBy(newExpression.Members[i].Name);
                        else
                            order = query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return (order ?? throw new ArgumentNullException()).ToList();
            }
            //单个字段排序
            else
            {
                if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                    return query.OrderByDescending(orderField).ToList();
                else
                    return query.OrderBy(orderField).ToList();
            }
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate)
            where T : class
        {
            return DbContext.Set<T>().Where(predicate).Select(selector).ToList();
        }

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
        public IEnumerable<TS> FindList<T, TS>(Expression<Func<T, TS>> selector, Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            if (i > 0)
                                order = order.ThenByDescending(newExpression.Members[i].Name);
                            else
                                order = query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            if (i > 0)
                                order = order.ThenBy(newExpression.Members[i].Name);
                            else
                                order = query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        if (i > 0)
                            order = order.ThenBy(newExpression.Members[i].Name);
                        else
                            order = query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return (order ?? throw new ArgumentNullException()).Select(selector).ToList();
            }
            //单个字段排序
            else
            {
                if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                    return query.OrderByDescending(orderField).Select(selector).ToList();
                else
                    return query.OrderBy(orderField).Select(selector).ToList();
            }
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>(string sql)
        {
            return FindList<T>(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>(string sql, params DbParameter[] parameter)
        {
            return DbContext.SqlQuery<T>(sql, parameter);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> FindList<T>(string sql, params object[] parameter) where T : class
        {
            return DbContext.Set<T>().FromSqlRaw(sql, parameter).ToList();
        }

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
        public (IEnumerable<T> list, long total) FindList<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, int pageSize, int pageIndex, params OrderByDirection[] orderTypes)
            where T : class
        {
            IOrderedQueryable<T> order = null;
            var query = DbContext.Set<T>().Where(predicate);
            var total = query.Count();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }
            }
            //单个字段排序
            else
            {
                order = orderTypes.FirstOrDefault() == OrderByDirection.Descending ? query.OrderByDescending(orderField) : query.OrderBy(orderField);
            }

            //分页
            query = (order ?? throw new ArgumentNullException()).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return (query.ToList(), total);
        }

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
        public (IEnumerable<TS> list, long total) FindList<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField, int pageSize, int pageIndex,
            params OrderByDirection[] orderTypes) where T : class
        {
            IOrderedQueryable<T> order = null;
            var query = DbContext.Set<T>().Where(predicate);
            var total = query.Count();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }
            }
            //单个字段排序
            else
            {
                order = orderTypes.FirstOrDefault() == OrderByDirection.Descending ? query.OrderByDescending(orderField) : query.OrderBy(orderField);
            }

            //分页
            query = (order ?? throw new ArgumentNullException()).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return (query.Select(selector).ToList(), total);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        public (List<T> list, long total) FindList<T>(string sql, string orderField, bool isAscending, int pageSize,
            int pageIndex)
        {
            return FindList<T>(sql, null, orderField, isAscending, pageSize, pageIndex);
        }

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
        public (List<T> list, long total) FindList<T>(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase)
                    ? $"ORDER BY {orderField}"
                    : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            var type = typeof(T);
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if(orderField.IsNullOrEmpty())
                    {
                        orderField = "ORDER BY (SELECT 0)";
                    }

                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                  
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};SELECT * INTO #TEMPORARY_{guid} FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};SELECT * FROM (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber, * FROM #TEMPORARY_{guid}) AS N WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};SELECT * INTO #TEMPORARY_{guid} FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};SELECT * FROM (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber, * FROM #TEMPORARY_{guid}) AS N WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.MySql:
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>(
                            $"DROP TEMPORARY TABLE IF EXISTS $TEMPORARY_{guid};CREATE TEMPORARY TABLE $TEMPORARY_{guid} SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM $TEMPORARY_{guid};SELECT * FROM $TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE $TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>(
                            $"DROP TEMPORARY TABLE IF EXISTS $TEMPORARY_{guid};CREATE TEMPORARY TABLE $TEMPORARY_{guid} SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM $TEMPORARY_{guid};SELECT * FROM $TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE $TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.Sqlite:
                {
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>(
                            $"SELECT COUNT(1) AS Total FROM ({sql}) AS T;SELECT * FROM ({sql}) AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>(
                            $"SELECT COUNT(1) AS Total FROM ({sql}) AS T;SELECT * FROM ({sql}) AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.Oracle:
                {
                    var total = DbContext.SqlQuery<long>($"SELECT COUNT(1) AS Total FROM ({sql}) T", parameter).FirstOrDefault();
                    var list = DbContext.SqlQuery<T>($"SELECT * FROM (SELECT X.*,ROWNUM AS RowNumber FROM ({sql} {orderField}) X WHERE ROWNUM <= {pageSize * pageIndex}) T WHERE T.RowNumber >= {pageSize * (pageIndex - 1) + 1}", parameter);
                    return (list?.ToList(), total);
                    }
                case DbCategory.NpgSql:
                    {
                        var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                        if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" && type.Name != "String")
                        {
                            var query = DbContext.SqlQueryMultiple<dynamic>($"DROP TABLE IF EXISTS TEMPORARY_{guid};CREATE TEMPORARY TABLE TEMPORARY_{guid} AS SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM TEMPORARY_{guid};SELECT * FROM TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE TEMPORARY_{guid};", parameter);
                            return ((query.LastOrDefault() ?? throw new ArgumentNullException()).Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(), Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault()?.Total ?? 0));
                        }
                        else
                        {
                            var query = DbContext.SqlQueryMultiple<T>($"DROP TABLE IF EXISTS TEMPORARY_{guid};CREATE TEMPORARY TABLE TEMPORARY_{guid} AS SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM TEMPORARY_{guid};SELECT * FROM TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE TEMPORARY_{guid};", parameter);
                            return (query.LastOrDefault(), Convert.ToInt64(((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as IDictionary<string, object>)?["Total"] ?? 0));
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
        public (List<T> list, long total) FindListByWith<T>(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase)
                    ? $"ORDER BY {orderField}"
                    : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            var type = typeof(T);
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                     orderField = "ORDER BY (SELECT 0)";
                    }

                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                  
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};{sql} SELECT * INTO #TEMPORARY_{guid} FROM T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};WITH R AS (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber,* FROM #TEMPORARY_{guid}) SELECT * FROM R  WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND RowNumber {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};{sql} SELECT * INTO #TEMPORARY_{guid} FROM T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};WITH R AS (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber,* FROM #TEMPORARY_{guid}) SELECT * FROM R  WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND RowNumber {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.MySql:
                {
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()
                                            ?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.Sqlite:
                {
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" && type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return ((query.LastOrDefault() ?? throw new ArgumentNullException()).Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(), Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return (query.LastOrDefault(), Convert.ToInt64(((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as IDictionary<string, object>)?["Total"] ?? 0));
                    }
                    }

                case DbCategory.Oracle:
                {
                    var total = DbContext.SqlQuery<long>($"{sql} SELECT COUNT(1) AS Total FROM T", parameter).FirstOrDefault();
                    var list = DbContext.SqlQuery<T>($"{sql},R AS (SELECT ROWNUM AS RowNumber,T.* FROM T WHERE ROWNUM <= {pageSize * pageIndex} {orderField}) SELECT * FROM R WHERE RowNumber>={pageSize * (pageIndex - 1) + 1}", parameter);
                    return (list?.ToList(), total);
                    }

                case DbCategory.NpgSql:
                {
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" && type.Name != "String")
                    {
                        var query = DbContext.SqlQueryMultiple<dynamic>($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return ((query.LastOrDefault() ?? throw new ArgumentNullException()).Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(), Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = DbContext.SqlQueryMultiple<T>($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return (query.LastOrDefault(), Convert.ToInt64(((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as IDictionary<string, object>)?["Total"] ?? 0));
                    }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>() where T : class
        {
            return await DbContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector) where T : class
        {
            return await DbContext.Set<T>().Select(selector).ToListAsync();
        }

        /// <summary>
        /// 查询全部并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>(Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return await (order ?? throw new ArgumentNullException()).ToListAsync();
            }
            //单个字段排序

            if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                return await query.OrderByDescending(orderField).ToListAsync();
            return await query.OrderBy(orderField).ToListAsync();
        }

        /// <summary>
        /// 查询指定列并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return await (order ?? throw new ArgumentNullException()).Select(selector).ToListAsync();
            }
            //单个字段排序

            if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                return await query.OrderByDescending(orderField).Select(selector).ToListAsync();
            return await query.OrderBy(orderField).Select(selector).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await DbContext.Set<T>().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>        
        /// <param name="predicate">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderTypes">排序类型，默认正序排序</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return await (order ?? throw new ArgumentNullException()).ToListAsync();
            }
            //单个字段排序

            if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                return await query.OrderByDescending(orderField).ToListAsync();
            return await query.OrderBy(orderField).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询指定列
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <typeparam name="TS">泛型类型</typeparam>
        /// <param name="selector">选择指定列</param>
        /// <param name="predicate">条件</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate) where T : class
        {
            return await DbContext.Set<T>().Where(predicate).Select(selector).ToListAsync();
        }

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
        public async Task<IEnumerable<TS>> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField,
            params OrderByDirection[] orderTypes) where T : class
        {
            var query = DbContext.Set<T>().Where(predicate);
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                IOrderedQueryable<T> order = null;
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }

                return await (order ?? throw new ArgumentNullException()).Select(selector).ToListAsync();
            }
            //单个字段排序

            if (orderTypes.FirstOrDefault() == OrderByDirection.Descending)
                return await query.OrderByDescending(orderField).Select(selector).ToListAsync();

            return await query.OrderBy(orderField).Select(selector).ToListAsync();
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>(string sql)
        {
            return await FindListAsync<T>(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>(string sql, params DbParameter[] parameter)
        {
            return await DbContext.SqlQueryAsync<T>(sql, parameter);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回集合</returns>
        public async Task<IEnumerable<T>> FindListAsync<T>(string sql, params object[] parameter) where T : class
        {
            return await DbContext.Set<T>().FromSqlRaw(sql, parameter).ToListAsync();
        }

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
        public async Task<(IEnumerable<T> list, long total)> FindListAsync<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderField, int pageSize, int pageIndex, params OrderByDirection[] orderTypes)
            where T : class
        {
            IOrderedQueryable<T> order = null;
            var query = DbContext.Set<T>().Where(predicate);
            var total = await query.CountAsync();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }
            }
            //单个字段排序
            else
            {
                order = orderTypes.FirstOrDefault() == OrderByDirection.Descending ? query.OrderByDescending(orderField) : query.OrderBy(orderField);
            }

            //分页
            query = (order ?? throw new ArgumentNullException()).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return (await query.ToListAsync(), total);
        }

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
        public async Task<(IEnumerable<TS> list, long total)> FindListAsync<T, TS>(Expression<Func<T, TS>> selector,
            Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderField, int pageSize, int pageIndex,
            params OrderByDirection[] orderTypes) where T : class
        {
            IOrderedQueryable<T> order = null;
            var query = DbContext.Set<T>().Where(predicate);
            var total = await query.CountAsync();
            //多个字段排序
            if (orderField.Body is NewExpression newExpression)
            {
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    //指定排序类型
                    if (i <= orderTypes.Length - 1)
                    {
                        if (orderTypes[i] == OrderByDirection.Descending)
                        {
                            order = i > 0 ? order.ThenByDescending(newExpression.Members[i].Name) : query.OrderByDescending(newExpression.Members[i].Name);
                        }
                        else
                        {
                            order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                        }
                    }
                    else
                    {
                        order = i > 0 ? order.ThenBy(newExpression.Members[i].Name) : query.OrderBy(newExpression.Members[i].Name);
                    }
                }
            }
            //单个字段排序
            else
            {
                order = orderTypes.FirstOrDefault() == OrderByDirection.Descending ? query.OrderByDescending(orderField) : query.OrderBy(orderField);
            }

            //分页
            query = (order ?? throw new ArgumentNullException()).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return (await query.Select(selector).ToListAsync(), total);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回集合和总记录数</returns>
        public async Task<(List<T> list, long total)> FindListAsync<T>(string sql, string orderField, bool isAscending,
            int pageSize, int pageIndex)
        {
            return await FindListAsync<T>(sql, null, orderField, isAscending, pageSize, pageIndex);
        }

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
        public async Task<(List<T> list, long total)> FindListAsync<T>(string sql, DbParameter[] parameter,
            string orderField, bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase)
                    ? $"ORDER BY {orderField}"
                    : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            var type = typeof(T);
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                      orderField = "ORDER BY (SELECT 0)";
                    }

                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};SELECT * INTO #TEMPORARY_{guid} FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};SELECT * FROM (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber, * FROM #TEMPORARY_{guid}) AS N WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};SELECT * INTO #TEMPORARY_{guid} FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};SELECT * FROM (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber, * FROM #TEMPORARY_{guid}) AS N WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.MySql:
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"DROP TEMPORARY TABLE IF EXISTS $TEMPORARY_{guid};CREATE TEMPORARY TABLE $TEMPORARY_{guid} SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM $TEMPORARY_{guid};SELECT * FROM $TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE $TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()
                                            ?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"DROP TEMPORARY TABLE IF EXISTS $TEMPORARY_{guid};CREATE TEMPORARY TABLE $TEMPORARY_{guid} SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM $TEMPORARY_{guid};SELECT * FROM $TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE $TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.Sqlite:
                {
                   if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" && type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>($"SELECT COUNT(1) AS Total FROM ({sql}) AS T;SELECT * FROM ({sql}) AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return ((query.LastOrDefault() ?? throw new ArgumentNullException()).Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(), Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>($"SELECT COUNT(1) AS Total FROM ({sql}) AS T;SELECT * FROM ({sql}) AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return (query.LastOrDefault(), Convert.ToInt64(((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as IDictionary<string, object>)?["Total"] ?? 0));
                    }
                    }

                case DbCategory.Oracle:
                {
                    var total = (await DbContext.SqlQueryAsync<long>($"SELECT COUNT(1) AS Total FROM ({sql}) T", parameter)).FirstOrDefault();
                    var list = await DbContext.SqlQueryAsync<T>($"SELECT * FROM (SELECT X.*,ROWNUM AS RowNumber FROM ({sql} {orderField}) X WHERE ROWNUM <= {pageSize * pageIndex}) T WHERE T.RowNumber >= {pageSize * (pageIndex - 1) + 1}", parameter);
                    return (list?.ToList(), total);
                    }

                case DbCategory.NpgSql:
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"DROP TABLE IF EXISTS TEMPORARY_{guid};CREATE TEMPORARY TABLE TEMPORARY_{guid} AS SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM TEMPORARY_{guid};SELECT * FROM TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"DROP TABLE IF EXISTS TEMPORARY_{guid};CREATE TEMPORARY TABLE TEMPORARY_{guid} AS SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM TEMPORARY_{guid};SELECT * FROM TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
        public async Task<(List<T> list, long total)> FindListByWithAsync<T>(string sql, DbParameter[] parameter,
            string orderField, bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase) ? $"ORDER BY {orderField}" : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            var type = typeof(T);
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                        orderField = "ORDER BY (SELECT 0)";
                    }

                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                   
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"IF OBJECT_ID(N'tempdb..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};{sql} SELECT * INTO #TEMPORARY_{guid} FROM T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};WITH R AS (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber,* FROM #TEMPORARY_{guid}) SELECT * FROM R  WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND RowNumber {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};{sql} SELECT * INTO #TEMPORARY_{guid} FROM T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};WITH R AS (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber,* FROM #TEMPORARY_{guid}) SELECT * FROM R  WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND RowNumber {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.MySql:
                {
                   if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()
                                            ?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.Sqlite:
                {
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException())
                            .Select(o => (o as IDictionary<string, object>).ToEntity<T>()).ToList(),
                            Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException())
                                            .FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as
                                    IDictionary<string, object>)?["Total"] ?? 0));
                    }
                }

                case DbCategory.Oracle:
                {
                    var total = (await DbContext.SqlQueryAsync<long>($"{sql} SELECT COUNT(1) AS Total FROM T", parameter)).FirstOrDefault();
                    var list = await DbContext.SqlQueryAsync<T>($"{sql},R AS (SELECT ROWNUM AS RowNumber,T.* FROM T WHERE ROWNUM <= {pageSize * pageIndex} {orderField}) SELECT * FROM R WHERE RowNumber>={pageSize * (pageIndex - 1) + 1}", parameter);
                    return (list?.ToList(), total);
                    }

                case DbCategory.NpgSql:
                {
                    if (!type.Name.Contains("Dictionary`2") && type.IsClass && type.Name != "Object" &&
                        type.Name != "String")
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<dynamic>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (
                            (query.LastOrDefault() ?? throw new ArgumentNullException()).Select(o => (o as IDictionary<string, object>).ToEntity<T>())
                                .ToList(), Convert.ToInt64((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault()?.Total ?? 0));
                    }
                    else
                    {
                        var query = await DbContext.SqlQueryMultipleAsync<T>(
                            $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                            parameter);
                        return (query.LastOrDefault(),
                            Convert.ToInt64(
                                ((query.FirstOrDefault() ?? throw new ArgumentNullException()).FirstOrDefault() as IDictionary<string, object>)?["Total"] ??
                                0));
                    }
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #endregion

        #region FindTable

        #region Sync

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataTable</returns>
        public DataTable FindTable(string sql)
        {
            return FindTable(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回DataTable</returns>
        public DataTable FindTable(string sql, params DbParameter[] parameter)
        {
            return DbContext.SqlDataTable(sql, parameter);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>        
        /// <returns>返回DataTable和总记录数</returns>
        public (DataTable table, long total) FindTable(string sql, string orderField, bool isAscending, int pageSize,
            int pageIndex)
        {
            return FindTable(sql, null, orderField, isAscending, pageSize, pageIndex);
        }

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
        public (DataTable table, long total) FindTable(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase) ? $"ORDER BY {orderField}" : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                        orderField = "ORDER BY (SELECT 0)";
                    }
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = DbContext.SqlDataSet($"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};SELECT * INTO #TEMPORARY_{guid} FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};SELECT * FROM (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber, * FROM #TEMPORARY_{guid}) AS N WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.MySql:
                {
                    
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = DbContext.SqlDataSet(
                        $"DROP TEMPORARY TABLE IF EXISTS $TEMPORARY_{guid};CREATE TEMPORARY TABLE $TEMPORARY_{guid} SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM $TEMPORARY_{guid};SELECT * FROM $TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE $TEMPORARY_{guid};",
                        parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Sqlite:
                    {
                       
                        var ds = DbContext.SqlDataSet($"SELECT COUNT(1) AS Total FROM ({sql}) AS T;SELECT * FROM ({sql}) AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Oracle:
                {
                   
                    var total = DbContext.SqlQuery<long>($"SELECT COUNT(1) AS Total FROM ({sql}) T", parameter).FirstOrDefault();
                    var table = DbContext.SqlDataTable($"SELECT * FROM (SELECT X.*,ROWNUM AS RowNumber FROM ({sql} {orderField}) X WHERE ROWNUM <= {pageSize * pageIndex}) T WHERE T.RowNumber >= {pageSize * (pageIndex - 1) + 1}", parameter);
                    return (table, total);
                    }

                case DbCategory.NpgSql:
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = DbContext.SqlDataSet($"DROP TABLE IF EXISTS TEMPORARY_{guid};CREATE TEMPORARY TABLE TEMPORARY_{guid} AS SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM TEMPORARY_{guid};SELECT * FROM TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE TEMPORARY_{guid};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
        public (DataTable table, long total) FindTableByWith(string sql, DbParameter[] parameter, string orderField,
            bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase) ? $"ORDER BY {orderField}" : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                        orderField = "ORDER BY (SELECT 0)";
                    }
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = DbContext.SqlDataSet($"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};{sql} SELECT * INTO #TEMPORARY_{guid} FROM T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};WITH R AS (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber,* FROM #TEMPORARY_{guid}) SELECT * FROM R  WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND RowNumber {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.MySql:
                {
                    var ds = DbContext.SqlDataSet(
                        $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                        parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Sqlite:
                {
                    if (!orderField.IsNullOrEmpty())
                    {
                        if (orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase))
                            orderField = $"ORDER BY {orderField}";
                        else
                            orderField = $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
                    }

                    var ds = DbContext.SqlDataSet(
                        $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                        parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                }

                case DbCategory.Oracle:
                {
                    var total = DbContext.SqlQuery<long>($"{sql} SELECT COUNT(1) AS Total FROM T", parameter).FirstOrDefault();
                    var table = DbContext.SqlDataTable($"{sql},R AS (SELECT ROWNUM AS RowNumber,T.* FROM T WHERE ROWNUM <= {pageSize * pageIndex} {orderField}) SELECT * FROM R WHERE RowNumber>={pageSize * (pageIndex - 1) + 1}", parameter);
                    return (table, total);
                    }

                case DbCategory.NpgSql:
                {
                    var ds = DbContext.SqlDataSet($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataTable</returns>
        public async Task<DataTable> FindTableAsync(string sql)
        {
            return await FindTableAsync(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回DataTable</returns>
        public async Task<DataTable> FindTableAsync(string sql, params DbParameter[] parameter)
        {
            return await DbContext.SqlDataTableAsync(sql, parameter);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns>返回DataTable和总记录数</returns>
        public async Task<(DataTable table, long total)> FindTableAsync(string sql, string orderField, bool isAscending,
            int pageSize, int pageIndex)
        {
            return await FindTableAsync(sql, null, orderField, isAscending, pageSize, pageIndex);
        }

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
        public async Task<(DataTable table, long total)> FindTableAsync(string sql, DbParameter[] parameter,
            string orderField, bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase) ? $"ORDER BY {orderField}" : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                        orderField = "ORDER BY (SELECT 0)";
                    }
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = await DbContext.SqlDataSetAsync($"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};SELECT * INTO #TEMPORARY_{guid} FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};SELECT * FROM (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber, * FROM #TEMPORARY_{guid}) AS N WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.MySql:
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = await DbContext.SqlDataSetAsync(
                        $"DROP TEMPORARY TABLE IF EXISTS $TEMPORARY_{guid};CREATE TEMPORARY TABLE $TEMPORARY_{guid} SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM $TEMPORARY_{guid};SELECT * FROM $TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE $TEMPORARY_{guid};",
                        parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Sqlite:
                    {
                       var ds = await DbContext.SqlDataSetAsync($"SELECT COUNT(1) AS Total FROM ({sql}) AS T;SELECT * FROM ({sql}) AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Oracle:
                {
                    var total = (await DbContext.SqlQueryAsync<long>($"SELECT COUNT(1) AS Total FROM ({sql}) T", parameter)).FirstOrDefault();
                    var table = await DbContext.SqlDataTableAsync($"SELECT * FROM (SELECT X.*,ROWNUM AS RowNumber FROM ({sql} {orderField}) X WHERE ROWNUM <= {pageSize * pageIndex}) T WHERE T.RowNumber >= {pageSize * (pageIndex - 1) + 1}", parameter);
                    return (table, total);
                    }

                case DbCategory.NpgSql:
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = await DbContext.SqlDataSetAsync($"DROP TABLE IF EXISTS TEMPORARY_{guid};CREATE TEMPORARY TABLE TEMPORARY_{guid} AS SELECT * FROM ({sql}) AS T;SELECT COUNT(1) AS Total FROM TEMPORARY_{guid};SELECT * FROM TEMPORARY_{guid} AS X {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};DROP TABLE TEMPORARY_{guid};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
        public async Task<(DataTable table, long total)> FindTableByWithAsync(string sql, DbParameter[] parameter,
            string orderField, bool isAscending, int pageSize, int pageIndex)
        {
            if (!orderField.IsNullOrEmpty())
            {
                orderField = orderField.Contains(@"(/\*(?:|)*?\*/)|(\b(ASC|DESC)\b)", RegexOptions.IgnoreCase) ? $"ORDER BY {orderField}" : $"ORDER BY {orderField} {(isAscending ? "ASC" : "DESC")}";
            }
            switch (DbCategory)
            {
                case DbCategory.SqlServer:
                {
                    if (orderField.IsNullOrEmpty())
                    {
                     orderField = "ORDER BY (SELECT 0)";
                    }
                    var guid = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    var ds = await DbContext.SqlDataSetAsync($"IF OBJECT_ID(N'TEMPDB..#TEMPORARY_{guid}') IS NOT NULL DROP TABLE #TEMPORARY_{guid};{sql} SELECT * INTO #TEMPORARY_{guid} FROM T;SELECT COUNT(1) AS Total FROM #TEMPORARY_{guid};WITH R AS (SELECT ROW_NUMBER() OVER ({orderField}) AS RowNumber,* FROM #TEMPORARY_{guid}) SELECT * FROM R  WHERE RowNumber BETWEEN {((pageIndex - 1) * pageSize + 1)} AND RowNumber {(pageIndex * pageSize)};DROP TABLE #TEMPORARY_{guid};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.MySql:
                {
                   var ds = await DbContext.SqlDataSetAsync(
                        $"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};",
                        parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Sqlite:
                    {
                        var ds = await DbContext.SqlDataSetAsync($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                        return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                case DbCategory.Oracle:
                {
                    var total = (await DbContext.SqlQueryAsync<long>($"{sql} SELECT COUNT(1) AS Total FROM T", parameter)).FirstOrDefault();
                    var table = await DbContext.SqlDataTableAsync($"{sql},R AS (SELECT ROWNUM AS RowNumber,T.* FROM T WHERE ROWNUM <= {pageSize * pageIndex} {orderField}) SELECT * FROM R WHERE RowNumber>={pageSize * (pageIndex - 1) + 1}", parameter);
                    return (table, total);
                    }

                case DbCategory.NpgSql:
                {
                   var ds = await DbContext.SqlDataSetAsync($"{sql} SELECT COUNT(1) AS Total FROM T;{sql} SELECT * FROM T {orderField} LIMIT {pageSize} OFFSET {(pageSize * (pageIndex - 1))};", parameter);
                    return (ds.Tables[1], Convert.ToInt64(ds.Tables[0].Rows[0]["Total"]));
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #endregion

        #region FindMultiple

        #region Sync

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果集</returns>
        public List<List<T>> FindMultiple<T>(string sql)
        {
            return FindMultiple<T>(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果集</returns>
        public List<List<T>> FindMultiple<T>(string sql, params DbParameter[] parameter)
        {
            return DbContext.SqlQueryMultiple<T>(sql, parameter);
        }

        #endregion

        #region Async

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>返回查询结果集</returns>
        public async Task<List<List<T>>> FindMultipleAsync<T>(string sql)
        {
            return await FindMultipleAsync<T>(sql, null);
        }

        /// <summary>
        /// 根据sql语句查询返回多个结果集
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">对应参数</param>
        /// <returns>返回查询结果集</returns>
        public async Task<List<List<T>>> FindMultipleAsync<T>(string sql, params DbParameter[] parameter)
        {
            return await DbContext.SqlQueryMultipleAsync<T>(sql, parameter);
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
