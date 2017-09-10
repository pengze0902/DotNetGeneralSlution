using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace AuxiliaryLibrary.Cache
{
    /// <summary>
    /// Cookies工具类
    /// </summary>
    public class Cookie
    {
        #region Set(设置Cookie)
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="exp">过期时间</param>
        public static void Set(string key, string value, DateTime? exp = null)
        {
            HttpCookie cookie = new HttpCookie(key, value)
            {
                Expires = exp ?? DateTime.MaxValue
            };
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 设置Cookie，该Cookie在浏览器关闭时，自动清除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetAuto(string key, string value)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="exp">过期时间</param>
        /// <param name="domain">Cookie域</param>
        /// <param name="path">虚拟路径</param>
        /// <param name="urlEncode">是否以UrlEncode进行转码</param>
        public static void Set(string key, string value, DateTime? exp = null, string domain = null,string path = "/", bool urlEncode = true)
        {
            if (urlEncode)
            {
                value = HttpUtility.UrlEncode(value);
            }
            HttpCookie cookie = new HttpCookie(key, value)
            {
                Path = path,
                Expires = exp ?? DateTime.MaxValue
            };

            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }
            HttpContext.Current.Response.Cookies.Set(cookie);
        }
        #endregion

        #region Get(读取Cookie)
        /// <summary>
        /// 读取Cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            var cookieValue = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null && cookie.ToString() != "")
            {
                cookieValue = cookie.Value;
            }
            return cookieValue;
        }
        #endregion

        #region Remove(删除Cookie)
        /// <summary>
        /// 删除指定Cookie，根据key删除某个Cookie的值
        /// </summary>
        /// <param name="key">键</param>
        public static void Remove(string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }
        #endregion

        #region Clear(清空Cookie)
        /// <summary>
        /// 清除Cookie，将Cookie的时间都变成过期
        /// </summary>
        public static void Clear()
        {
            int cookieCount = HttpContext.Current.Request.Cookies.Count;
            for (int i = 0; i < cookieCount; i++)
            {
                HttpCookie httpCookie = HttpContext.Current.Request.Cookies[i];
                if (httpCookie != null)
                {
                    string cookieName = httpCookie.Name;
                    HttpCookie cookie = new HttpCookie(cookieName)
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    };
                    HttpContext.Current.Response.Cookies.Set(cookie);
                }
            }
        }
        #endregion

        #region GetAll(获取全部Cookie集合)
        /// <summary>
        /// 获取全部Cookie集合
        /// </summary>
        /// <param name="cc">Cookie容器</param>
        /// <returns></returns>
        public static List<System.Net.Cookie> GetAll(CookieContainer cc)
        {
            List<System.Net.Cookie> cookies = new List<System.Net.Cookie>();
            Hashtable table =
                (Hashtable)
                cc.GetType()
                    .InvokeMember("m_domainTable",
                        BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, cc,
                        new object[] { });
            StringBuilder sb = new StringBuilder();
            foreach (object pathList in table.Values)
            {
                SortedList cookieCol =
                    (SortedList)
                    pathList.GetType()
                        .InvokeMember("m_list",
                            BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, pathList,
                            new object[] { });
                foreach (CookieCollection collection in cookieCol)
                {
                    foreach (System.Net.Cookie cookie in collection)
                    {
                        cookies.Add(cookie);
                        sb.AppendLine(cookie.Domain + ":" + cookie.Name + "____" + cookie.Value + "\r\n");
                    }
                }
            }
            return cookies;
        }
        #endregion
    }
}
