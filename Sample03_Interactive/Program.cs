using CliLib;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sample03_Interactive
{

    [Cli.Doc(@"The test program to demo using interactive input and environment variables")]
    internal class Program : Cli.IExecutable
    {
        [Cli.Doc("Print given arguments before execution")]
        [Cli.Named('A')]
        public bool PrintArgs = false;
        
        [Cli.Doc("Master key to decrypt secrets stored within configuration")]
        [Cli.Interactive("Please enter master key", false)]
        [Cli.EnvironmentVariable("MASTER_KEY")]
        [Cli.Secret]
        [Cli.Required]
        public string MasterKey = string.Empty;

        [Cli.Doc("Connection password stored within app.config")]
        [Cli.EnvironmentVariable]
        [Cli.AppSettings]
        [Cli.Secret]
        [Cli.Required]
        public string ConnectionPassword = "123";

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            Console.WriteLine("Now you would see results of decryption of the password stored within app.config.");
            Cli.AskIfUserWantedContinue("Would you like to continue?", "Yes", "No");
            var decrypted = ImitationOfDecryption(this.ConnectionPassword, this.MasterKey);
            Console.WriteLine($"Original masterKey={this.MasterKey}, and connectionPassword={this.ConnectionPassword}, decrypted={decrypted}");
        }

        public static string ImitationOfDecryption(string connectionPassword, string masterKey)
        {
            return $"Decrypted[{connectionPassword}] with key={masterKey}";
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