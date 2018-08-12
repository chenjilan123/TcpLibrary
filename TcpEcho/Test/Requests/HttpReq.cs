using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Test.Request;

namespace Test.Requests
{
    public class HttpReq
    {
        public static void QuestPost(string url, string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            HttpPost(url, data);
        }

        public static async void HttpPost(string url, byte[] data)
        {
            var request = WebRequest.Create(url);
            var httpRequest = request as HttpWebRequest;
            //①POST可以动态绑定ASP.NET MVC的实体吗？
            //②回去用ASP.NET Core的参数试下。
            //最好用API测试，MVC页面主要不负责提供接口
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            httpRequest.ContentLength = data.Length;

            using (var stream = await httpRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using (var httpResponse = await httpRequest.GetResponseAsync() as HttpWebResponse)
            using (var stream = httpResponse.GetResponseStream())
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                var s = await sr.ReadToEndAsync();

                Console.WriteLine(s);
            }
        }

        public static async void HttpRedirect(string url, bool allowRedirect)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.AllowAutoRedirect = allowRedirect;
            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var sr = new StreamReader(stream)) 
            {
                Console.WriteLine(await sr.ReadToEndAsync());
            }
        }
    }
}
