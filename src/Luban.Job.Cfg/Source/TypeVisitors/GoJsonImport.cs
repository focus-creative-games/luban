using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System.Collections.Generic;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoJsonImport : DecoratorActionVisitor<HashSet<string>>
    {
        public static GoJsonImport Ins { get; } = new();

        public override void DoAccept(TType type, HashSet<string> x)
        {
            x.Add("errors");
        }

        public override void Accept(TArray type, HashSet<string> x)
        {
            type.ElementType.Apply(this, x);
        }

        public override void Accept(TList type, HashSet<string> x)
        {
            type.ElementType.Apply(this, x);
        }

        public override void Accept(TSet type, HashSet<string> x)
        {
            type.ElementType.Apply(this, x);
        }

        public override void Accept(TMap type, HashSet<string> x)
        {
            type.KeyType.Apply(this, x);
            type.ValueType.Apply(this, x);
        }

        public override void Accept(TVector2 type, HashSet<string> x)
        {
            x.Add("errors");
            x.Add($"{DefAssembly.LocalAssebmly.Args.GoBrightModuleName}/serialization");
        }

        public override void Accept(TVector3 type, HashSet<string> x)
        {
            x.Add("errors");
            x.Add($"{DefAssembly.LocalAssebmly.Args.GoBrightModuleName}/serialization");
        }

        public override void Accept(TVector4 type, HashSet<string> x)
        {
            x.Add("errors");
            x.Add($"{DefAssembly.LocalAssebmly.Args.GoBrightModuleName}/serialization");
        }
    }
}
