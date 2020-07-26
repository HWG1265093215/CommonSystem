using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Infrastructrue
{
    public static class OtherHelper
    {
        /// <summary>  
        /// 运行一个控制台程序并返回其输出参数。  
        /// </summary>  
        /// <param name="filename">程序名</param>  
        /// <param name="arguments">输入参数</param>
        /// <param name="recordLog">是否在控制台输出日志</param>
        /// <returns></returns>  
        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }

                using var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = filename,
                        CreateNoWindow = true,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                proc.Start();

                using var sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default);
                //上面标记的是原文，下面是我自己调试错误后自行修改的  
                Thread.Sleep(100); //貌似调用系统的nslookup还未返回数据或者数据未编码完成，程序就已经跳过直接执行  
                if (!proc.HasExited) //在无参数调用nslookup后，可以继续输入命令继续操作，如果进程未停止就直接执行  
                {
                    proc.Kill();
                }

                string txt = sr.ReadToEnd();
                if (recordLog)
                {
                    Trace.WriteLine(txt);
                }

                return txt;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// 执行一个控制台程序，并获取在控制台返回的数据
        /// </summary>
        /// <param name="dosCommand">dos/cmd命令</param>
        /// <param name="outtime">等待执行时间毫秒值，默认不等待</param>
        /// <returns>控制台输出信息</returns>
        /// <exception cref="SystemException">尚未设置进程 <see cref="P:System.Diagnostics.Process.Id" />，而且不存在可从其确定 <see cref="P:System.Diagnostics.Process.Id" /> 属性的 <see cref="P:System.Diagnostics.Process.Handle" />。- 或 -没有与此 <see cref="T:System.Diagnostics.Process" /> 对象关联的进程。- 或 -您正尝试为远程计算机上运行的进程调用 <see cref="M:System.Diagnostics.Process.WaitForExit(System.Int32)" />。此方法仅对本地计算机上运行的进程可用。</exception>
        /// <exception cref="Win32Exception">未能访问该等待设置。</exception>
        /// <exception cref="Exception">命令参数无效，必须传入一个控制台能被cmd.exe可执行程序; 如：ping 127.0.0.1</exception>
        public static string Execute(string dosCommand, int outtime = 0)
        {
            string output = "";
            if (!string.IsNullOrEmpty(dosCommand))
            {
                using var process = new Process();
                var startinfo = new ProcessStartInfo(); //创建进程时使用的一组值，如下面的属性  
                startinfo.FileName = "cmd.exe"; //设定需要执行的命令程序  
                //以下是隐藏cmd窗口的方法  
                startinfo.Arguments = "/c" + dosCommand; //设定参数，要输入到命令程序的字符，其中"/c"表示执行完命令后马上退出  
                startinfo.UseShellExecute = false; //不使用系统外壳程序启动  
                startinfo.RedirectStandardInput = false; //不重定向输入  
                startinfo.RedirectStandardOutput = true; //重定向输出，而不是默认的显示在dos控制台上  
                startinfo.CreateNoWindow = true; //不创建窗口  
                process.StartInfo = startinfo;
                if (process.Start()) //开始进程  
                {
                    if (outtime == 0)
                    {
                        process.WaitForExit();
                    }
                    else
                    {
                        process.WaitForExit(outtime);
                    }

                    output = process.StandardOutput.ReadToEnd(); //读取进程的输出  
                }

                return output;
            }

            throw new Exception("命令参数无效，必须传入一个控制台能被cmd.exe可执行程序;\n如：ping 127.0.0.1");
        }


        /// <summary>
        /// 创建xml文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="UrlPath"></param>
        /// <param name="list"></param>
         public static void CreateListXml<T>(this XmlDocument document,string UrlPath,List<T> list)
         {
            XmlDocument xml = new XmlDocument();
            XmlElement element = xml.CreateElement("Root");
            foreach (var item in list)
            {
                XmlNode node = xml.CreateElement(typeof(T).Name);
                element.AppendChild(node);
                foreach (var property in typeof(T).GetProperties())
                {
                    XmlElement node1 = xml.CreateElement(property.Name);
                    node1.InnerText = typeof(T).GetProperty(property.Name).GetValue(typeof(T), null).ToString();
                    node.AppendChild(node1);
                }
            }
            xml.Save(UrlPath);
        }

        public static void AppendList<T>(this XmlDocument xml, string url,List<T> list)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(url);
            XmlElement element = doc.DocumentElement;
            foreach (var item in list)
            {
                XmlNode node = doc.CreateElement(typeof(T).Name);
                element.AppendChild(node);
                foreach (var property in typeof(T).GetProperties())
                {
                    XmlElement node1 = doc.CreateElement(property.Name);
                    node1.InnerText = typeof(T).GetProperty(property.Name).GetValue(typeof(T), null).ToString();
                    node.AppendChild(node1);
                }
            }
            doc.Save(url);
        }

        public static List<T> SerialierList<T>(this XmlDocument xml,string url)
        {
            XmlDocument document = new XmlDocument();
            document.Load(url);
            XmlNodeList nodeList = xml.DocumentElement.ChildNodes;
            List<T> list = new List<T>();
            foreach (XmlNode item in nodeList)
            {
                if(item.Name==item.Name)
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(item.OuterXml)))
                    {
                        using (XmlReader reader = XmlReader.Create(ms))
                        {
                            XmlSerializer formatter = new XmlSerializer(typeof(T));
                            T stu = (T)formatter.Deserialize(reader);
                            list.Add(stu);
                        }
                    }
                }
            }
            return list;
        }
        
        /// <summary>
        /// Ftp文件上传
        /// </summary>
        /// <param name="Url">路径</param>
        /// <param name="BaseFileName">本地文件名字</param>
        /// <param name="RomoteFileName">上传到Ftp的文件名字</param>
        /// <returns></returns>
        public static bool FtpUpLoadFile(string Url,string BaseFileName,string RomoteFileName)
        {
            bool b = false;
            FtpWebRequest ftpWeb;
            //判断文件是否存在
            if(File.Exists(BaseFileName))
            {
                ftpWeb = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://"+Url+"/"+ RomoteFileName));
                //要发到ftp的命令
                ftpWeb.Method = WebRequestMethods.Ftp.UploadFile;
                //指定文件传输类型
                ftpWeb.UseBinary = true;
                //验证用户和密码
                #warning  账号密码待补全
                ftpWeb.Credentials = new NetworkCredential("","");
                //获取响应的流
                using (Stream rs=ftpWeb.GetRequestStream())
                {
                    //上传文件流
                    using (FileStream file=new FileStream(BaseFileName,FileMode.OpenOrCreate,FileAccess.ReadWrite))
                    {
                        //4k字节
                        byte[] buffer = new byte[4096];
                        int count = file.Read(buffer,0,buffer.Length);
                        //判断文件流是
                        while (count>0)
                        {
                            //写入到Ftp文件流而不是本地
                            rs.Write(buffer,0,count);
                            count = file.Read(buffer,0,buffer.Length);
                        }
                        file.Close();
                        b = true;
                    }
                }
                return b;
            }
            throw new Exception("UpLoadFile not Find!");
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool FtpDownLoadFile(string Url,string FileName)
        {
            FtpWebRequest ftpWeb;
            try
            {
                using (FileStream fs=new FileStream(FileName,FileMode.OpenOrCreate))
                {
                    ftpWeb = (FtpWebRequest)WebRequest.Create(new Uri("ftp://"+Url+"/"+FileName));
                    ftpWeb.Method = WebRequestMethods.Ftp.DownloadFile;
                    ftpWeb.UseBinary = true;
                    ftpWeb.ContentOffset = fs.Length;
                    ftpWeb.Credentials = new NetworkCredential("","");
                    using (FtpWebResponse webResponse= (FtpWebResponse)ftpWeb.GetResponse())
                    {
                        fs.Position = fs.Length;
                        byte[] buffer = new byte[4096];
                        int count = webResponse.GetResponseStream().Read(buffer,0,buffer.Length);
                        while(count>0)
                        {
                            fs.Write(buffer,0,count);
                            count = webResponse.GetResponseStream().Read(buffer,0,buffer.Length);
                        }
                    }
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message) ;
            }
        }

        public static bool FtpDeleteFile(string Url)
        {
            FtpWebRequest ftpWeb;
            ftpWeb = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://"+Url));
            ftpWeb.Method = WebRequestMethods.Ftp.DeleteFile;
            ftpWeb.UseBinary = true;
            ftpWeb.Credentials = new NetworkCredential();
            FtpWebResponse response = (FtpWebResponse)ftpWeb.GetResponse();
            response.Close();
            return true;
        }
        public static bool FtpCreateDir(string Url)
        {
            FtpWebRequest ftpWeb;
            ftpWeb = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + Url));
            ftpWeb.Method = WebRequestMethods.Ftp.MakeDirectory;
            ftpWeb.UseBinary = true;
            ftpWeb.Credentials = new NetworkCredential("Ftp_Server", "123456");
            FtpWebResponse web = (FtpWebResponse)ftpWeb.GetResponse();

            web.Close();
            return true;
        }
        public static bool FtpRenameFile(string Url, string FileName)
        {
            FtpWebRequest ftpWeb;
            try
            {
                ftpWeb = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + Url + "/" + FileName));
                ftpWeb.Method = WebRequestMethods.Ftp.Rename;
                ftpWeb.UseBinary = true;
                ftpWeb.RenameTo = FileName;
                FtpWebResponse response = (FtpWebResponse)ftpWeb.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("File Delete Faile!" + ex.Message);
            }
        }

        public static bool SendMail(MailModel model)
        {
            try
            {
                MailAddress receiver = new MailAddress(model.ReceiverAddress, model.ReceiverName);
                MailAddress sender = new MailAddress(model.SenderAddress, model.SenderName);
                MailAddress mail = new MailAddress("1847017679@qq.com", "GDX");
                MailMessage message = new MailMessage();
                message.From = sender;//发件人
                message.To.Add(receiver);//收件人
                message.CC.Add(mail);//抄送人
                message.Subject = model.Title;//标题
                message.Body = model.Content;//内容
                message.IsBodyHtml = true;//是否支持内容为HTML

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.qq.com";
                //client.Port = 465;
                client.EnableSsl = true;//是否启用SSL
                client.Timeout = 10000;//超时
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(model.SenderAddress, model.SenderPassword);
                client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 邮件结构体
        /// </summary>
        public struct MailModel
        {
            /// <summary>
            /// 收件人地址
            /// </summary>
            public string ReceiverAddress { get; set; }
            /// <summary>
            /// 收件人姓名
            /// </summary>
            public string ReceiverName { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 发件人地址（非必填）
            /// </summary>
            public string SenderAddress { get; set; }
            /// <summary>
            /// 发件人姓名（非必填）
            /// </summary>
            public string SenderName { get; set; }
            /// <summary>
            /// 发件人密码（非必填）
            /// </summary>
            public string SenderPassword { get; set; }
        }

        public static void TestProc()
        {

            SqlConnection sqlcon = new SqlConnection("Data Source=.;Initial Catalog=Test;Integrated Security=True");
            #warning 读取表注释
            string sql = @"SELECT A.name AS table_name,B.name AS column_name,C.value AS column_description FROM sys.tables A INNER JOIN sys.columns B ON B.object_id = A.object_id LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id WHERE A.name = 'TestTable2'";
            sqlcon.Open();
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("exec Test", sqlcon);
            adapter.Fill(dataSet);
            DataTable data = dataSet.Tables[0];
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
                return replacestr;

            if (datetimestr.Equals(""))
                return replacestr;

            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;
        }

        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }


        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// 为脚本替换特殊字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrToScript(string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");

            if (!overwrite && System.IO.File.Exists(destFileName))
                return false;

            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }


        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");

                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }

        public readonly static VersionInfo AssemblyFileVersion = new VersionInfo();

        private static string TemplateCookieName = string.Format("kztemplateid_{0}_{1}_{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart);
        /// <summary>
        /// 获取记录模板id的cookie名称
        /// </summary>
        /// <returns></returns>
        public static string GetTemplateCookieName()
        {
            return TemplateCookieName;
        }

        public class VersionInfo
        {
            public int FileMajorPart
            {
                get { return 1; }
            }
            public int FileMinorPart
            {
                get { return 0; }
            }
            public int FileBuildPart
            {
                get { return 0; }
            }
            public string ProductName
            {
                get { return "KZ"; }
            }
            public string LegalCopyright
            {
                get { return "2011, SZKZ.COM"; }
            }
        }

        /// <summary>
        /// 将数据表转换成JSON类型串
        /// </summary>
        /// <param name="dt">要转换的数据表</param>
        /// <returns></returns>
        public static StringBuilder DataTableToJSON(System.Data.DataTable dt)
        {
            return DataTableToJson(dt, true);
        }

        /// <summary>
        /// 将数据表转换成JSON类型串
        /// </summary>
        /// <param name="dt">要转换的数据表</param>
        /// <param name="dispose">数据表转换结束后是否dispose掉</param>
        /// <returns></returns>
        public static StringBuilder DataTableToJson(System.Data.DataTable dt, bool dt_dispose)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[\r\n");

            //数据表字段名和类型数组
            string[] dt_field = new string[dt.Columns.Count];
            int i = 0;
            string formatStr = "{{";
            string fieldtype = "";
            foreach (System.Data.DataColumn dc in dt.Columns)
            {
                dt_field[i] = dc.Caption.ToLower().Trim();
                formatStr += "'" + dc.Caption.ToLower().Trim() + "':";
                fieldtype = dc.DataType.ToString().Trim().ToLower();
                if (fieldtype.IndexOf("int") > 0 || fieldtype.IndexOf("deci") > 0 ||
                    fieldtype.IndexOf("floa") > 0 || fieldtype.IndexOf("doub") > 0 ||
                    fieldtype.IndexOf("bool") > 0)
                {
                    formatStr += "{" + i + "}";
                }
                else
                {
                    formatStr += "'{" + i + "}'";
                }
                formatStr += ",";
                i++;
            }

            if (formatStr.EndsWith(","))
                formatStr = formatStr.Substring(0, formatStr.Length - 1);//去掉尾部","号

            formatStr += "}},";

            i = 0;
            object[] objectArray = new object[dt_field.Length];
            foreach (System.Data.DataRow dr in dt.Rows)
            {

                foreach (string fieldname in dt_field)
                {   //对 \ , ' 符号进行转换 
                    objectArray[i] = dr[dt_field[i]].ToString().Trim().Replace("\\", "\\\\").Replace("'", "\\'");
                    switch (objectArray[i].ToString())
                    {
                        case "True":
                            {
                                objectArray[i] = "true"; break;
                            }
                        case "False":
                            {
                                objectArray[i] = "false"; break;
                            }
                        default: break;
                    }
                    i++;
                }
                i = 0;
                stringBuilder.Append(string.Format(formatStr, objectArray));
            }
            if (stringBuilder.ToString().EndsWith(","))
                stringBuilder.Remove(stringBuilder.Length - 1, 1);//去掉尾部","号

            if (dt_dispose)
                dt.Dispose();

            return stringBuilder.Append("\r\n];");
        }

        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;

            return false;
        }

        /// <summary>
        /// 根据Url获得源文件内容
        /// </summary>
        /// <param name="url">合法的Url地址</param>
        /// <returns></returns>
        public static string GetSourceTextByUrl(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 20000;//20秒超时
                WebResponse response = request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                return sr.ReadToEnd();
            }
            catch { return ""; }
        }
    }
}
