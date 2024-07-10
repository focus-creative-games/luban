using System.Text;

namespace Luban;

public class OutputFile
{
    public string File { get; init; }

    /// <summary>
    /// Data type: string or byte[]
    /// </summary>
    public object Content { get; init; }

    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public byte[] GetContentBytes()
    {
        if (Content is byte[] bytes)
        {
            return bytes;
        }
        return this.Encoding.GetBytes((string)Content);
    }
}
