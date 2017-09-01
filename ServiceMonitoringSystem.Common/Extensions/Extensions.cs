using System;
using System.Linq.Expressions;

namespace ServiceMonitoringSystem.Common.Extensions
{
    public static class Extensions
    {
        public static bool Contains(this string src, string str, StringComparison comparisonType)
        {
            return src.IndexOf(str, comparisonType) >= 0;
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
