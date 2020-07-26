﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonSystem.Filter
{
    public static class MyHttpContext 
    {
        public static IServiceProvider _serviceProvider;
        public static HttpContext Current
        {
            get
            {
                object factory = _serviceProvider.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        /// <summary>
        /// 生成token令牌
        /// </summary>
        /// <param name="tokenName"></param>
        /// <returns>成功返回随机数</returns>
        public static string setToken(string tokenName)
        {
            string retStr = "";
            //生成 Token
            string Token = new Random().NextDouble().ToString();
            Current.Session.Set(tokenName,Encoding.UTF8.GetBytes(Token));
            //Current.Session["1111"] = "1";
            retStr = Token;
            return retStr;
        }
    }
}
