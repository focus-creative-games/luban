using Bright.Net.Codecs;
using Bright.Serialization;
using System;
using System.Collections.Generic;

namespace Luban.Common.Protos
{
    public class QueryFilesExistsArg : BeanBase
    {
        public string Root { get; set; }

        public List<string> Files { get; set; }

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteString(Root);
            Bright.Common.SerializationUtil.Serialize(os, Files);
        }
        public override void Deserialize(ByteBuf os)
        {
            Root = os.ReadString();
            Bright.Common.SerializationUtil.Deserialize(os, Files = new List<string>());
        }
    }

    public class QueryFilesExistsRes : BeanBase
    {
        public List<bool> Exists { get; set; }

        public override int GetTypeId()
        {
            return 0;
        }

        public override void Serialize(ByteBuf os)
        {
            os.WriteSize(Exists.Count);
            foreach (var v in Exists)
            {
                os.WriteBool(v);
            }
        }
        public override void Deserialize(ByteBuf os)
        {
            int n = os.ReadSize();
            Exists = new List<bool>();
            for (int i = 0; i < n; i++)
            {
                Exists.Add(os.ReadBool());
            }
        }
    }

    public class QueryFilesExists : Rpc<QueryFilesExistsArg, QueryFilesExistsRes>
    {
        public const int ID = 106;

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
