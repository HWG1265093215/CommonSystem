using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        


    }
}
