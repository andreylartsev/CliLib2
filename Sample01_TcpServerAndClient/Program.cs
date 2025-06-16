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

                try
                {
                    var command = Cli.ParseCommandLine(args, commands);
                    command.Exec();
                }
                catch (Cli.PrintAppSettingsException)
                {
                    Cli.PrintAppSettings(program, commands);
                }
                catch (Cli.ProgramHelpException e)
                {
                    Cli.PrintCommandLine(args);
                    Cli.PrintUsage(program, commands, e.HelpType);
                }
                catch (Cli.CommandHelpException e)
                {
                    Cli.PrintCommandUsage(e.Command, e.HelpType);
                }
                catch (Cli.PrintVersionException)
                {
                    Cli.PrintVersion();
                }
                catch (Cli.UnknownCommandException e)
                {
                    Console.WriteLine(e.Message);
                    Cli.PrintCommandLine(args);
                    Cli.PrintUsage(program, commands);
                }
                catch (Cli.ArgumentParseException e)
                {
                    Console.WriteLine(e.Message);
                    Cli.PrintCommandLine(args);
                    Cli.PrintCommandUsage(e.Command as Cli.ICommand);
                }

            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString());
            }

        }

    }
}