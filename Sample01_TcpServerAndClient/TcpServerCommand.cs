using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using CliLib;

namespace Sample01_TcpServerAndClient
{
    [Cli.Doc(@"Accepts TCP connections and reads messages separated by line end symbol")]
    [Cli.GenerateSample(@"binds to localhost on the TCP port")]
    internal class TcpServerCommand : Cli.ICommand
    {
        [Cli.Named]
        public bool PrintArgs = false;

        [Cli.Named]
        [Cli.AllowedRange(0)]
        public uint WaitingMilliseconds = 100;

        [Cli.Named]
        [Cli.AllowedRange(0, Int32.MaxValue)]
        public int ReceiveTimeout = 0;

        [Cli.Named]
        [Cli.AllowedRange(10, Int32.MaxValue)]
        public int MaxMessageSize = 65535 * 256;

        [Cli.Named]
        [Cli.AllowedRange(0, Int32.MaxValue)]
        public int ReceiveBufferSize = 65535 * 256;

        [Cli.Named]
        [Cli.AllowedRange(5, 65535)]
        public int DisplayMessageSize = 10;

        [Cli.Doc("binding to specific IP address i.e. \"0.0.0.0\" / \"127.0.0.1\"  ")]
        [Cli.Named('B')]
        [Cli.SampleValue("localhost")]
        [Cli.Required]
        public IPHostEntry BindTo;

        [Cli.Doc("bind to specific IP port i.e. 1000. Note that binding to ports with number equals or less than 1000 required root permissions")]
        [Cli.Positional]
        [Cli.Required]
        [Cli.SampleValue("5555")]
        [Cli.AllowedRange(1001, 65535)]
        public int Port = 5000;

        public string CommandName => "tcp-server";

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            var separatorByte = System.Text.Encoding.ASCII.GetBytes("\n");

            Console.WriteLine($"Binding to: {this.BindTo}:{this.Port} ...");

            IPEndPoint bindToEndPoint = new IPEndPoint(this.BindTo.AddressList[0], this.Port);

            Socket server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(bindToEndPoint);
            try
            {
                Console.WriteLine("Waiting for the clients...");
                server.Listen(100);
                while (true)
                {
                    try
                    {
                        var clientConnection = server.Accept();
                        try
                        {
                            var startTime = DateTime.Now;
                            Console.WriteLine("Accepted connection.");
                            clientConnection.ReceiveTimeout = this.ReceiveTimeout;
                            clientConnection.ReceiveBufferSize = this.ReceiveBufferSize;
                            long totalBytesReceived = 0;
                            var messageBuffer = new byte[this.MaxMessageSize];
                            var singleByte = new byte[1];
                            while (true)
                            {
                                var messageLength = 0;
                                var received = 0;
                                while (true)
                                {
                                    received = clientConnection.Receive(singleByte);
                                    if (received == 0)
                                        break; // end of stream
                                    totalBytesReceived += received;
                                    if (received > 0)
                                    {
                                        if (singleByte[0] == separatorByte[0])
                                            break;

                                        messageBuffer[messageLength] = singleByte[0];
                                        messageLength += received;

                                        if (messageLength >= (messageBuffer.Length - 1))
                                            break;
                                    }
                                }
                                if (received == 0)
                                    break; // end of stream
                                var elapsed = DateTime.Now.Subtract(startTime);
                                var message = System.Text.Encoding.ASCII.GetString(messageBuffer, 0, messageLength);
                                var shortMessage = $"{message.Substring(0, Math.Min(this.DisplayMessageSize, message.Length))}";
                                if (message.Length > this.DisplayMessageSize)
                                {
                                    shortMessage += $" and {message.Length - shortMessage.Length} more characters";
                                }
                                Console.WriteLine($"at {elapsed.TotalSeconds} second : received the message \"{shortMessage}\"");
                                if (this.WaitingMilliseconds > 0)
                                {
                                    Thread.Sleep((int)this.WaitingMilliseconds);
                                }
                            }
                            Console.WriteLine($"The stream has been finished by the other side. The total bytes received {totalBytesReceived}");
                        }
                        finally
                        {
                            clientConnection.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            finally
            {
                server.Close();
            }
        }
    }
}
