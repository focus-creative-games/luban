using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static GoDeserializeVisitor Ins { get; } = new GoDeserializeVisitor();

        public override string DoAccept(TType type, string fieldName, string bufName)
        {
            if (type.IsNullable)
            {
                return $"{{ var __exists__ bool; if __exists__, err = {bufName}.ReadBool(); err != nil {{ return }}; if __exists__ {{ var __x__ {type.Apply(GoTypeUnderingNameVisitor.Ins)};  {type.Apply(GoDeserializeUnderingVisitor.Ins, "__x__", bufName)}; {fieldName} = {(type.Apply(IsGoPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
            }
            else
            {
                return type.Apply(GoDeserializeUnderingVisitor.Ins, (string)fieldName, bufName);
            }
        }
    }
}
