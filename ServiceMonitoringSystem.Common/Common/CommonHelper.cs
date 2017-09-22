using System;
using System.IO;
using System.Net;
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
        public static string GetMd5(string str, bool isLower = false)
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
        public static string GetMd5(Stream inputStream)
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

        public static string PutModel(string url, string postData, CookieContainer cookieContainer)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ServicePoint.ConnectionLimit = 300;
                httpWebRequest.Referer = url;
                httpWebRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                httpWebRequest.Method = "Put";
                httpWebRequest.ContentLength = bytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                string end = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return end;
            }
            catch (Exception ex)
            {
                if (httpWebRequest != null)
                    httpWebRequest.Abort();
                if (httpWebResponse != null)
                    httpWebResponse.Close();
                throw;
            }
        }

    }
}
