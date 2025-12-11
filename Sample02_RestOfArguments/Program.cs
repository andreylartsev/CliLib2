using CliLib;
using System;
using System.IO;

namespace Sample02_RestOfArguments
{

    [Cli.Doc(@"The test program documentation goes here")]
    internal class Program : Cli.IExecutable
    {
        [Cli.Doc("Print given arguments before execution")]
        [Cli.Named('A')]
        public bool PrintArgs = false;

        [Cli.Doc("File names to check if they are exists")]
        [Cli.Positional]
        [Cli.RestOfArguments]
        [Cli.Required]
        public string[] FileName = { };

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            foreach (var fileName in this.FileName)
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

        public static void Main(string[] args)
        {
            try
            {
                var p = new Program();
                new Cli.SimpleCommandLine(p).ParseArgs(args).Exec();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    }

}