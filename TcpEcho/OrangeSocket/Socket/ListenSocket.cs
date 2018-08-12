using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OrangeSocket
{
    /// <summary>
    /// Combine of listen socket and datasocket
    /// </summary>
    public class ListenSocket : Socket
    {
        private readonly ConcurrentDictionary<string, ClientSocket> _clients = new ConcurrentDictionary<string, ClientSocket>();
        
        public ListenSocket(SocketType socketType, ProtocolType protocolType) 
            : base(socketType, protocolType)
        { }

        public async new Task Listen(int backLog)
        {
            base.Listen(backLog);

            //Strt Log
            Console.WriteLine($"Listening on {this.LocalEndPoint}");

            while (true)
            {
                var socket = await this.AcceptAsync();
                var client = new ClientSocket(socket);
                TryAddSocket(client);
                _ = client.ReadWriteAsync();
                TryRemoveSocket(client);
            }
        }

        private bool TryRemoveSocket(ClientSocket client)
        {
            return _clients.TryRemove(client.RemoteEndPoint.ToString(), out var value);
        }

        private bool TryAddSocket(ClientSocket client)
        {
            return _clients.TryAdd(client.RemoteEndPoint.ToString(), client);
        }
    }
}
