using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.Defs
{
    class TTypeTemplateExtends : TTypeTemplateCommonExtends
    {

        public static string CsJsonDeserialize(string bufName, string fieldName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"{{ var _j = {bufName}.GetProperty(\"{jsonFieldName}\"); if (_j.ValueKind != JsonValueKind.Null) {{ {type.Apply(CsJsonUserialize.Ins, "_j", fieldName)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(CsJsonUserialize.Ins, $"{bufName}.GetProperty(\"{jsonFieldName}\")", fieldName);
            }
        }

        public static string CsRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(CsRecursiveResolveVisitor.Ins, field.CsStyleName, tables);
        }

        public static string CsRefValidatorResolve(DefField field)
        {
            var refVarName = field.CsRefVarName;
            var name = field.CsStyleName;
            var tableName = field.Ref.FirstTable;
            var table = field.Assembly.GetCfgTable(field.Ref.FirstTable);
            if (field.IsNullable)
            {
                return $"this.{refVarName} = this.{name} != null ? (_tables[\"{tableName}\"] as  {table.FullName}).GetOrDefault({name}.Value) : null;";
            }
            else
            {
                return $"this.{refVarName} = (_tables[\"{tableName}\"] as {table.FullName}).GetOrDefault({name});";
            }
        }

        public static string JavaDeserialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(JavaDeserializeVisitor.Ins, bufName, fieldName);
        }

        public static string JavaRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(JavaRecursiveResolveVisitor.Ins, field.JavaStyleName, tables);
        }

        public static string JavaRefValidatorResolve(DefField field)
        {
            var refVarName = field.JavaRefVarName;
            var name = field.JavaStyleName;
            var tableName = field.Ref.FirstTable;
            var table = field.Assembly.GetCfgTable(field.Ref.FirstTable);
            if (field.IsNullable)
            {
                return $"this.{refVarName} = this.{name} != null ? (({table.FullNameWithTopModule})_tables.get(\"{tableName}\")).get({name}.Value) : null;";
            }
            else
            {
                return $"this.{refVarName} = (({table.FullNameWithTopModule})_tables.get(\"{tableName}\")).get({name});";
            }
        }

        public static string CppDeserialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(CppDeserializeVisitor.Ins, bufName, fieldName);
        }


        public static string GoDefineType(TType type)
        {
            return type.Apply(GoTypeNameVisitor.Ins);
        }

        public static string GoDeserializeType(TType type, string bufName)
        {
            return type.Apply(GoDeserializeVisitor.Ins, bufName);
        }

        public static string GoDeserializeField(DefField field, string bufName)
        {
            var name = field.CsStyleName;
            TType type = field.CType;
            if (field.CType.Apply(NeedMarshalBoolPrefixVisitor.Ins))
            {
                return $"{{ var _exists bool; if _exists, err = {bufName}.ReadBool(); err != nil {{ return }}; if _exists {{ if _v.{name}, err = {type.Apply(GoDeserializeVisitor.Ins, bufName)}; err != nil  {{ return }} }} }}";
            }
            else
            {
                return $"if _v.{name}, err = {type.Apply(GoDeserializeVisitor.Ins, bufName)}; err != nil  {{ return }} ";
            }

        }

        public static string TsDeserialize(string fieldName, string jsonFieldName, TType type)
        {
            return type.Apply(TsDeserializeVisitor.Ins, $"{jsonFieldName}", fieldName);
        }

        public static string TsRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(TsRecursiveResolveVisitor.Ins, "this." + field.CsStyleName, tables);
        }

        public static string TsRefValidatorResolve(DefField field)
        {
            var refVarName = field.CsRefVarName;
            var name = "this." + field.TsStyleName;
            var tableName = field.Ref.FirstTable;
            var table = field.Assembly.GetCfgTable(field.Ref.FirstTable);
            if (field.IsNullable)
            {
                return $"this.{refVarName} = {name} != null ? (_tables.get('{tableName}') as  {table.FullName}).get({name}) : null;";
            }
            else
            {
                return $"this.{refVarName} = (_tables.get('{tableName}') as {table.FullName}).get({name});";
            }
        }

        public static string Py3Deserialize(string fieldName, string jsonFieldName, TType type)
        {
            return type.Apply(PyDeserializeVisitor.Py3Ins, $"{jsonFieldName}", fieldName);
        }

        public static string Py27Deserialize(string fieldName, string jsonFieldName, TType type)
        {
            return type.Apply(PyDeserializeVisitor.Py27Ins, $"{jsonFieldName}", fieldName);
        }
    }
}
