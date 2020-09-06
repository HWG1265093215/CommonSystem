using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApplicationLayer.ExtendHelper
{
    public static class DtoModelMapper
    {
        public static TResult ExtendMap<TResult, TObject>(this IMapper mapper, TObject obj)
        {
            //映射赋值
            TResult result = mapper.Map<TResult>(obj);
            string Column = string.Empty;
            //获取对应特性名称
            foreach (PropertyInfo property in typeof(TResult).GetProperties())
            {
                //若在属性上定义了特性，则根据特性进行赋值
                if (property.IsDefined(typeof(FieldAttribute), false))
                {
                    //获取标记属性值
                    Column = property.GetCustomAttribute<FieldAttribute>().FieldName;
                    //获取标记属性值是否有在映射源中
                    if (obj.GetType().GetProperties().First(n => n.Name == Column) != null)
                    {
                        //获取映射源中的对应属性名及属性值
                        dynamic temp = obj.GetType().GetProperty(Column).GetValue(obj, null);
                        property.SetValue(result, temp);
                    }
                }
                //当有忽略属性值的时候则进行赋初始值
                if (property.IsDefined(typeof(IgnoreAttribute), false))
                {
                    property.SetValue(result, property.PropertyType.GetDefaultTypeValue());
                }
            }

            return result;
        }

        public static object GetDefaultTypeValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 忽略null值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TObject MapperSource<TSource, TObject>(this IMapper mapper, TSource source,TObject obj) where TObject : new()
        {

            
            TSource _source = mapper.Map<TSource>(source);

            foreach (var item in _source.GetType().GetProperties())
            {
                var tempValue = source.GetType().GetProperty(item.Name).GetValue(source, null);
                if (tempValue == null)
                {
                    continue;
                }
                obj.GetType().GetProperty(item.Name).SetValue(obj, tempValue,null);
            }

            return (TObject)obj;
        }
    }
}
