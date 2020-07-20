using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructrue
{
    public static class RequestApiHelper
    {
        //返回序列化字符串
        public static string GetRequestApi(string url,Dictionary<string,string> list,string Method="Get")
        {
            string str = "";
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (var item in list)
            {
                if(index==0)
                {
                    sb.Append("?");
                }
                else
                {
                    sb.Append("&");
                }
                sb.Append($"{item.Key}={item.Value}");
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url+sb.ToString());
            req.Method = Method;
            req.ContentType = "application/json;charset=UTF-8";
            HttpWebResponse webResponse = req.GetResponse() as HttpWebResponse;
            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                str = reader.ReadToEnd();
            }
            return str;
        }

        //返回序列化字符串
        public static List<T> GetRequestApi<T>(string url, Dictionary<string, string> list,string Method="Get")
        {
            string str = "";
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (var item in list)
            {
                if (index == 0)
                {
                    sb.Append("?");
                }
                else
                {
                    sb.Append("&");
                }
                sb.Append($"{item.Key}={item.Value}");
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + sb.ToString());
            req.Method = Method;
            req.ContentType = "application/json;charset=UTF-8";
            HttpWebResponse webResponse = req.GetResponse() as HttpWebResponse;
            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                str = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<T>>(str);
        }
    }
}
