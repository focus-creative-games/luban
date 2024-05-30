using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Java.TypeVisitors;

public class JavaBinDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static JavaBinDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string bufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({bufName}.readBool()){{ {type.Apply(JavaBinUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)} }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(JavaBinUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0);
        }
    }
}
