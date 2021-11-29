using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Common.Utils
{
    public static class CommandLineUtil
    {
        public static T ParseOptions<T>(String[] args)
        {
            var helpWriter = new StringWriter();
            var parser = new Parser(ps =>
            {
                ps.HelpWriter = helpWriter;
            });

            var result = parser.ParseArguments<T>(args);
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.Error.WriteLine(helpWriter.ToString());
                Environment.Exit(1);
            }
            return ((Parsed<T>)result).Value;
        }
    }
}
