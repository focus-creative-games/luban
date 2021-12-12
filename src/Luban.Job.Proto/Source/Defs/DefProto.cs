
using Luban.Common.Utils;
using Luban.Job.Proto.RawDefs;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Proto.Defs
{
    class DefProto : ProtoDefTypeBase
    {

        public DefProto(PProto b)
        {
            Name = b.Name;
            Namespace = b.Namespace;
            Id = b.Id;
            Comment = b.Comment;

            foreach (var field in b.Fields)
            {
                Fields.Add(new DefField(this, field, 0));
            }
        }

        public List<DefField> Fields { get; set; } = new List<DefField>();

        public virtual string GoBinImport
        {
            get
            {
                var imports = new HashSet<string>();
                if (this.Fields.Count > 0)
                {
                    imports.Add("errors");
                }
                foreach (var f in Fields)
                {
                    f.CType.Apply(Luban.Job.Common.TypeVisitors.GoBinImport.Ins, imports);
                }
                return string.Join('\n', imports.Select(im => $"import \"{im}\""));
            }
        }

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
