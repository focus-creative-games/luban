using Bright.Net.Codecs;
using Bright.Serialization;
using System;
using System.Collections.Generic;

namespace Luban.Common.Protos
{
    public class GenJobArg : BeanBase
    {
        public string JobType { get; set; }

        public List<string> JobArguments { get; set; }

        public bool Verbose { get; set; }

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(JobType);
            Bright.Common.SerializationUtil.Serialize(os, JobArguments);
            os.WriteBool(Verbose);
        }

        public override void Deserialize(ByteBuf os)
        {
            JobType = os.ReadString();
            Bright.Common.SerializationUtil.Deserialize(os, JobArguments = new List<string>());
            Verbose = os.ReadBool();
        }
    }


    public class FileGroup : BeanBase
    {
        public string Dir { get; set; }

        public List<FileInfo> Files { get; set; }

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(Dir);
            Bright.Common.SerializationUtil.Serialize(os, Files);

        }

        public override void Deserialize(ByteBuf os)
        {
            Dir = os.ReadString();
            Bright.Common.SerializationUtil.Deserialize(os, Files = new List<FileInfo>());
        }
    }


    public class GenJobRes : BeanBase
    {
        public EErrorCode ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public List<FileGroup> FileGroups { get; set; } = new List<FileGroup>();

        public List<FileInfo> ScatteredFiles { get; set; } = new List<FileInfo>();

        public string StackTrace { get; set; }

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteInt((int)ErrCode);
            os.WriteString(ErrMsg);
            Bright.Common.SerializationUtil.Serialize(os, FileGroups);
            Bright.Common.SerializationUtil.Serialize(os, ScatteredFiles);
            os.WriteString(StackTrace);
        }
        public override void Deserialize(ByteBuf os)
        {
            ErrCode = (EErrorCode)os.ReadInt();
            ErrMsg = os.ReadString();
            Bright.Common.SerializationUtil.Deserialize(os, FileGroups);
            Bright.Common.SerializationUtil.Deserialize(os, ScatteredFiles);
            StackTrace = os.ReadString();
        }
    }

    public class GenJob : Rpc<GenJobArg, GenJobRes>
    {
        public const int ID = 100;

        public override int GetTypeId()
        {
            return ID;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
