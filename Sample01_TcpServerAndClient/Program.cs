using System;
using CliLib;

namespace Sample01_TcpServerAndClient
{
    [Cli.Doc(@"The test program documentation goes here")]
    internal class Program 
    {

        public static void Main(string[] args)
        {
            try
            {
                Program program = new Program();

                Cli.ICommand[] commands =
                {
                    new TcpServerCommand(),
                    new TcpProducerCommand(),
                };

                new Cli.MultiCommandLine(program, commands).ParseArgs(args).Exec();
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString());
            }
        }

    }
}