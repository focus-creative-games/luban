using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Proto.Defs
{
    class TTypeTemplateExtends : TTypeTemplateCommonExtends
    {

        public static string JavaSerialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(JavaSerializeVisitor.Ins, bufName, fieldName);
        }

        public static string JavaDeserialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(JavaDeserializeVisitor.Ins, bufName, fieldName);
        }
    }
}
