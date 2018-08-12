using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class WebReq
    {
        public static async Task<string> Request(string url)
        {
            var uri = new Uri(url);
            var request = WebRequest.Create(uri);
            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var sr = new StreamReader(stream))
            {
                //Encoder?
                return await sr.ReadToEndAsync();
            }
        }

        /// <summary>
        /// WebClient
        /// Upload, Download, File, Data...
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> RequestX(string url)
        {
            //代码少了很多。
            var client = new WebClient();
            var data = await client.DownloadDataTaskAsync(new Uri(url));
            return ASCIIEncoding.UTF8.GetString(data);
        }
    }
}
