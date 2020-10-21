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
                return $"{{ bool _read_succ_; if(!{bufName}.readBool(_read_succ_)){{return false;}}  if(_read_succ_) {{ {type.Apply(CppUnderingDeserializeVisitor.Ins, bufName, fieldName)} }} else {{ {fieldName} = {{}}; }} }}";
            }
            else
            {
                return type.Apply(CppUnderingDeserializeVisitor.Ins, bufName, fieldName);
            }
        }

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            return type.Apply(CppUnderingDeserializeVisitor.Ins, bufName, fieldName);
        }
    }
}
