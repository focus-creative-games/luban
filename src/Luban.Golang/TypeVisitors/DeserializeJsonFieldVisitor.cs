using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class DeserializeJsonFieldVisitor : DecoratorFuncVisitor<string, string, string, string>
{
    public static DeserializeJsonFieldVisitor Ins { get; } = new DeserializeJsonFieldVisitor();

    public override string DoAccept(TType type, string varName, string fieldName, string bufName)
    {
        var jsonObjName = $"__json_{fieldName}__";
        if (type.IsNullable)
        {
            return $"{{ var _ok_ bool; var {jsonObjName} interface{{}}; if {jsonObjName}, _ok_ = {bufName}[\"{fieldName}\"]; !_ok_ || {jsonObjName} == nil {{ {varName} = nil }} else {{ var __x__ {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(DeserializeJsonUnderingVisitor.Ins, "__x__", jsonObjName, 0)}; {varName} = {(type.Apply(IsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {

            return $"{{ var _ok_ bool; var {jsonObjName} interface{{}}; if {jsonObjName}, _ok_ = {bufName}[\"{fieldName}\"]; !_ok_ || {jsonObjName} == nil {{ err = errors.New(\"{fieldName} error\"); return }} else {{ var __x__ {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(DeserializeJsonUnderingVisitor.Ins, "__x__", jsonObjName, 0)}; {varName} = {(type.Apply(IsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
            //return type.Apply(DeserializeJsonUnderingVisitor.Ins, varName, fieldName, bufName, 0);
        }
    }
}
