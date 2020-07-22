using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
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
            string sql = @"SELECT A.name AS table_name,B.name AS column_name,C.value AS column_description FROM sys.tables A INNER JOIN sys.columns B ON B.object_id = A.object_id LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id WHERE A.name = 'TestTable2'";
            sqlcon.Open();
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("exec Test", sqlcon);
            adapter.Fill(dataSet);
            DataTable data = dataSet.Tables[0];
        }
    }
}
