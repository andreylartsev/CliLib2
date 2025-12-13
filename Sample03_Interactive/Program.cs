using CliLib;
using System;
using System.IO;

namespace Sample03_Interactive
{

    [Cli.Doc(@"The test program to demo using interactive input and environment variables")]
    internal class Program : Cli.IExecutable
    {
        [Cli.Doc("Print given arguments before execution")]
        [Cli.Named('A')]
        public bool PrintArgs = false;
        
        [Cli.Doc("It is a master key")]
        [Cli.Interactive("Please enter master key", false)]
        [Cli.EnvironmentVariable("MASTER_KEY")]
        [Cli.Secret]
        [Cli.Required]
        public string MasterKey = string.Empty;

        [Cli.Doc("File names to check if they are exists")]
        [Cli.AppSettings]
        [Cli.Secret]
        public string ConnectionPassword = "123";

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            Console.WriteLine("Now you would see results of decryption of the password stored within app.config.");
            Cli.AskIfUserWantedContinue("Would you like to continue?", "Yes", "No");
            Console.WriteLine($"masterKey={this.MasterKey}, connectionPassword={this.ConnectionPassword}");
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