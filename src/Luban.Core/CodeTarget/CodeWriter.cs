using Luban.Utils;
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
        return sb.ToString();
    }
}
