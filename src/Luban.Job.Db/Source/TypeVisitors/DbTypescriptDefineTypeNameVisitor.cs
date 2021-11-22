using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbTypescriptDefineTypeNameVisitor : TypescriptDefineTypeNameVisitor
    {
        public static new DbTypescriptDefineTypeNameVisitor Ins { get; } = new();

        public override string Accept(TArray type)
        {
            throw new NotSupportedException();
        }

        public override string Accept(TList type)
        {
            if (type.ElementType.IsBean)
            {
                return $"PList2<{type.ElementType.Apply(this)}>";
            }
            else
            {
                return $"PList1<{type.ElementType.Apply(this)}>";
            }
        }

        public override string Accept(TSet type)
        {
            return $"PSet<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TMap type)
        {
            if (type.ValueType.IsBean)
            {
                return $"PMap2<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
            }
            else
            {
                return $"PMap1<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
            }
        }
    }
}
