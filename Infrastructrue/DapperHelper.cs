using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Infrastructrue
{
    public static class DapperHelper
    {
        private static IDbConnection DbConnection
        {
            get
            {
                SqlConnectionHelper.GetInstance();
                return SqlConnectionHelper.OpenConnection();
            }
        }

        //获取单个对象带默认值  强类型
        public static T QueryFirstOrDefault<T>(string sql,object obj) where T:class
        {
           return DbConnection.QueryFirstOrDefault<T>(sql,obj);
        }
        //获取单个对象  返回值为动态类型
        public static dynamic QueryFirstDynamic(string sql,object obj)
        {
            return DbConnection.QueryFirstOrDefault(sql, obj);
        }
        //获取集合
        public static IEnumerable<dynamic> QueryMany<T>(string sql,object obj,int Timeout=60) where T:class
        {
            return DbConnection.Query(sql,obj,commandTimeout:Timeout);
        }
        //插入单个实体类   存储过程不带返回值
        public static int ExcuteEntity<T>(string sql,object obj, CommandType command=CommandType.Text) where T:class
        {
            int result = 0;
            IDbTransaction transaction = null;
            result = DbConnection.Execute(sql, obj, transaction,commandType:command);
            return result;
        }
        //插入多个实体   开启事务
        public static int ExcuteManyEntity<T>(string sql,List<object> obj,int timeout=60) where T:class
        {
            IDbTransaction transaction = null;
            int result = 0;
            try
            {
                transaction = DbConnection.BeginTransaction();
                result = DbConnection.Execute(sql, obj, transaction, timeout);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return result;
        }
        //封装存储过程  带输出参数
        public static string ExcuteProduce(string ProduceName, DynamicParameters dynamic,string OutPutName)
        {
            DbConnection.Execute(ProduceName, dynamic,commandType:CommandType.StoredProcedure);

            return dynamic.Get<string>("@" + OutPutName);
        }
        //执行储存过程  返回值为集合
        public static List<T> ExcuteProduce<T>(string ProduceName, DynamicParameters dynamic)
        {
           return DbConnection.Query<T>(ProduceName, dynamic, commandType: CommandType.StoredProcedure).ToList();
        }

        public static dynamic ExcuteProduce(string ProduceName,DynamicParameters dynamic)
        {
            var temp=DbConnection.Query(ProduceName, dynamic, commandType: CommandType.StoredProcedure).ToList();
            return temp;
        }
    }
}
