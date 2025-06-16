using CliLib;
using System;
using System.IO;

namespace Sample02_RestOfArguments
{

    [Cli.Doc(@"The test program documentation goes here")]
    internal class Program
    {
        [Cli.Doc("Print given arguments before execution")]
        [Cli.Named('A')]
        public bool PrintArgs = false;

        [Cli.Doc("File names to check if they are exists")]
        [Cli.Positional]
        [Cli.RestOfArguments]
        [Cli.Required]
        public string[] FileName = { };

        public static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                try
                {
                    Cli.ParseCommandLine(args, p);

                    if (p.PrintArgs)
                        Cli.PrintArgs(p);

                    foreach (var fileName in p.FileName)
                    {
                        FileInfo fileInfo = new FileInfo(fileName);
                        if (fileInfo.Exists)
                        {
                            Console.WriteLine($"{fileName} exists");
                        }
                        else
                        {
                            Console.WriteLine($"{fileName} does not exists");
                        }
                    }

                }
                catch (Cli.PrintAppSettingsException)
                {
                    Cli.PrintAppSettings(p);
                }
                catch (Cli.PrintVersionException)
                {
                    Cli.PrintVersion();
                }
                catch (Cli.ProgramHelpException e)
                {
                    Cli.PrintCommandLine(args);
                    Cli.PrintUsage(p, e.HelpType);
                }
                catch (Cli.ArgumentParseException e)
                {
                    Console.WriteLine(e.Message);
                    Cli.PrintCommandLine(args);
                    Cli.PrintUsage(p, Cli.HelpType.Full);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    }

}