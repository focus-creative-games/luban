using Luban.Common.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Proto.RawDefs;
using System;

namespace Luban.Job.Proto.Defs
{
    class DefRpc : ProtoDefTypeBase
    {

        public DefRpc(PRpc r)
        {
            Name = r.Name;
            Namespace = r.Namespace;
            Id = r.Id;
            ArgType = r.ArgType;
            ResType = r.ResType;
            Comment = r.Comment;
        }

        public string ArgType { get; set; }

        public string ResType { get; set; }

        public TType TArgType { get; private set; }

        public TType TResType { get; private set; }


        public override void Compile()
        {
            var pass = Assembly;
            if (Id == 0)
            {
                Id = (int)TypeUtil.ComputProtoHashIdByName(FullName);
            }
            pass.AddProto(this);

            if ((TArgType = Assembly.CreateType(Namespace, ArgType, false)) == null)
            {
                throw new Exception($"rpc name:'{FullName}' arg:{ArgType} is invalid");
            }

            if ((TResType = Assembly.CreateType(Namespace, ResType, false)) == null)
            {
                throw new Exception($"rpc name:'{FullName}' res:{ResType} is invalid");
            }
        }
    }
}
