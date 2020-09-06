using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructrue
{
    public static class EnumerableHelper
    {
        /// <summary>
        /// 字符串相加
        /// </summary>
        /// <param name="source"></param>
        /// <param name="splitor">分割符号</param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> source,string splitor)
        {
            if (source is null) return string.Empty;
            return string.Join(splitor,source);
        }

        public static string Join<T>(this IEnumerable<T> source,string splitor)
        {
            if (source == null) return string.Empty;
            return source.Select(x => x.ToString()).Join(splitor);
        }
        [Obsolete("待完善")]
        public static void ForEach<T>(this IEnumerable<T> source,Action<T> action)
        {
            if (source == null || action == null)
                return;

            foreach (var element in source)
            {
                action(element);
            }
        }

        public static void NewForEach<T>(this IEnumerable<T> source,Action<T> action)
        {
            if (source == null || action == null)
                return;

            foreach (var element in source)
            {
                action(element);
            }
        }

        public static bool AnyOne<T>(this IEnumerable<T> source)
        {
            return source?.Any() ?? false;
        }

        public static string GetDescriptionForEnumName(this object obj)
        {
            try
            {
                if (obj == null) return string.Empty;
                var type = obj.GetType();
                var fieldName = type.GetField(Enum.GetName(type,obj));
                if (fieldName == null) return obj.ToString();
                var des = CustomAttributeData.GetCustomAttributes(type.GetMember(fieldName.Name)[0]);
                return des.AnyOne() && des[0].ConstructorArguments.AnyOne()
                    ? des[0].ConstructorArguments[0].Value.ToString()
                    : obj.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public static string GetKeyValue<T>(this T obj)
        {
            if (obj == null)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var val = prop.GetValue(obj, null);
                var name = prop.Name;

                sb.AppendFormat("{0}={1}\r\n", name, val);
            }
            return sb.ToString();
        }
    }
}
