using Luban.Job.Common.Types;
using System.Collections.Generic;

namespace Luban.Job.Common.TypeVisitors
{
    public class GoBinImport : DecoratorActionVisitor<HashSet<string>>
    {
        public static GoBinImport Ins { get; } = new();

        public override void DoAccept(TType type, HashSet<string> x)
        {

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

        }

        public override void Accept(TMap type, HashSet<string> x)
        {
            type.KeyType.Apply(this, x);
            type.ValueType.Apply(this, x);
        }

        public override void Accept(TVector2 type, HashSet<string> x)
        {
            //x.Add("bright/serialization");
        }

        public override void Accept(TVector3 type, HashSet<string> x)
        {
            //x.Add("bright/serialization");
        }

        public override void Accept(TVector4 type, HashSet<string> x)
        {
            //x.Add("bright/serialization");
        }
    }
}
