using CliLib;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml.Linq;

namespace Sample05_FileContent
{

    [Cli.Doc(@"Read secrets from /etc/passwd file")]
    [Cli.GenerateSample]
    internal class Program : Cli.IExecutable
    {
        [Cli.Doc("Print given arguments")]
        [Cli.Named('A')]
        [Cli.SampleValue]
        public bool PrintArgs = false;

        [Cli.Doc("content of file /etc/passwd")]
        [Cli.ContentOfFile("/etc/passwd", 1024 * 65)]
        [Cli.Required]
        [Cli.Secret]
        public MemoryStream PasswdContent;

        [Cli.Doc("The master key")]
        [Cli.EnvironmentVariable("MASTER_KEY")]
        [Cli.Interactive("Please enter master key", false)]
        [Cli.Required]
        [Cli.Secret]
        public string MasterKey = "123";

        [Cli.Doc("Non required appSettings parameter")]
        [Cli.AppSettings]
        public bool NonRequired = false;

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            if (this.PasswdContent == null)
            {
                Console.WriteLine($"{nameof(PasswdContent)} is empty or null");
                return;
            }
            using (var reader = new StreamReader(this.PasswdContent, Encoding.UTF8))
            {
                string asStr = reader.ReadToEnd();
                Console.WriteLine(asStr);
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