using System.Text;

namespace Luban;

public class OutputFile
{
    public string File { get; init; }

    /// <summary>
    /// Data type: string or byte[]
    /// </summary>
    public object Content { get; init; }

    public byte[] GetContentBytes()
    {
        if (Content is byte[] bytes)
        {
            return bytes;
        }
        return Encoding.UTF8.GetBytes((string)Content);
    }
}
