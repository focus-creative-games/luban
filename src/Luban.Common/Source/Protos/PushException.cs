using Bright.Net.Codecs;
using Bright.Serialization;

namespace Luban.Common.Protos
{
    public class PushException : Protocol
    {
        public const int ID = 105;
        public override int GetTypeId()
        {
            return ID;
        }

        public string LogContent { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(LogContent);
            os.WriteString(Message);
            os.WriteString(StackTrace);
        }

        public override void Deserialize(ByteBuf os)
        {
            LogContent = os.ReadString();
            Message = os.ReadString();
            StackTrace = os.ReadString();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
