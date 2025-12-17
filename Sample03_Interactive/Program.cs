using CliLib;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sample03_Interactive
{

    [Cli.Doc(@"The test program to demo using interactive input and environment variables")]
    [Cli.GenerateSample]
    internal class Program : Cli.IExecutable
    {
        [Cli.Doc("Print given arguments before execution")]
        [Cli.Named('A')]
        [Cli.SampleValue]
        public bool PrintArgs = false;
        
        [Cli.Doc("Master key to decrypt secrets that stored within app.config")]
        [Cli.Interactive("Please enter master key", false)]
        [Cli.EnvironmentVariable("MASTER_KEY")]
        [Cli.Secret]
        [Cli.Required]
        [Cli.AllowedRegexPattern("^.{8,}$")]
        public string MasterKey = string.Empty;

        [Cli.Doc("Connection password stored within app.config")]
        [Cli.AppSettings]
        [Cli.Secret]
        [Cli.Required]
        public string ConnectionPassword = "123";

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            Console.WriteLine("Now you would see results of decryption of the password stored within app.config.");
            try
            {
                Cli.AskIfUserWantedContinue("Would you like to continue?", "Yes", "No");

                var decrypted = ImitationOfDecryption(this.ConnectionPassword, this.MasterKey);
                Console.WriteLine($"Original masterKey={this.MasterKey}, and connectionPassword={this.ConnectionPassword}, decrypted={decrypted}");
            }
            catch (Cli.UserInterruptedInputException ex)
            {
                Console.WriteLine(ex.Message);
            }
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