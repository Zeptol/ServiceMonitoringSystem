using System;

namespace ServiceMonitoringSystem.Common.Extensions
{
    public static class CommonExtensions
    {
        public static bool Contains(this string src, string str, StringComparison comparisonType)
        {
            return src.IndexOf(str, comparisonType) >= 0;
        }

    }
}
