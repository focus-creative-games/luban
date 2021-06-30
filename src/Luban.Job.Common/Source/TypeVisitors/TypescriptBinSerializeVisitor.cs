using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.TypeVisitors
{
    class TypescriptBinSerializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static TypescriptBinSerializeVisitor Ins { get; } = new TypescriptBinSerializeVisitor();

        public override string DoAccept(TType type, string bytebufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({bytebufName}.ReadBool()) {{{type.Apply(TypescriptBinUnderingSerializeVisitor.Ins, bytebufName, fieldName)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TypescriptBinUnderingSerializeVisitor.Ins, bytebufName, fieldName);
            }
        }
    }
}
