using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using Luban.Job.Proto.TypeVisitors;

namespace Luban.Job.Proto.Defs
{
    class TTypeTemplateExtends : TTypeTemplateCommonExtends
    {
        public static string CsSerialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(CsSerializeVisitor.Ins, bufName, fieldName);
        }

        public static string CsCompatibleSerialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(CsSerializeVisitor.Ins, bufName, fieldName);
        }

        public static string CsDeserialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(CsDeserializeVisitor.Ins, bufName, fieldName);
        }

        public static string CsCompatibleDeserialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(CsDeserializeVisitor.Ins, bufName, fieldName);
        }

        public static string LuaCommentType(TType type)
        {
            return type.Apply(LuaCommentTypeVisitor.Ins);
        }

        public static string LuaUnderingDeserialize(string bufName, TType type)
        {
            return type.Apply(LuaUnderingDeserializeVisitor.Ins, bufName);
        }

        public static string CsInitFieldCtorValue(DefField field)
        {
            return $"{field.CsStyleName} = default;";
        }
    }
}
