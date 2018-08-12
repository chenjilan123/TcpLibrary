using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Test.References
{
    public class RequestState
    {
        public const int BufferSize = 1024;
        public byte[] Buffer;
        public StringBuilder ResponseData;
        public Decoder Decoder;
        public Stream ResponseStream;
        public WebRequest Request;

        public RequestState()
        {
            Buffer = new byte[BufferSize];
            ResponseData = new StringBuilder(string.Empty);
            Decoder = Encoding.UTF8.GetDecoder();
        }
    }

    public class AsynchronousWebRequest
    {
        private const int BufferSize = 1024;
        private ManualResetEvent allDone = new ManualResetEvent(false);
        public void AsynchronousRequest(string[] args)
        {
            if (args.Length < 1)
            {
                showusage();
                return;
            }

            var uri = new Uri(args[0]);
            var request = WebRequest.Create(uri);

            var state = new RequestState();
            state.Request = request;

            request.BeginGetResponse(new AsyncCallback(ResponseCallback), state);
            allDone.WaitOne();

            Console.WriteLine($"Response Data Length: {state.ResponseData.Length}bytes");
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as RequestState;

            var response = state.Request.EndGetResponse(ar);
            state.ResponseStream = response.GetResponseStream();

            state.ResponseStream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as RequestState;

            var iRead = state.ResponseStream.EndRead(ar);
            if (iRead > 0)
            {
                var charBuffer = new char[BufferSize];
                var len = state.Decoder.GetChars(state.Buffer, 0, iRead, charBuffer, 0);

                var str = new string(charBuffer, 0, len);

                state.ResponseData.Append(Encoding.ASCII.GetString(state.Buffer, 0, iRead));

                state.ResponseStream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                state.ResponseStream.Close();
                allDone.Set();
            }
        }
        private void showusage()
        {
            Console.WriteLine("Attempts to GET a URL");
            Console.WriteLine("\r\nUsage:");
            Console.WriteLine("   ClientGetAsync URL");
            Console.WriteLine("   Example:");
            Console.WriteLine("      ClientGetAsync http://www.contoso.com/");
        }
    }

}
