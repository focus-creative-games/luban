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

        public static string DbCsReadonlyDefineType(TType type)
        {
            return type.Apply(DbCsReadOnlyDefineTypeVisitor.Ins);
        }

        public static string CsImmutableType(TType type)
        {
            return type.Apply(ImmutableTypeName.Ins);
        }

        public static string DbCsInitField(string fieldName, TType type)
        {
            return type.Apply(DbCsInitFieldVisitor.Ins, fieldName);
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
            return $"{field.ConventionName} = default;";
        }

        public static string CsWriteBlob(string bufName, string fieldName, TType type)
        {
            //return type.Apply(DbWriteBlob.Ins, bufName, valueName);
            return DbCsCompatibleSerialize(bufName, fieldName, type);
        }

        public static string DbTsDefineType(TType type)
        {
            return type.Apply(DbTypescriptDefineTypeNameVisitor.Ins);
        }

        public static string DbTsInitField(string fieldName, string logType, TType type)
        {
            return type.Apply(DbTypescriptInitFieldVisitor.Ins, fieldName, logType);
        }

        public static string TsWriteBlob(string bufName, string fieldName, TType type)
        {
            return DbTsCompatibleSerialize(bufName, fieldName, type);
        }

        public static string DbTsCompatibleSerialize(string bufName, string fieldName, TType type)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return @$"{{let _state_ = {bufName}.BeginWriteSegment();{type.Apply(DbTypescriptCompatibleSerializeVisitor.Ins, bufName, fieldName)}; _buf.EndWriteSegment(_state_)}}";
            }
            else
            {
                return type.Apply(DbTypescriptCompatibleSerializeVisitor.Ins, bufName, fieldName);
            }
        }

        public static string DbTsCompatibleDeserialize(string bufName, string fieldName, TType type)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return $@"{{ let _state_ = {bufName}.EnterSegment(); {type.Apply(DbTypescriptCompatibleDeserializeVisitor.Ins, bufName, fieldName)} {bufName}.LeaveSegment(_state_); }}";
            }
            else
            {
                return type.Apply(DbTypescriptCompatibleDeserializeVisitor.Ins, bufName, fieldName);
            }
        }

        public static string DbTsCompatibleSerializeWithoutSegment(string bufName, string fieldName, TType type)
        {
            return type.Apply(DbTypescriptCompatibleSerializeVisitor.Ins, bufName, fieldName);
        }

        public static string DbTsCompatibleDeserializeWithoutSegment(string bufName, string fieldName, TType type)
        {
            return type.Apply(DbTypescriptCompatibleDeserializeVisitor.Ins, bufName, fieldName);
        }
    }
}
