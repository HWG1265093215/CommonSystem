using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSystem.ModelHelper
{
    public static class LoggerHelper
    {
        private static Logger _logger = null;

        static LoggerHelper()
        {
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
        }

        public static void LogError(string msg,Exception ex=null)
        {
            _logger.Error(msg+(ex!=null?ex.Message:""));
        }

        public static void LogInfo(string msg)
        {
            _logger.Info(msg);
        }

        public static void LogDebug(string msg)
        {
            _logger.Debug(msg);
        }

        public static void LogTrace(string msg)
        {
            _logger.Debug(msg);
        }
    }
}
