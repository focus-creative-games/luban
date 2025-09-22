using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Kotlin.TypeVisitors;

public class KotlinBinDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static KotlinBinDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string bufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({bufName}.readBool()) {{ {type.Apply(KotlinBinUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)} }} else {{ {fieldName} = null }}";
        }
        else
        {
            return type.Apply(KotlinBinUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0);
        }
    }
}