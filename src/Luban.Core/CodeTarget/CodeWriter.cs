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
        sb.Replace("\r\n", "\n");
        sb.Replace("\r", "\n");
        string lineEndingStyle = EnvManager.Current.GetOptionOrDefault(string.Empty, BuiltinOptionNames.CodeLineEndingStyle, true, string.Empty).ToUpper();
        if (lineEndingStyle == "CRLF")
        {
            sb.Replace("\n", "\r\n");
        }
        return sb.ToString();
    }
}
