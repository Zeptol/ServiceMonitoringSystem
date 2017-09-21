using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServiceMonitoringSystem.Common.Extensions
{
    public static class CommonExtensions
    {
        public static bool Contains(this string src, string str, StringComparison comparisonType)
        {
            return src.IndexOf(str, comparisonType) >= 0;
        }
        public static object Clone(this object old)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(memoryStream, old);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                object obj = binaryFormatter.Deserialize(memoryStream);
                memoryStream.Close();
                return obj;
            }
        }
    }
}
