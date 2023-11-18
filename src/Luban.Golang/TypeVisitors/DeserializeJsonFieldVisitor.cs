using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class DeserializeJsonFieldVisitor : DecoratorFuncVisitor<string, string, string, string>
{
    public static DeserializeJsonFieldVisitor Ins { get; } = new DeserializeJsonFieldVisitor();

    public override string DoAccept(TType type, string varName, string fieldName, string bufName)
    {
        if (type.IsNullable)
        {
            var jsonObjName = $"__json_{fieldName}__";
            return $"{{ var _ok_ bool; var {jsonObjName} interface{{}}; if {jsonObjName}, _ok_ = {bufName}[\"{fieldName}\"]; !_ok_ || {jsonObjName} == nil {{ {varName} = nil }} else {{ var __x__ {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(DeserializeJsonUndering2Visitor.Ins, "__x__", jsonObjName)}; {varName} = {(type.Apply(IsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {
            return type.Apply(DeserializeJsonUnderingVisitor.Ins, varName, fieldName, bufName);
        }
    }
}
