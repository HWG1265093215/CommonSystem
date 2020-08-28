using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructrue
{
    public class SqlConnectionHelper
    {
        //数据库连接字符串
        private static string Connection = string.Empty;
        //设置只读属性防止修改
        private static string _Connection
        {
            get { return  Connection; }
        }
        //初始化连接字符串    之后由配置文件读取
        private SqlConnectionHelper()
        {
            Connection = "Data Source=222.92.117.211,2017; Initial Catalog=TestDataBase; Pooling=True; UID=sa;PWD=susoft;connect Timeout=120;";
        }
        //线程使用
        private static readonly object obj = new object();
        //单例模式
        private static SqlConnectionHelper helperCon = null;
        private static IDbConnection _db = null;
        //对外公开获取实例
        public static SqlConnectionHelper GetInstance()
        {
            //双重检查  防止多实例
            if(helperCon == null)
            {
                lock (obj)
                {
                    if (helperCon == null)
                    {
                        helperCon = new SqlConnectionHelper();
                    }
                }
            }
            return helperCon;
        }
        //创建数据库对象并打开连接
        public static IDbConnection OpenConnection()
        {
            if (_db == null)
            {
                _db = new SqlConnection(_Connection);
            }
            if(_db.State==ConnectionState.Closed||_db.State==ConnectionState.Broken)
            {
                _db.Open();
            }
            return _db;
        }
    }
}
