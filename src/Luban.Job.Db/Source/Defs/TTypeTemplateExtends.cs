using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Db.TypeVisitors;
using System.Text;

namespace Luban.Job.Db.Defs
{
    class TTypeTemplateExtends : TTypeTemplateCommonExtends
    {
        public static string DbCsDefineType(TType type)
        {
            return type.Apply(DbCsDefineTypeVisitor.Ins);
        }

        public static string CsImmutableType(TType type)
        {
            return type.Apply(ImmutableTypeName.Ins);
        }

        public static string DbCsInitField(string fieldName, string logType, TType type)
        {
            return type.Apply(DbCsInitFieldVisitor.Ins, fieldName, logType);
        }

        public static bool HasSetter(TType type)
        {
            return type.Apply(BeanFieldHasSetterVisitor.Ins);
        }

        public static bool NeedSetChildrenRoot(TType type)
        {
            return type.Apply(NeedSetChildrenRootVisitor.Ins);
        }

        public static string DbCsCompatibleSerialize(string bufName, string fieldName, TType type)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                var sb = new StringBuilder($"{bufName}.BeginWriteSegment(out var _state_);");
                sb.Append(type.Apply(DbCsCompatibleSerializeVisitor.Ins, bufName, fieldName));
                sb.Append("_buf.EndWriteSegment(_state_);");
                return sb.ToString();
            }
            else
            {
                return type.Apply(DbCsCompatibleSerializeVisitor.Ins, bufName, fieldName);
            }
        }

        public static string DbCsCompatibleDeserialize(string bufName, string fieldName, TType type)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                var sb = new StringBuilder($"{bufName}.EnterSegment(out var _state_);");
                sb.Append(type.Apply(DbCsCompatibleDeserializeVisitor.Ins, bufName, fieldName));
                sb.Append("_buf.LeaveSegment(_state_);");
                return sb.ToString();
            }
            else
            {
                return type.Apply(DbCsCompatibleDeserializeVisitor.Ins, bufName, fieldName);
            }
        }


        public static string CsInitFieldCtorValue(DefField field)
        {
            return $"{field.CsStyleName} = default;";
        }

        public static string CsWriteBlob(string bufName, string valueName, TType type)
        {
            return type.Apply(DbWriteBlob.Ins, bufName, valueName);
        }
    }
}
