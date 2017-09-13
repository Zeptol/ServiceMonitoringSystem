using System;
using System.Linq.Expressions;

namespace ServiceMonitoringSystem.Common.Extensions
{
    public static class LambdaExtensions
    {

        /// <summary>
        /// 动态创建Lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> ToLambda<T>(this string propertyName)
        {
            //1.创建表达式参数（指定参数或变量的类型:p）  
            ParameterExpression param = Expression.Parameter(typeof(T));
            //2.构建表达式体(类型包含指定的属性:p.Name)  
            MemberExpression body = Expression.Property(param, propertyName);
            //var body = Expression.MakeMemberAccess(param, typeof(T).GetProperty(propertyName));
            //3.根据参数和表达式体构造一个lambda表达式  
            //var funType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(object));
            return Expression.Lambda<Func<T, object>>(Expression.Convert(body, typeof(object)), param);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null) return right;
            if (right == null) return left;
            var p = Expression.Parameter(typeof(T), "x");
            var binaryExpression = Expression.AndAlso(Expression.Invoke(left, p), Expression.Invoke(right, p));
            var res = Expression.Lambda<Func<T, bool>>(binaryExpression, p);
            return res;
        }
    }
}
