using System;
using System.Runtime.Serialization;

namespace Luban.Job.Cfg.DataCreators
{
    class InvalidExcelDataException : Exception
    {
        public InvalidExcelDataException()
        {
        }

        public InvalidExcelDataException(string message) : base(message)
        {
        }

        public InvalidExcelDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidExcelDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
