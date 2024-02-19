using System.Text;

namespace Luban.CodeTarget;

public class CodeWriter
{
    private readonly List<string> _lines;

    public CodeWriter(int lineCapacity = 1000)
    {
        _lines = new List<string>(lineCapacity);
    }

    public void Write(string line)
    {
        _lines.Add(line);
    }

    private static string GetLineEnding()
    {
        string endings = EnvManager.Current.GetOptionOrDefault("", BuiltinOptionNames.LineEnding, true, "").ToLowerInvariant();
        switch (endings)
        {
            case "": return Environment.NewLine;
            case "crlf": return "\r\n";
            case "lf": return "\n";
            case "cr": return "\r";
            default: throw new Exception($"unknown line ending: {endings}");
        }
    }

    public string ToResult(string header)
    {
        var sb = new StringBuilder(100 * 1024);
        if (!string.IsNullOrEmpty(header))
        {
            sb.AppendLine(header);
        }
        foreach (var line in _lines)
        {
            sb.AppendLine(line);
        }
        string lineEnding = GetLineEnding();
        return sb.ToString().ReplaceLineEndings(lineEnding);
    }
}
