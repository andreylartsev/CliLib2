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
        
        [Cli.Doc("Password")]
        [Cli.Interactive("Please enter passwords", false)]
        [Cli.Named]
        [Cli.Secret]
        [Cli.Required]
        public string[] Passwords = { };

        [Cli.Doc("File names to check if they are exists")]
        [Cli.Positional]
        [Cli.Interactive("Please enter filenames", false)]
        [Cli.RestOfArguments]
        [Cli.Required]
        public FileInfo[] FileName = { };

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            foreach (var fileInfo in this.FileName)
            {
                if (fileInfo.Exists)
                {
                    Console.WriteLine($"{fileInfo.Name} exists");
                }
                else
                {
                    Console.WriteLine($"{fileInfo.Name} does not exists");
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