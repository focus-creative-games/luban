using Bright.Net.Codecs;
using Bright.Serialization;

namespace Luban.Common.Protos
{

    public class GetOutputFile : Rpc<GetOutputFileArg, GetOutputFileRes>
    {
        public const int ID = 103;

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


    public class GetOutputFileArg : BeanBase
    {
        //public string Type { get; set; }

        //public string RelatePath { get; set; }

        public string MD5 { get; set; }

        public override int GetTypeId()
        {
            throw new System.NotImplementedException();
        }

        public override void Serialize(ByteBuf os)
        {
            //os.WriteString(Type);
            //os.WriteString(RelatePath);
            os.WriteString(MD5);
        }

        public override void Deserialize(ByteBuf os)
        {
            //Type = os.ReadString();
            //RelatePath = os.ReadString();
            MD5 = os.ReadString();
        }
    }



    public class GetOutputFileRes : BeanBase
    {
        public bool Exists { get; set; }
        public byte[] FileContent { get; set; }

        public override void Serialize(ByteBuf os)
        {
            os.WriteBool(Exists);
            os.WriteBytes(FileContent);
        }

        public override void Deserialize(ByteBuf os)
        {
            Exists = os.ReadBool();
            FileContent = os.ReadBytes();
        }

        public override int GetTypeId()
        {
            throw new System.NotImplementedException();
        }
    }
}
