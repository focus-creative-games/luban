using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class BinaryDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static BinaryDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string bufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({bufName}.ReadBool()){{ {type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)} }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0);
        }
    }
}
