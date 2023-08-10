using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class GoDeserializeJsonFieldVisitor : DecoratorFuncVisitor<string, string, string, string>
{
    public static GoDeserializeJsonFieldVisitor Ins { get; } = new GoDeserializeJsonFieldVisitor();

    public override string DoAccept(TType type, string varName, string fieldName, string bufName)
    {
        if (type.IsNullable)
        {
            var jsonObjName = $"__json_{fieldName}__";
            return $"{{ var _ok_ bool; var {jsonObjName} interface{{}}; if {jsonObjName}, _ok_ = {bufName}[\"{fieldName}\"]; !_ok_ || {jsonObjName} == nil {{ {varName} = nil }} else {{ var __x__ {type.Apply(GoUnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(GoDeserializeJsonUndering2Visitor.Ins, "__x__", jsonObjName)}; {varName} = {(type.Apply(GoIsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {
            return type.Apply(GoDeserializeJsonUnderingVisitor.Ins, varName, fieldName, bufName);
        }
    }
}