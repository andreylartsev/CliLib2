using CliLib;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml.Linq;

namespace Sample04_XmlC14n
{

    [Cli.Doc(@"Canonization of the XML documents")]
    [Cli.GenerateSample]
    internal class Program : Cli.IExecutable
    {
        [Cli.Doc("Print given arguments")]
        [Cli.Named('A')]
        public bool PrintArgs = false;

        [Cli.Doc("File name to read")]
        [Cli.Named('I')]
        [Cli.SampleValue("doc1.xml")]
        [Cli.Required]
        public FileInfo InFile;

        [Cli.Doc("File name to write")]
        [Cli.Named('O')]
        [Cli.SampleValue("doc2.xml")]
        [Cli.Required]
        public FileInfo OutFile;

        public enum SignatureRemovingMethod { None, BySubstringRemoving };

        [Cli.Doc("The method of removing Signature element")]
        [Cli.SampleValue(nameof(SignatureRemovingMethod.BySubstringRemoving))]
        [Cli.Named]
        public SignatureRemovingMethod RemovingMethod = SignatureRemovingMethod.BySubstringRemoving;

        [Cli.Doc("The output file encoding")]
        [Cli.SampleValue("UTF-8")]
        [Cli.Named]
        public string OutputFileEncoding = "UTF-8";

        public void Exec()
        {
            if (this.PrintArgs)
                Cli.PrintArgs(this);

            using (var reader = MakeInputReader())
            {
                using (var outputStream = MakeOutputStream())
                {
                    using (var writer = MakeOutputWriter(outputStream))
                    {
                        string envelope = reader.ReadToEnd();
                        var transformed = GetByteRepresentationWithoutSignature(envelope, this.RemovingMethod);
                        writer.Write(transformed);
                    }
                }
            }

        }

        private Encoding GetOutputFileEncodingByName()
        {
            if (this.OutputFileEncoding == null)
                throw new ArgumentNullException(nameof(this.OutputFileEncoding));
            return Encoding.GetEncoding(OutputFileEncoding);
        }

        private StreamReader MakeInputReader()
        {
            if (this.InFile == null)
                throw new InvalidOperationException($"The parameter {nameof(InFile)} must be provided");

            if (!this.InFile.Exists)
                throw new InvalidOperationException($"The file specified by parameter {nameof(InFile)} does not exists");

            var reader = new StreamReader(this.InFile.FullName);
            return reader;
        }

        private FileStream MakeOutputStream()
        {
            if (this.OutFile == null)
                throw new InvalidOperationException($"The parameter {nameof(OutFile)} must be provided");

            var outputStream = new FileStream(this.OutFile.FullName, FileMode.Create);
            return outputStream;
        }
        private BinaryWriter MakeOutputWriter(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            var outputEncoding = GetOutputFileEncodingByName();
            var writer = new BinaryWriter(stream, outputEncoding);
            return writer;
        }

        public static byte[] GetByteRepresentationWithoutSignature(string stringSourceRepresentation, SignatureRemovingMethod removingMethod)
        {
            var transformer = new XmlDsigExcC14NWithCommentsTransform();

            object xmlOutput;

            string docRepresentation = RemoveSignatureContent(stringSourceRepresentation, removingMethod);

            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(docRepresentation)))
            {
                transformer.LoadInput(inputStream);
                xmlOutput = transformer.GetOutput();
            }

            return ((MemoryStream)xmlOutput).ToArray();
        }

        private static string RemoveSignatureContent(string stringSourceRepresentation, SignatureRemovingMethod removingMethod)
        {
            string docRepresentation;

            switch (removingMethod)
            {
                case SignatureRemovingMethod.None:
                    docRepresentation = stringSourceRepresentation;
                    break;
                case SignatureRemovingMethod.BySubstringRemoving:
                    docRepresentation = GetSourceStringRepresentationWithoutSignature(stringSourceRepresentation);
                    break;
                default:
                    throw new NotImplementedException($"Unknown removing method '{removingMethod}'");
            }

            return docRepresentation;
        }

        private static string GetSourceStringRepresentationWithoutSignature(string stringSourceRepresentation)
        {
            const string StartTag = "<Security>";
            const string EndTag = "</Security>";

            int securityTagStartIndex = stringSourceRepresentation.IndexOf(StartTag, StringComparison.Ordinal);
            if (securityTagStartIndex < 0)
                throw new InvalidOperationException($"The start tag '{StartTag}' was not found in the source document");

            int securityTagEndIndex = stringSourceRepresentation.IndexOf(EndTag, StringComparison.Ordinal);
            if (securityTagEndIndex < 0)
                throw new InvalidOperationException($"The end tag '{EndTag}' was not found in the source document");
            if (securityTagEndIndex <= securityTagStartIndex)
                throw new InvalidOperationException($"The end tag '{EndTag}' has been found in a position {securityTagEndIndex} that is before position of the start tag {securityTagEndIndex}");

            var result = stringSourceRepresentation.Remove(securityTagStartIndex, (securityTagEndIndex + EndTag.Length) - securityTagStartIndex);
            return result;
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