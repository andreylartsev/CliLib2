using CliLib;
using System;

namespace Sample00_Simple
{
    public enum Option { First, Second };

    [Cli.Doc(@"The test program documentation goes here")]
    internal class Program
    {

        [Cli.Named('A')]
        public bool PrintArgs = false;

        [Cli.Named('O')]
        public Option Option = Option.First;

        [Cli.Named]
        public uint One = 1;

        [Cli.Named]
        public double Two = 2.0;

        [Cli.Named]
        public DateTimeOffset LastUpdateTime = DateTimeOffset.UtcNow;

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
                    
                    switch (p.Option)
                    {
                        case Option.First:
                            Console.WriteLine("Option first has been selected");
                            break;
                        case Option.Second:
                            Console.WriteLine("Option second has been selected");
                            break;
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
