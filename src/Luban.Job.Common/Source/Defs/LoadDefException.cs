using System;
using System.Runtime.Serialization;

namespace Luban.Job.Common.Defs
{
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

        protected LoadDefException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
