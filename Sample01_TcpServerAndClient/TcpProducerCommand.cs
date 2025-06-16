using System;
using System.Net.Sockets;
using System.Text;
using CliLib;

namespace Sample01_TcpServerAndClient
{
    [Cli.Doc("Generates number of messages with specific size separated by line end symbol")]
    [Cli.GenerateSample]
    internal class TcpProducerCommand : Cli.ICommand
    {
        [Cli.Named]
        public bool PrintArgs = false;

        [Cli.Doc("Connecting to specific IP address or hostname i.e. \"127.0.0.1\" / \"localhost\" ")]
        [Cli.Positional]
        [Cli.Required]
        public string Host = String.Empty;

        [Cli.Doc("Connecting to specific IP port")]
        [Cli.Positional]
        [Cli.Required]
        [Cli.AllowedRange(1001, 65535)]
        public int Port = 1001;

        [Cli.Doc("Number messages to send")]
        [Cli.Positional]
        [Cli.Required]
        [Cli.AllowedRange(1, Int32.MaxValue)]
        public int NumberOfMessages = 1000;

        [Cli.Doc("Message size")]
        [Cli.Positional]
        [Cli.SampleValue(65535)]
        [Cli.AllowedRange(10, Int32.MaxValue)]
        public int MessageSize = 65535;

        [Cli.Doc("Sending buffer size")]
        [Cli.Positional]
        [Cli.SampleValue(65535 * 16)]
        [Cli.AllowedRange(10, Int32.MaxValue)]
        public int SendingBufferSize = 65535 * 256;

        public TcpProducerCommand() { }

        public string CommandName => "tcp-producer";

        public void Exec()
        {

            if (this.PrintArgs)
                Cli.PrintArgs(this);

            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(this.Host, this.Port);
            try
            {
                Console.WriteLine($"Connected to {this.Host}:{this.Port}");

                socket.SendBufferSize = this.SendingBufferSize;
                
                var startTime = DateTime.Now;
                
                long totalBytesSent = 0;

                for (int i = 0; i < this.NumberOfMessages; i++)
                {
                    Console.WriteLine($"at {DateTime.Now.Subtract(startTime).TotalSeconds} second : building message...");
                    var message = GetMessageBytes(i, this.MessageSize);
                    Console.WriteLine($"at {DateTime.Now.Subtract(startTime).TotalSeconds} second : sending message...");
                    var sent = socket.Send(message);
                    totalBytesSent += sent;
                    var elapsed = DateTime.Now.Subtract(startTime);
                    Console.WriteLine($"at {elapsed.TotalSeconds} second : sent the message id {i} that is {sent} bytes long (including message separator \\n).");
                }
                Console.WriteLine($"All done. Total bytes sent {totalBytesSent}");
            }
            finally
            {
                socket.Close();
                Console.WriteLine("connection closed.");
            }
        }
        private static byte[] GetMessageBytes(long id, long length)
        {
            var idPrefix = id.ToString("D5");
            StringBuilder builder = new StringBuilder();
            builder.Append(idPrefix);
            for (long i = idPrefix.Length; i < length - 1; i++)
            {
                builder.Append("*");
            }
            builder.Append("\n");
            return System.Text.Encoding.ASCII.GetBytes(builder.ToString());
        }
    }
}
