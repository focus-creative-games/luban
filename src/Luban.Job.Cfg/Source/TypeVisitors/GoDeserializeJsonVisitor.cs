using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoDeserializeJsonVisitor : DecoratorFuncVisitor<string, string, string, string>
    {
        public static GoDeserializeJsonVisitor Ins { get; } = new GoDeserializeJsonVisitor();

        public override string DoAccept(TType type, string varName, string fieldName, string bufName)
        {
            if (type.IsNullable)
            {
                var jsonObjName = $"__json_{fieldName}__";
                return $"{{ var _ok_ bool; var {jsonObjName} interface{{}}; if {jsonObjName}, _ok_ = {bufName}[\"{fieldName}\"]; !_ok_ || {jsonObjName} == nil {{ {varName} = nil }} else {{ var __x__ {type.Apply(GoTypeUnderingNameVisitor.Ins)};  {type.Apply(GoDeserializeJsonUndering2Visitor.Ins, "__x__", jsonObjName)}; {varName} = {(type.Apply(GoIsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
            }
            else
            {
                return type.Apply(GoDeserializeJsonUnderingVisitor.Ins, varName, fieldName, bufName);
            }
        }
    }
}
