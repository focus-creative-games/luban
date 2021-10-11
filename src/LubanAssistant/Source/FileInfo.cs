using Bright.Serialization;

namespace Luban.Common.Protos
{
    public class FileInfo : BeanBase
    {
        public string FilePath { get; set; }

        public string MD5 { get; set; }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(FilePath);
            os.WriteString(MD5);
        }

        public override void Deserialize(ByteBuf os)
        {
            FilePath = os.ReadString();
            MD5 = os.ReadString();
        }

        public override int GetTypeId()
        {
            throw new System.NotImplementedException();
        }
    }
}
