using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructrue
{
    public static  class GeneralSqlHelper<T> where T:class 
    {
        private static string _Insert = string.Empty;
        private static string _Update = string.Empty;
        private static string _Delete = string.Empty;
        private static string _Select = string.Empty;

        static GeneralSqlHelper()
        {
            Type type = typeof(T);
            string ColumnName = string.Join(",", type.GetProperties()
                   .Where(n => !n.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) && !n.Name.Equals("INDX", StringComparison.OrdinalIgnoreCase)).
                   Select(c => $"[{c.Name}]"));
            string ValuesName = string.Join(",", type.GetProperties()
                   .Where(n => !n.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) && !n.Name.Equals("INDX", StringComparison.OrdinalIgnoreCase)).
                   Select(c => $"@{c.Name}"));
            string UpdateColumn= string.Join(",", type.GetProperties()
                   .Where(n => !n.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) && !n.Name.Equals("INDX", StringComparison.OrdinalIgnoreCase)).
                   Select(c => $"[{c.Name}]=@{c.Name}"));
            {
                _Select = $"select {string.Join(",",type.GetProperties().Select(c=>$"[{c.Name}]"))} from {type.Name}";
            }
            {
                _Update = $"update {type.Name} set {UpdateColumn} where Id=@Id";
            }
            {
                _Delete = $"delete from {type} where Id=@Id";
            }
            {
                _Insert = $"insert into ({ColumnName}) values({ValuesName})";
            }
        }
        public static string GeneralSQL(SQLEnum sqlenum)
        {
            switch (sqlenum)
            {
                case SQLEnum.Insert:
                    return _Insert;
                    break;
                case SQLEnum.Update:
                    return _Update;
                    break;
                case SQLEnum.Delete:
                    return _Delete;
                    break;
                case SQLEnum.Select:
                    return _Select;
                    break;
                default:
                    throw new Exception("NotDefind SqlType!");
                    break;
            }
        }
    }
}
