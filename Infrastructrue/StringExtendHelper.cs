using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructrue
{
    public static class StringExtendHelper
    {
        /// <summary>  
        /// 获取单个中文的首字母  
        /// </summary>  
        /// <param name="cnChar"></param>  
        /// <returns></returns>  
        private static string GetSpell(string cnChar)
        {
            var arrCn = Encoding.Default.GetBytes(cnChar);
            if (arrCn.Length > 1)
            {
                var area = arrCn[0];
                var pos = arrCn[1];
                var code = (area << 8) + pos;
                var areacode = new[] { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };

                for (var i = 0; i < 26; i++)
                {
                    var max = 55290;
                    if (i != 25)
                    {
                        max = areacode[i + 1];
                    }
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new[] { (byte)(97 + i) });
                    }
                }
                return "*";
            }
            return cnChar;
        }
        /// <summary>
        /// 获取中文字符串首字母
        /// </summary>
        /// <param name="source"></param>
        ///  <param name="toUpper">是否大写</param>
        /// <returns></returns>
        public static string GetChineseSpell(this string source, bool toUpper = true)
        {
            var len = source.Length;
            var myStr = new StringBuilder();
            for (var i = 0; i < len; i++)
            {
                myStr.Append(GetSpell(source.Substring(i, 1)));
            }
            return toUpper ? myStr.ToString().ToUpper() : myStr.ToString();
        }
        /// <summary>
        /// 用于判断是否为空字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBlank(this string s)
        {
            return s == null || (s.Trim().Length == 0);
        }

        /// <summary>
        /// 用于判断是否为空字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotBlank(this string s)
        {
            return !s.IsBlank();
        }

        /// <summary>
        /// 将字符串转换成MD5加密字符串
        /// </summary>
        /// <param name="orgStr"></param>
        /// <returns></returns>
        public static string ToMd5(this string orgStr)
        {
            using var md5 = MD5.Create();
            var encoding = Encoding.UTF8;
            var encryptedBytes = md5.ComputeHash(encoding.GetBytes(orgStr));
            var sb = new StringBuilder(32);
            foreach (var bt in encryptedBytes)
            {
                sb.Append(bt.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetExt(this string s)
        {
            var ret = string.Empty;
            if (!s.Contains('.')) return ret;
            var temp = s.Split('.');
            ret = temp[temp.Length - 1];

            return ret;
        }

        /// <summary>
        /// 验证QQ格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsQq(this string s)
        {
            return s.IsBlank() || Regex.IsMatch(s, @"^[1-9]\d{4,15}$");
        }

        /// <summary>
        /// 判断是否为有效的Email地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
        {
            if (!s.IsBlank())
            {
                const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
                return Regex.IsMatch(s, pattern);
            }
            return false;
        }

        /// <summary>
        /// 验证是否是合法的手机号码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
        {
            if (!s.IsBlank())
            {
                return Regex.IsMatch(s, @"^\+?\d{0,4}?[1][3-8]\d{9}$");
            }
            return false;
        }
        /// <summary>
        /// 检查字符串是否为有效的int数字
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool Isint(this string val)
        {
            if (IsBlank(val))
                return false;
            int k;
            return int.TryParse(val, out k);
        }

        /// <summary>
        /// 字符串转数字，未转换成功返回0
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ToInt(this string val)
        {
            if (IsBlank(val))
                return 0;
            int k;
            return int.TryParse(val, out k) ? k : 0;
        }

        /// <summary>
        /// 检查字符串是否为有效的Decimal
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string val)
        {
            if (IsBlank(val))
                return false;
            decimal d;
            return decimal.TryParse(val, out d);
        }

        /// <summary>
        /// 检查字符串是否为有效的Decimal
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string val)
        {
            if (IsBlank(val))
                return 0.00m;
            decimal d;
            return decimal.TryParse(val, out d)?d:0.00m;
        }
    }
}
