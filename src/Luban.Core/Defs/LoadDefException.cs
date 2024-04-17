using System.Runtime.Serialization;

namespace Luban.Defs;

public class LoadDefException : Exception
{
    public LoadDefException()
    {
    }

    public LoadDefException(string message) : base(message)
    {
    }

    public LoadDefException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
