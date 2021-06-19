using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Option('f', SetName = "diff", Required = false, HelpText = "3rd diff program argument format. default is TortoiseMerge format:'/base:{0} /mine:{1}'")]
        public string DiffProgramArgumentFormat { get; set; }

        [Value(0)]
        public IList<string> Files { get; set; }

        [Usage()]
        public static IEnumerable<Example> Examples => new List<Example>
        {
            new Example("tranfrom to text", new CommandLineOptions { IsTransform = true, Files = new List<string>{"a.xlsx", "a.txt" } }),
            new Example("diff two excel file", new CommandLineOptions{ IsDiff = true, Files = new List<string>{"a.xlsx", "b.xlsx"}}),
            new Example("diff two excel file with TortoiseMerge", new CommandLineOptions{ IsDiff = true, DiffProgram = "TortoiseMerge",DiffProgramArgumentFormat = "/base:{0} /mine:{1}",  Files = new List<string>{"a.xlsx", "b.xlsx"}}),
        };
    }

    class Program
    {
        static void Main(string[] args)
        {
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
                    Console.WriteLine("Usage: Excel2TextDiff -d <excel file 1> <excel file 2> ");
                    Environment.Exit(1);
                }

                var diffProgame = options.DiffProgram ?? "TortoiseMerge.exe";

                var tempTxt1 = Path.GetTempFileName();
                writer.TransformToTextAndSave(options.Files[0], tempTxt1);

                var tempTxt2 = Path.GetTempFileName();
                writer.TransformToTextAndSave(options.Files[1], tempTxt2);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = diffProgame;
                string argsFormation = options.DiffProgramArgumentFormat ?? "/base:{0} /mine:{1}";
                startInfo.Arguments = string.Format(argsFormation, tempTxt1, tempTxt2);
                Process.Start(startInfo);
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
