using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CppDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static CppDeserializeVisitor Ins { get; } = new CppDeserializeVisitor();

        public override string DoAccept(TType type, string bufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"{{ bool _has_value_; if(!{bufName}.readBool(_has_value_)){{return false;}}  if(_has_value_) {{{type.Apply(CppUnderingDefineTypeName.Ins)} _temp_;  {type.Apply(CppUnderingDeserializeVisitor.Ins, bufName, "_temp_")} {(type.IsBean ? $"{fieldName} = _temp_;" : $"{fieldName} = new {type.Apply(CppUnderingDefineTypeName.Ins)}; *{fieldName} = _temp_;")} }} else {{ {fieldName} = nullptr; }} }}";
            }
            else
            {
                return type.Apply(CppUnderingDeserializeVisitor.Ins, bufName, fieldName);
            }
        }
    }
}
