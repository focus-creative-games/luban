using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class JsonDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static JsonDeserializeVisitor Ins { get; } = new ();

    public override string DoAccept(TType type, string jsonFieldName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({jsonFieldName} != undefined) {{ {type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonFieldName, fieldName)} }} else {{ {fieldName} = undefined }}";
        }
        else
        {
            return type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonFieldName, fieldName);
        }
    }

    //public override string Accept(TBean type, string bytebufName, string fieldName)
    //{
    //    return type.Apply(TypescriptJsonUnderingConstructorVisitor.Ins, bytebufName, fieldName);
    //}
}