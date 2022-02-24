using Luban.Job.Common;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.TypeVisitors
{
    public class CsEditorInitValueVisitor : CsCtorValueVisitor
    {
        public static new CsEditorInitValueVisitor Ins { get; } = new CsEditorInitValueVisitor();

        public override string Accept(TEnum type)
        {
            return $"{(type.DefineEnum.Items.Count > 0 ? $"{type.Apply(CsEditorDefineTypeName.Ins)}."+type.DefineEnum.Items[0].Name : "default")}";
        }

        public override string Accept(TText type)
        {
            return $"new {CfgConstStrings.EditorTextTypeName}(\"\", \"\")";
        }

        public override string Accept(TDateTime type)
        {
            return "\"1970-01-01 00:00:00\"";
        }

        public override string Accept(TBean type)
        {
            return type.IsNullable || type.Bean.IsAbstractType ? "default" : $"new {type.Apply(CsEditorUnderlyingDefineTypeName.Ins)}()";
        }

        public override string Accept(TArray type)
        {
            return $"System.Array.Empty<{type.ElementType.Apply(CsEditorDefineTypeName.Ins)}>()";
        }

        public override string Accept(TList type)
        {
            return $"new {ConstStrings.CsList}<{type.ElementType.Apply(CsEditorDefineTypeName.Ins)}>()";
        }

        public override string Accept(TSet type)
        {
            return $"new {ConstStrings.CsHashSet}<{type.ElementType.Apply(CsEditorDefineTypeName.Ins)}>()";
        }

        public override string Accept(TMap type)
        {
            return $"new {ConstStrings.CsHashMap}<{type.KeyType.Apply(CsEditorDefineTypeName.Ins)},{type.ValueType.Apply(CsEditorDefineTypeName.Ins)}>()";
        }
    }
}
