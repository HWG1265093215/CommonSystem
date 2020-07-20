using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Infrastructrue
{
    public static class DataSetToListHelper
    {
        public static List<T> DataTableToList<T>(this DataTable dataTable)
        {
            List<T> list = new List<T>();
            //初始化集合大小  并赋初始值
            var property = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow item in dataTable.Rows)
            {
                //创建实例对象
                T type = Activator.CreateInstance<T>();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = property.Find(p=>p.Name==dataTable.Columns[i].ColumnName);
                    if(property!=null)
                    {
                        try
                        {
                            //若值不为NULL
                            if(!Convert.IsDBNull(item[i]))
                            {
                                object obj = null;
                                //判断是否可以为NULL
                                if(propertyInfo.PropertyType.ToString().Contains("System.Nullable",StringComparison.OrdinalIgnoreCase))
                                {
                                    //返回指定可空的基础类型参数
                                    obj = Convert.ChangeType(item[i],Nullable.GetUnderlyingType(propertyInfo.PropertyType));
                                }
                                else
                                {
                                    obj = Convert.ChangeType(item[i],propertyInfo.PropertyType);
                                }
                                propertyInfo.SetValue(type, obj,null);
                            }
                        }
                        catch (Exception ex)
                        {

                            throw new Exception("字段[" + propertyInfo.Name + "]转换出错," + ex.Message);
                        }
                    }
                }
                list.Add(type);
            }
            return list;
        }

        public static DataTable ListToDataTable<T>(this List<T> list)
        {
            DataTable dt = new DataTable(typeof(T).FullName);
            //搜索指定包括公开成员   实例成员
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public|BindingFlags.Instance);
            foreach (PropertyInfo item in props)
            {
                dt.Columns.Add(item.Name,Nullable.GetUnderlyingType(item.PropertyType)??item.PropertyType);
            }
            foreach (T item in list)
            {
                var values = new object[props.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item,null);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }
    }
}
