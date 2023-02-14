using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Linq;
using JavaDeserializeVisitor = Luban.Job.Cfg.TypeVisitors.JavaDeserializeVisitor;

namespace Luban.Job.Cfg.Utils
{
    class TTypeTemplateExtends : TTypeTemplateCommonExtends
    {
        public static string CsDefineTextKeyField(DefField field)
        {
            return $"string {field.GetTextKeyName(field.ConventionName)}";
        }

        public static string CsTranslateText(DefField field, string translatorName)
        {
            return $"{field.ConventionName} = {translatorName}({field.GetTextKeyName(field.ConventionName)}, {field.ConventionName});";
        }

        public static string CsRecursiveTranslateText(DefField field, string translatorName)
        {
            return field.CType.Apply(TypescriptRecursiveTranslateVisitor.Ins, field.ConventionName, translatorName);
        }

        public static string CsJsonDeserialize(string bufName, string fieldName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"{{ if ({bufName}.TryGetProperty(\"{jsonFieldName}\", out var _j) && _j.ValueKind != JsonValueKind.Null) {{ {type.Apply(TypeVisitors.CsJsonDeserialize.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(TypeVisitors.CsJsonDeserialize.Ins, $"{bufName}.GetProperty(\"{jsonFieldName}\")", fieldName, 0);
            }
        }

        public static string CsImplDataType(DefBean type, DefBean parent)
        {
            return DataUtil.GetImplTypeName(type, parent);
        }

