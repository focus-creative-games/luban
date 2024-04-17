using System.Runtime.Serialization;

namespace Luban.Utils;

public class LoadXmlException : Exception
{
    public LoadXmlException()
    {
    }

    public LoadXmlException(string message) : base(message)
    {
    }

    public LoadXmlException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
