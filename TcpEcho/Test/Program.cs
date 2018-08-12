using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using OrangeSocket;
using Test.References;
using Test.Request;
using Test.Requests;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AsynchronousWebRequest(args);

            Console.ReadLine();
        }

        #region AsynchronousWebRequest
        private static void AsynchronousWebRequest(string[] args)
        {
            var request = new AsynchronousWebRequest();
            request.AsynchronousRequest(args);
        }
        #endregion

        #region HttpWebRequest
        private static void HttpWebRequest()
        {
            HttpReq.QuestPost(Mime.htmlPost, "Hehehehaha!");
        }
        #endregion

        #region WebRequest
        private static async void WebRequest(int reqNum)
        {
            var results = new List<string>();
            string s = string.Empty;
            foreach (var uri in GetMimeList().Take(reqNum))
            {
                try
                {
                    s = await WebReq.Request(uri);
                    results.Add(s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            Console.WriteLine($"Count: {results.Count}");
            Console.WriteLine("Length:");
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine($"\t{i+1}: {results[i].Length}char");
            }
        }

        private static IEnumerable<string> GetMimeList()
        {
            yield return Mime.html1;
            yield return Mime.html2;
            yield return Mime.xml;
            yield return Mime.json;
            yield return Mime.mp4;
            yield return Mime.file;
            yield return Mime.binary;

        }
        #endregion

        #region WebRequestX
        private static async void WebRequestX()
        {
            var sr = await WebReq.RequestX(Mime.html1);
            Console.WriteLine(sr);
        }
        #endregion

        #region Socket
        private static async void BeginSocket()
        {
            var orange = new ListenSocket(SocketType.Stream, ProtocolType.Tcp);
            orange.Bind(new IPEndPoint(IPAddress.Loopback, 8087));

            await orange.Listen(120);
        }
        #endregion
    }
}
