using Bright.Net.Codecs;
using Bright.Serialization;

namespace Luban.Common.Protos
{
    public class GetInputFileArg : BeanBase
    {
        public string File { get; set; }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(File);
        }

        public override void Deserialize(ByteBuf os)
        {
            File = os.ReadString();
        }

        public override int GetTypeId()
        {
            throw new System.NotImplementedException();
        }
    }



    public class GetInputFileRes : BeanBase
    {
        public EErrorCode Err { get; set; }

        public byte[] Content { get; set; }

        public override void Serialize(ByteBuf os)
        {
            os.WriteInt((int)Err);
            os.WriteBytes(Content);
        }

        public override void Deserialize(ByteBuf os)
        {
            Err = (EErrorCode)os.ReadInt();
            Content = os.ReadBytes();
        }

        public override int GetTypeId()
        {
            throw new System.NotImplementedException();
        }
    }

    public class GetInputFile : Rpc<GetInputFileArg, GetInputFileRes>
    {
        public const int ID = 102;

        public override int GetTypeId()
        {
            return ID;
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
