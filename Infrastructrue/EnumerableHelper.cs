using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void ForEach<T>(this IEnumerable<T> source,Func<T> action)
        {

        }

        public static bool AnyOne<T>(this IEnumerable<T> source)
        {
            return source?.Any() ?? false;
        }
    }
}
