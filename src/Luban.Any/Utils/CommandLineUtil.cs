namespace Luban.Any.Utils;

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