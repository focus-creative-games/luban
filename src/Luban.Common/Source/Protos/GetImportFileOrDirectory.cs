using Bright.Net.Codecs;
using Bright.Serialization;
using System;
using System.Collections.Generic;

namespace Luban.Common.Protos
{
    public class GetImportFileOrDirectoryArg : BeanBase
    {
        public string FileOrDirName { get; set; }

        public List<string> InclusiveSuffixs { get; set; } = new List<string>();

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(FileOrDirName);
            Bright.Common.SerializationUtil.Serialize(os, InclusiveSuffixs);
        }
        public override void Deserialize(ByteBuf os)
        {
            FileOrDirName = os.ReadString();
            Bright.Common.SerializationUtil.Deserialize(os, InclusiveSuffixs);
        }
    }

    public class GetImportFileOrDirectoryRes : BeanBase
    {
        public EErrorCode Err { get; set; }

        public bool IsFile { get; set; }

        public string Md5 { get; set; }

        //public byte[] Content { get; set; }

        public List<FileInfo> SubFiles { get; set; }

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteInt((int)Err);
            os.WriteBool(IsFile);
            os.WriteString(Md5);
            Bright.Common.SerializationUtil.Serialize(os, SubFiles);
        }
        public override void Deserialize(ByteBuf os)
        {
            Err = (EErrorCode)os.ReadInt();
            IsFile = os.ReadBool();
            Md5 = os.ReadString();
            Bright.Common.SerializationUtil.Deserialize(os, SubFiles = new List<FileInfo>());
        }
    }

    public class GetImportFileOrDirectory : Rpc<GetImportFileOrDirectoryArg, GetImportFileOrDirectoryRes>
    {
        public const int ID = 108;

        public override int GetTypeId()
        {
            return ID;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
