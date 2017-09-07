using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ServiceMonitoringSystem.Common.Common
{
    public class CommonHelper
    {
        /// <summary>
        /// 获取md5
        /// </summary>
        /// <param name="str">需转的字符串</param>
        /// <param name="isLower">是否32位小写  默认32位大写</param>
        /// <returns></returns>
        public string GetMd5(string str, bool isLower = false)
        {
            try
            {
                var md5 = new MD5CryptoServiceProvider();
                var reStr = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");
                reStr = isLower ? reStr.ToLower() : reStr;
                return reStr;
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5() failed,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取md5
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <returns></returns>
        public string GetMd5(Stream inputStream)
        {
            try
            {
                var md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(inputStream);
                StringBuilder sb = new StringBuilder();
                foreach (byte t in retVal)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5() failed,error:" + ex.Message);
            }
        }
    }
}
