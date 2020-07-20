using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Web;
using SharpCompress.Writers;
using SharpCompress.Common;

namespace Infrastructrue
{
    public static class FileBigStreamHelper
    {
        /// <summary>
        ///以文件流形式复制文件
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <param name="dest">目标地址</param>
        /// <param name="buffsize">缓冲区大小</param>
        ///FileStream stream = new FileStream(@"C:\Users\Administrator\Desktop\1.zip", FileMode.Open,FileAccess.Read,FileShare.Read);
        ///byte[] b = new byte[stream.Length];
        ///stream.Read(b,0,b.Length);
        ///Stream stream1 = new MemoryStream(b);
        ///stream1.CopyToFile(@"C:\Users\Administrator\Desktop\新建文件夹\2.zip");
        public static void CopyToFile(this Stream fs,string dest,int buffsize=1024*1024*4)
        {
            using var fsStrams = new FileStream(dest,FileMode.CreateNew,FileAccess.ReadWrite);
            byte[] buf = new byte[buffsize];
            int len = 0;
            //读写流若为0  已读取完毕
            while ((len=fs.Read(buf,0,buf.Length))!=0)
            {
                fsStrams.Write(buf,0,len);
            }
        }

        public static async void CopyToFileAsync(this Stream fs, string dest, int buffsize = 1024 * 1024 * 4)
        {
            using var fsStrams = new FileStream(dest, FileMode.CreateNew, FileAccess.ReadWrite);
            byte[] buf = new byte[buffsize];
            int len = 0;
            //读写流若为0  已读取完毕
            await Task.Run(()=>
            {
                while ((len = fs.Read(buf, 0, buf.Length)) != 0)
                {
                    fsStrams.Write(buf, 0, len);
                }
            }).ConfigureAwait(true);
        }
        /// <summary>
        /// 将内存流转换为文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        public static void SaveFile(this MemoryStream stream,string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] buffer = stream.ToArray(); // 转化为byte格式存储
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
        }
        /// <summary>
        /// 将多个文件压缩到一个内存流中 可保存为Zip，方便下载
        /// </summary>
        /// <param name="files"></param>
        /// <param name="rootdir"></param>
        /// <returns></returns>
        public static MemoryStream ZipStream(List<string> files,string rootdir="")
        {
            using var achive = CreateZipArchive(files,rootdir);
            var ms = new MemoryStream();
            achive.SaveTo(ms, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate)
            {
                LeaveStreamOpen = true,
                ArchiveEncoding=new SharpCompress.Common.ArchiveEncoding()
                {
                    Default=Encoding.UTF8
                }
            }) ;
            return ms;
        }
        /// <summary>
        /// 创建Zip包
        /// </summary>
        /// <param name="files">多个路径</param>
        /// <param name="rootdir"></param>
        /// <returns></returns>
        private static ZipArchive CreateZipArchive(List<string> files,string rootdir)
        {
            //创建Zip
            var zip = ZipArchive.Create();
            var dic = GetFileEntryMaps(files);

            var remoteUrls = files.Distinct().Where(s=>s.StartsWith("http")).Select(s=>
            {
                try
                {
                    return new Uri(s);
                }
                catch (Exception)
                {
                    return null;
                    throw;
                }
            }).Where(u=>u!=null).ToList();
            #warning 可优化
            foreach (var item in dic)
            {
                //rootdir为 内部文件夹
                zip.AddEntry(Path.Combine(rootdir,item.Value),item.Key);
            }


            if(!remoteUrls.Any())
            {
                return zip;
            }

            var streams = new ConcurrentDictionary<string, Stream>();
            using var httpclient = new HttpClient();
            Parallel.ForEach(remoteUrls,url=>
            {
                httpclient.GetAsync(url).ContinueWith(async t=>
                {
                    //等待是否执行完成
                    if(t.IsCompleted)
                    {
                        var res = await t;
                        //若返回能正确访问
                        if(res.IsSuccessStatusCode)
                        {
                            Stream s = await res.Content.ReadAsStreamAsync();
                            streams[Path.Combine(rootdir, Path.GetFileName(HttpUtility.UrlEncode(url.AbsolutePath)))] = s;
                        }
                    }
                }).Wait();
            });
            foreach (var kv in streams)
            {
                zip.AddEntry(kv.Key,kv.Value);
            }
            return zip;
        }

        public static void Zip(List<string> files,string ZipPath,string ZipFile,string rootdir="")
        {
            using var achive = CreateZipArchive(files,rootdir);
            achive.SaveTo(ZipFile,new WriterOptions(CompressionType.Deflate){
                LeaveStreamOpen=true,
                ArchiveEncoding=new ArchiveEncoding()
                {
                    Default=Encoding.UTF8
                }
            });
        }
        /// <summary>
        /// 获取文件路径和Zip-entry映射
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetFileEntryMaps(List<string> files)
        {
            var filelist = new List<string>();
            void GetFilesRecurs(string path)
            {
                //要搜索目录的相对路径或绝对路径
                filelist.AddRange(Directory.GetFiles(path));
                //遍历路径下的所有文件夹包含子文件
                foreach (var item in Directory.GetDirectories(path))
                {
                    GetFilesRecurs(item);
                }
            }

            files.Where(f=>!f.StartsWith("http")).ToList().ForEach(s=>
            {
                //判断是否存在目录
                if(Directory.Exists(s))
                {
                    GetFilesRecurs(s);
                }
                else
                {
                    filelist.Add(s);
                }
            });
            if (!filelist.Any()) return new Dictionary<string, string>();

            //获取路径最短的那一个   进行数据截取
            var dir = new string(filelist.First().Substring(0, filelist.Min(s => s.Length)).TakeWhile((c, i) => filelist.All(s => s[i] == c)).ToArray());

            if(!Directory.Exists(dir))
            {
                //获取父路径
                dir = Directory.GetParent(dir).FullName;
            }
            //将集合转换为键值对
            var dic = filelist.ToDictionary(s=>s,s=>s.Substring(dir.Length));
            return dic;
        }
    }
}
