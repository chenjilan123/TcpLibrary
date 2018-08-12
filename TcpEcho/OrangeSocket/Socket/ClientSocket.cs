using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OrangeSocket
{
    public class ClientSocket
    {
        private readonly Socket _socket; //如何将这个集成到基类去？

        public EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        #region Constructor
        public ClientSocket(Socket socket)
        {
            this._socket = socket;
        }
        #endregion

        #region Interface

        #region ReadWriteAsync
        /// <summary>
        /// ReadWriteAsync
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public async Task ReadWriteAsync()
        {
            //Connect Log
            Console.WriteLine($"[{_socket.RemoteEndPoint}]: connected");

            //Process Logic
            var pipe = new Pipe();
            Task waiting = FillPipeAsync(pipe.Writer);
            Task reading = ReadPipeAsync(pipe.Reader);
            await Task.WhenAll(waiting, reading);

            //Disconnect Log
            Console.WriteLine($"[{_socket.RemoteEndPoint}]: disconnected");

        }
        #endregion

        #region Shutdown
        public void Shutdown(SocketShutdown how)
        {
            _socket.Shutdown(how);
            //Dispose?


        }
        #endregion

        #region SendAsync
        /// <summary>
        /// SendAsync
        /// Learn by FillPipeAsync
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] bytes)
        {
            var bytesSend = await _socket.SendAsync(bytes, SocketFlags.None);
            //And



        }
        #endregion

        #endregion

        #region FillPipeAsync
        /// <summary>
        /// FillPipeAsync
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private async Task FillPipeAsync(PipeWriter writer)
        {
            const int minimumBufferSize = 512;
            while (true)
            {
                try
                {
                    var memory = writer.GetMemory(minimumBufferSize);

                    int bytesRead = await _socket.ReceiveAsync(memory, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    writer.Advance(bytesRead);
                }
                catch
                {
                    break;//Disconnect?
                }
                var result = await writer.FlushAsync();
                if (result.IsCompleted)
                {
                    break;
                }
            }
            writer.Complete();
        }
        #endregion

        #region ReadPipeAsync
        /// <summary>
        /// ReadPipeAsync
        /// Override this method to process different data protocol.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual async Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();

                var buffer = result.Buffer;
                do
                {
                    //Abstract
                    SequencePosition? position = buffer.PositionOf((byte)'\n');
                    if (position == null)
                    {
                        break;
                    }
                    var line = buffer.Slice(0, position.Value);
                    ProcessData(line);

                    var next = buffer.GetPosition(1, position.Value);

                    buffer = buffer.Slice(next);
                } while (true);
                reader.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted)
                {
                    break;
                }
            }
            reader.Complete();
        }
        #endregion

        #region ProcessData
        private void ProcessData(in ReadOnlySequence<byte> buffer)
        {
            Console.Write($"[{_socket.RemoteEndPoint}]: ");

            foreach (var segment in buffer)
            {
                Console.Write(Encoding.ASCII.GetString(segment.Span));
            }
            Console.WriteLine();
        }
        #endregion
    }
}
