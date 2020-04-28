using System;
using System.Linq;
using System.Linq.Expressions;

namespace Jory.NetCore.Core.Helpers
{
    public static class MapperHelper<T, TF>
    {
        /// <summary>
        /// 私有静态字段
        /// </summary>
        private static readonly Func<T, TF> Map = MapProvider();

        /// <summary>
        /// 私有方法
        /// </summary>
        /// <returns></returns>
        private static Func<T, TF> MapProvider()
        {
            var parameterExpression = Expression.Parameter(typeof(T), "p");

            var memberInitExpression = Expression.MemberInit(Expression.New(typeof(TF)),
                (from item in typeof(TF).GetProperties()
                    where item.CanWrite
                    let property =
                        Expression.Property(parameterExpression,
                            typeof(T).GetProperty(item.Name) ?? throw new ArgumentNullException())
                    select Expression.Bind(item, property)).Cast<MemberBinding>().ToArray());
            var lambda =
                Expression.Lambda<Func<T, TF>>(memberInitExpression, new ParameterExpression[] {parameterExpression});
            return lambda.Compile();
        }

        /// <summary>
        /// 映射方法
        /// </summary>
        /// <param name="entity">待映射的对象</param>
        /// <returns>目标类型对象</returns>
        public static TF MapTo(T entity)
        {
            return Map(entity);
        }
    }
}
