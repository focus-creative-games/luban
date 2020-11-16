
using Luban.Common.Utils;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Proto.Defs
{
    class DefProto : ProtoDefTypeBase
    {

        public DefProto(PProto b)
        {
            Name = b.Name;
            Namespace = b.Namespace;
            Id = b.Id;

            foreach (var field in b.Fields)
            {
                Fields.Add(new DefField(this, field, 0));
            }
        }

        public List<DefField> Fields { get; set; } = new List<DefField>();

        public override void Compile()
        {
            var pass = Assembly;
            if (Id == 0)
            {
                Id = (int)TypeUtil.ComputProtoHashIdByName(FullName);
            }
            pass.AddProto(this);
            DefField.CompileFields(this, Fields, false);
        }
    }
}
