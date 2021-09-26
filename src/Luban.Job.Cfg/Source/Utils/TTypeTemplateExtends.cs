using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.Utils
{
    class TTypeTemplateExtends : TTypeTemplateCommonExtends
    {
        public static string CsDefineTextKeyField(DefField field)
        {
            return $"string {field.GetTextKeyName(field.CsStyleName)}";
        }

        public static string CsTranslateText(DefField field, string translatorName)
        {
            return $"{field.CsStyleName} = {translatorName}({field.GetTextKeyName(field.CsStyleName)}, {field.CsStyleName});";
        }

        public static string CsRecursiveTranslateText(DefField field, string translatorName)
        {
            return field.CType.Apply(CsRecursiveTranslateVisitor.Ins, field.CsStyleName, translatorName);
        }

        public static string CsJsonDeserialize(string bufName, string fieldName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"{{ if ({bufName}.TryGetProperty(\"{jsonFieldName}\", out var _j) && _j.ValueKind != JsonValueKind.Null) {{ {type.Apply(TypeVisitors.CsJsonDeserialize.Ins, "_j", fieldName)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(TypeVisitors.CsJsonDeserialize.Ins, $"{bufName}.GetProperty(\"{jsonFieldName}\")", fieldName);
            }
        }

        public static string CsUnityJsonDeserialize(string bufName, string fieldName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"{{ var _j = {bufName}[\"{jsonFieldName}\"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) {{ {type.Apply(TypeVisitors.CsUnityJsonDeserialize.Ins, "_j", fieldName)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(TypeVisitors.CsUnityJsonDeserialize.Ins, $"{bufName}[\"{jsonFieldName}\"]", fieldName);
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

        public static string JavaJsonDeserialize(string jsonName, string fieldName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"{{ if ({jsonName}.has(\"{jsonFieldName}\") && !{jsonName}.get(\"{jsonFieldName}\").isJsonNull()) {{ {type.Apply(TypeVisitors.JavaJsonDeserialize.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(TypeVisitors.JavaJsonDeserialize.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName);
            }
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
                return $"this.{refVarName} = this.{name} != null ? (({table.FullNameWithTopModule})_tables.get(\"{tableName}\")).get({name}) : null;";
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

        public static string CppRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(CppRecursiveResolveVisitor.Ins, field.CsStyleName, tables);
        }

        public static string CppRefValidatorResolve(DefField field)
        {
            var refVarName = field.CppRefVarName;
            var name = field.CsStyleName;
            var tableName = field.Ref.FirstTable;
            var table = field.Assembly.GetCfgTable(field.Ref.FirstTable);
            if (field.IsNullable)
            {
                return $"this->{refVarName} = this->{name} != nullptr ? (({table.CppFullName}*)(_tables[\"{tableName}\"]))->get(*(this->{name})) : nullptr;";
            }
            else
            {
                return $"this->{refVarName} = (({table.CppFullName}*)(_tables[\"{tableName}\"]))->get({name});";
            }
        }

        public static string GoDefineType(TType type)
        {
            return type.Apply(GoTypeNameVisitor.Ins);
        }

        public static string GoDeserializeType(TBean type, string bufName)
        {
            return $"New{type.Bean.GoFullName}({bufName})";
        }

        public static string GoDeserializeField(TType type, string name, string bufName)
        {
            return type.Apply(GoDeserializeBinVisitor.Ins, name, bufName);
        }

        public static string GoDeserializeJsonField(TType type, string name, string fieldName, string bufName)
        {
            return type.Apply(GoDeserializeJsonVisitor.Ins, name, fieldName, bufName);
        }

        public static string TsJsonConstructor(string fieldName, string jsonFieldName, TType type)
        {
            return type.Apply(TypescriptJsonConstructorVisitor.Ins, jsonFieldName, fieldName);
        }

        public static string TsBinConstructor(string fieldName, string byteBufName, TType type)
        {
            return type.Apply(TypescriptBinConstructorVisitor.Ins, byteBufName, fieldName);
        }

        public static string TsRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(TypescriptRecursiveResolveVisitor.Ins, "this." + field.CsStyleName, tables);
        }

        public static string TsRefValidatorResolve(DefField field)
        {
            var refVarName = field.CsRefVarName;
            var name = "this." + field.TsStyleName;
            var tableName = field.Ref.FirstTable;
            var table = field.Assembly.GetCfgTable(field.Ref.FirstTable);
            if (field.IsNullable)
            {
                return $"this.{refVarName} = {name} != undefined ? (_tables.get('{tableName}') as  {table.FullName}).get({name}) : undefined";
            }
            else
            {
                return $"this.{refVarName} = (_tables.get('{tableName}') as {table.FullName}).get({name})!";
            }
        }

        public static string Py3DeserializeValue(string fieldName, string jsonVarName, TType type)
        {
            if (type.IsNullable)
            {
                return $"if {jsonVarName} != None: {type.Apply(PyUnderingDeserializeVisitor.Ins, jsonVarName, fieldName)}";
            }
            else
            {
                return type.Apply(PyUnderingDeserializeVisitor.Ins, jsonVarName, fieldName);
            }
        }

        public static string Py3DeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"if {jsonVarName}.get('{jsonFieldName}') != None: {type.Apply(PyUnderingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName)}";
            }
            else
            {
                return type.Apply(PyUnderingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName);
            }
        }

        public static string DefineTextKeyField(DefField field, string lan)
        {
            switch (lan)
            {
                case "cs": return $"string {field.CsStyleName}{TText.L10N_FIELD_SUFFIX};";
                default: throw new NotSupportedException($"not support lan:{lan}");
            }
        }

        //public static string DeserializeTextKeyField(DefField field, string lan, string bufName)
        //{
        //    switch (lan)
        //    {
        //        case "cpp": return $"{CppDefineTypeName.Ins.Accept(TString.Ins)} {field.CppStyleName}{L10N_FIELD_SUFFIX};";
        //        default: throw new NotSupportedException($"not support lan:{lan}");
        //    }
        //}
    }
}
