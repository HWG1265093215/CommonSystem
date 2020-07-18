using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructrue
{
    public static class CookieHelper
    {
        /// <summary>
        /// 清除指定Cookie
        /// </summary>
        /// <param name="cookiename">cookiename</param>
        public static void Clear(this HttpContext context, string cookiename)
        {
            var cookie = context.Request.Cookies[cookiename];
            if (cookie != null)
            {
                context.Request.Cookies.Keys.Remove(cookiename);
            }
        }

        /// <summary>
        /// 删除所有cookie值
        /// </summary>
        public static void ClearAll(this HttpContext context)
        {
                context.Request.Cookies.Keys.Clear();
        }
        /// <summary>
        /// 获取指定Cookie值
        /// </summary>
        /// <param name="cookiename">cookiename</param>
        /// <returns>Cookie值</returns>
        public static string GetCookieValue(this HttpContext context,string cookiename)
        {
            var cookie = context.Request.Cookies[cookiename];
            string str = null;
            if (cookie != null)
            {
                str = cookie;
            }

            return str;
        }

        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookiename">cookie名</param>
        /// <param name="cookievalue">cookie值</param>
        /// <param name="expires">过期时间 DateTime</param>
        public static void SetCookie(this HttpContext context,string cookiename, string cookievalue, DateTime expires)
        {
            context.Response.Cookies.Append(cookiename, cookievalue,new CookieOptions()
            {
                IsEssential=true,
                Expires=expires
            });
        }

        /// <summary>
        /// 添加一个Session
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SessionName"></param>
        /// <param name="SessionValue"></param>
        public static void setSession(this HttpContext context,string SessionName,string SessionValue)
        {
            context.Session.Set(SessionName, Encoding.UTF8.GetBytes(SessionValue));
        }

        public static void RemoveSession(this HttpContext context,string SessionName)
        {
            context.Session.Remove(SessionName);
        }

        public static void RemoveAll(this HttpContext context)
        {
            context.Session.Clear();
        }

        public static string GetSession(this HttpContext context,string SessionName)
        {
           return Encoding.UTF8.GetString(context.Session.Get(SessionName));
        }

        //public static byte[] TryGetSessionValue(this HttpContext context,string Session,out byte[] Values)
        //{
        //    return context.Session.TryGetValue(Session,Values);
        //}
    }
}
