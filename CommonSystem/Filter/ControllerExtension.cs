using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
namespace CommonSystem.Filter
{
    public static class ControllerExtension
    {
        private static IWebHostEnvironment _hostEnvironment;

        /// <summary>
        /// 获取wwwroot下文件
        /// </summary>
        /// <param name="filename">路径</param>
        /// <returns>返回绝对路径</returns>
        public static string GetabsoluteFilePath(string Path)
        {
            string temp = _hostEnvironment.WebRootPath;
            temp =temp+Path.Replace("~", "");
            return temp;
        }

        /// <summary>
        /// 获取项目下文件
        /// </summary>
        /// <param name="Path"></param>
        /// <returns>返回项目文件下路径</returns>
        public static string GetTempFilePath(string Path)
        {
            string temp = _hostEnvironment.ContentRootPath;
            return temp;
        }

        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        public static void SetHostEnvironment(IWebHostEnvironment host)
        {
            _hostEnvironment = host;
        }

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
                return false;

            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }
    }
}