        public static string CsUnityJsonDeserialize(string bufName, string fieldName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"{{ var _j = {bufName}[\"{jsonFieldName}\"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) {{ {type.Apply(TypeVisitors.CsUnityJsonDeserialize.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(TypeVisitors.CsUnityJsonDeserialize.Ins, $"{bufName}[\"{jsonFieldName}\"]", fieldName, 0);
            }
        }

        public static string CsRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(CsRecursiveResolveVisitor.Ins, field.ConventionName, tables);
        }

        public static string CsRefValidatorResolve(DefField field)
        {
            var refVarName = field.RefVarName;
            var name = field.ConventionName;

            if (field.Ref != null)
            {
                var tableName = field.Ref.FirstTable;
                var table = field.Assembly.GetCfgTable(tableName);
                if (field.IsNullable)
                {
                    return $"this.{refVarName} = this.{name} != null ? (_tables[\"{tableName}\"] as  {table.FullName}).GetOrDefault({name}.Value) : null;";
                }
                else
                {
                    return $"this.{refVarName} = (_tables[\"{tableName}\"] as {table.FullName}).GetOrDefault({name});";
                }
            }
            else
            {
                var tableName = field.ElementRef.FirstTable;
                var table = field.Assembly.GetCfgTable(tableName);
                switch (field.CType)
                {
                    case TArray:
                    {
                        return $@"{{ int __n = {name}.Length; {table.FullName} __table = ({table.FullName})_tables[""{tableName}""]; this.{refVarName} = new {table.ValueTType.Apply(CsDefineTypeName.Ins)}[__n]; for(int i = 0 ; i < __n ; i++) {{ this.{refVarName}[i] =  __table.GetOrDefault({name}[i]); }} }}";
                    }
                    case TList:
                    case TSet:
                    {
                        return $@"{{ {table.FullName} __table = ({table.FullName})_tables[""{tableName}""]; this.{refVarName} = new {field.ElementRefType.Apply(CsDefineTypeName.Ins)}(); foreach(var __e in {name}) {{ this.{refVarName}.Add(__table.GetOrDefault(__e)); }} }}";
                    }
                    case TMap:
                    {
                        return $@"{{ {table.FullName} __table = ({table.FullName})_tables[""{tableName}""]; this.{refVarName} = new {field.ElementRefType.Apply(CsDefineTypeName.Ins)}(); foreach(var __e in {name}) {{ this.{refVarName}.Add(__e.Key, __table.GetOrDefault(__e.Value)); }} }}";
                    }
                    default: throw new NotSupportedException($"type:'{field.CType.TypeName}' not support ref");
                }
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
                return $"{{ if ({jsonName}.has(\"{jsonFieldName}\") && !{jsonName}.get(\"{jsonFieldName}\").isJsonNull()) {{ {type.Apply(TypeVisitors.JavaJsonDeserialize.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
            }
            else
            {
                return type.Apply(TypeVisitors.JavaJsonDeserialize.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName, 0);
            }
        }

        public static string JavaRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(JavaRecursiveResolveVisitor.Ins, field.ConventionName, tables);
        }

        public static string JavaRefValidatorResolve(DefField field)
        {
            var refVarName = field.RefVarName;
            var name = field.ConventionName;

            if (field.Ref != null)
            {
                var tableName = field.Ref.FirstTable;
                var table = field.Assembly.GetCfgTable(tableName);
                if (field.IsNullable)
                {
                    return $"this.{refVarName} = this.{name} != null ? (({table.FullNameWithTopModule})_tables.get(\"{tableName}\")).get({name}) : null;";
                }
                else
                {
                    return $"this.{refVarName} = (({table.FullNameWithTopModule})_tables.get(\"{tableName}\")).get({name});";
                }
            }
            else
            {
                var tableName = field.ElementRef.FirstTable;
                var table = field.Assembly.GetCfgTable(tableName);
                switch (field.CType)
                {
                    case TArray:
                    {
                        return $@"{{ int __n = {name}.length; {table.FullNameWithTopModule} __table = ({table.FullNameWithTopModule})_tables.get(""{tableName}""); this.{refVarName} = new {table.ValueTType.Apply(JavaDefineTypeName.Ins)}[__n]; for(int i = 0 ; i < __n ; i++) {{ this.{refVarName}[i] =  __table.get({name}[i]); }} }}";
                    }
                    case TList:
                    case TSet:
                    {
                        return $@"{{ {table.FullNameWithTopModule} __table = ({table.FullNameWithTopModule})_tables.get(""{tableName}""); this.{refVarName} = new {field.ElementRefType.Apply(JavaDefineTypeName.Ins)}(); for({field.CType.ElementType.TypeName} __e : {name}) {{ this.{refVarName}.add(__table.get(__e)); }} }}";
                    }
                    case TMap map:
                    {
                        return $@"{{ {table.FullNameWithTopModule} __table = ({table.FullNameWithTopModule})_tables.get(""{tableName}""); this.{refVarName} = new {field.ElementRefType.Apply(JavaDefineTypeName.Ins)}(); for(java.util.Map.Entry<{map.KeyType.Apply(JavaBoxDefineTypeName.Ins)}, {map.ValueType.Apply(JavaBoxDefineTypeName.Ins)}> __e : {name}.entrySet()) {{ {map.KeyType.TypeName} __eKey = __e.getKey(); {map.ValueType.TypeName} __eValue = __e.getValue(); this.{refVarName}.put(__eKey, __table.get(__eValue)); }} }}";
                    }
                    default: throw new NotSupportedException($"type:'{field.CType.TypeName}' not support ref");
                }
            }
        }

        public static string CppDeserialize(string bufName, string fieldName, TType type)
        {
            return type.Apply(CppDeserializeVisitor.Ins, bufName, fieldName);
        }

        public static string CppRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(CppRecursiveResolveVisitor.Ins, field.ConventionName, tables);
        }

        public static string CppRefValidatorResolve(DefField field)
        {
            var refVarName = field.RefVarName;
            var name = field.ConventionName;
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

        public static string TsDefineTextKeyField(DefField field)
        {
            return $"{field.GetTextKeyName(field.ConventionName)}";
        }

        public static string TsTranslateText(DefField field, string translatorName)
        {
            return $"this.{field.ConventionName} = {translatorName}(this.{field.GetTextKeyName(field.ConventionName)}, this.{field.ConventionName});";
        }

        public static string TsRecursiveTranslateText(DefField field, string translatorName)
        {
            return field.CType.Apply(TypescriptRecursiveTranslateVisitor.Ins, field.ConventionName, translatorName);
        }

        public static string TsRecursiveResolve(DefField field, string tables)
        {
            return field.CType.Apply(TypescriptRecursiveResolveVisitor.Ins, "this." + field.ConventionName, tables);
        }

        public static string TsRefValidatorResolve(DefField field)
        {
            var refVarName = field.RefVarName;
            var name = "this." + field.ConventionName;
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

        public static string GdscriptDeserializeValue(string fieldName, string jsonVarName, TType type)
        {
            if (type.IsNullable)
            {
                return $"if {jsonVarName} != None: {type.Apply(GDScriptUnderingDeserializeVisitor.Ins, jsonVarName, fieldName)}";
            }
            else
            {
                return type.Apply(GDScriptUnderingDeserializeVisitor.Ins, jsonVarName, fieldName);
            }
        }

        public static string GdscriptDeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"if {jsonVarName}.get('{jsonFieldName}') != null: {type.Apply(GDScriptUnderingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName)}";
            }
            else
            {
                return type.Apply(GDScriptUnderingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName);
            }
        }

        public static string DefineTextKeyField(DefField field, string lan)
        {
            switch (lan)
            {
                case "cs": return $"string {field.ConventionName}{TText.L10N_FIELD_SUFFIX};";
                default: throw new NotSupportedException($"not support lan:{lan}");
            }
        }

        public static string RustJsonConstructor(string jsonFieldName, TType type)
        {
            return type.Apply(RustJsonConstructorVisitor.Ins, jsonFieldName);
        }

        public static string CsTableUnionMapTypeName(DefTable table)
        {
            return $"Dictionary<({string.Join(", ", table.IndexList.Select(idx => CsDefineType(idx.Type)))}), {CsDefineType(table.ValueTType)}>";
        }

        public static string CsTableKeyList(DefTable table, string varName)
        {
            return string.Join(", ", table.IndexList.Select(idx => $"{varName}.{idx.IndexField.ConventionName}"));
        }

        public static string CsTableGetParamDefList(DefTable table)
        {
            return string.Join(", ", table.IndexList.Select(idx => $"{CsDefineType(idx.Type)} {idx.IndexField.Name}"));
        }

        public static string CsTableGetParamNameList(DefTable table)
        {
            return string.Join(", ", table.IndexList.Select(idx => $"{idx.IndexField.Name}"));
        }

        public static string CsEditorDefineType(TType type)
        {
            return type.Apply(CsEditorDefineTypeName.Ins);
        }

        public static string CsUnityEditorJsonLoad(string jsonName, string fieldName, TType type)
        {
            return $"{type.Apply(CsEditorJsonLoad.Ins, jsonName, fieldName)}";
        }

        public static string CsUnityEditorJsonSave(string jsonName, string jsonFieldName, string fieldName, TType type)
        {
            return $"{type.Apply(CsEditorJsonSave.Ins, jsonName, jsonFieldName, fieldName)}";
        }

        public static bool CsIsEditorRawNullable(TType type)
        {
            return type.Apply(CsIsRawNullableTypeVisitor.Ins);
        }

        public static bool CsEditorNeedInit(TType type)
        {
            return type.Apply(CsEditorNeedInitVisitor.Ins);
        }

        public static string CsEditorInitValue(TType type)
        {
            return type.Apply(CsEditorInitValueVisitor.Ins);
        }
    }
}
