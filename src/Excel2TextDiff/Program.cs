using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace Excel2TextDiff
{
    class CommandLineOptions
    {
        [Option('t', SetName = "transform", HelpText = "transform excel to text file")]
        public bool IsTransform { get; set; }

        [Option('d', SetName = "diff", HelpText = "transform and diff file")]
        public bool IsDiff { get; set; }

        [Option('p', SetName = "diff", Required = false, HelpText = "3rd diff program. default TortoiseMerge")]
        public string DiffProgram { get; set; }

        [Value(0)]
        public IList<string> Files { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length != 2 && args.Length != 3)
            //{
            //    Console.WriteLine("Usage:");
            //    Console.WriteLine("Excel2TextDiff <file1> <file2> [path_of_diff_program]");
            //    return;
            //}

            var options = ParseOptions(args);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var writer = new Excel2TextWriter();

            if (options.IsTransform)
            {
                if (options.Files.Count != 2)
                {
                    Console.WriteLine("Usage: Excel2TextDiff -t <excel file> <text file>");
                    Environment.Exit(1);
                }

                writer.TransformToTextAndSave(options.Files[0], options.Files[1]);
            }
            else
            {
                if (options.Files.Count != 2)
                {
                    Console.WriteLine("Usage: Excel2TextDiff -d <excel file 1> <excel file 2> [-p <diff programe>]");
                    Environment.Exit(1);
                }

                var diffProgame = options.DiffProgram ?? "TortoiseMerge.exe";

                var tempTxt1 = Path.GetTempFileName();
                writer.TransformToTextAndSave(options.Files[0], tempTxt1);

                var tempTxt2 = Path.GetTempFileName();
                writer.TransformToTextAndSave(options.Files[1], tempTxt2);

                string arg1 = $"/base:{tempTxt1.Replace('\\', '/')}";
                string arg2 = $"/mine:{tempTxt2.Replace('\\', '/')}";
                Console.WriteLine(" {0} {1}", arg1, arg2);
                System.Diagnostics.Process.Start(diffProgame, new string[] { arg1, arg2 });
            }
        }

        private static CommandLineOptions ParseOptions(String[] args)
        {
            var helpWriter = new StringWriter();
            var parser = new Parser(ps =>
            {
                ps.HelpWriter = helpWriter;
            });

            var result = parser.ParseArguments<CommandLineOptions>(args);
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.Error.WriteLine(helpWriter.ToString());
                Environment.Exit(1);
            }
            return ((Parsed<CommandLineOptions>)result).Value;
        }
    }
}
