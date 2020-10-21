using Bright.Net.Codecs;
using Bright.Serialization;

namespace Luban.Common.Protos
{

    public class PushLog : Protocol
    {
        public const int ID = 104;
        public override int GetTypeId()
        {
            return ID;
        }

        public string Level { get; set; }

        public string LogContent { get; set; }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(Level);
            os.WriteString(LogContent);
        }

        public override void Deserialize(ByteBuf os)
        {
            Level = os.ReadString();
            LogContent = os.ReadString();
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
