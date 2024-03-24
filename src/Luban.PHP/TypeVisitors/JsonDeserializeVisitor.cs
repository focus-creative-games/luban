using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.PHP.TypeVisitors;

public class JsonDeserializeVisitor : DecoratorFuncVisitor<string, string, int, string>
{
    public static JsonDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string jsonFieldName, string fieldName, int depth)
    {
        if (type.IsNullable)
        {
            return $"if({jsonFieldName} != null) {{ {type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonFieldName, fieldName, depth)}; }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonFieldName, fieldName, depth);
        }
    }

    //public override string Accept(TBean type, string bytebufName, string fieldName)
    //{
    //    return type.Apply(TypescriptJsonUnderingConstructorVisitor.Ins, bytebufName, fieldName);
    //}
}
