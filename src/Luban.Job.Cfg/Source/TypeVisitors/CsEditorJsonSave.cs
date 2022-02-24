using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CsEditorJsonSave : ITypeFuncVisitor<string, string, string, string>
    {
        public static CsEditorJsonSave Ins { get; } = new();

        public string Accept(TBool type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONBool({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TByte type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TShort type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TFshort type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TInt type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TFint type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TLong type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TFlong type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TFloat type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TDouble type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
        }

        public string Accept(TEnum type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber((int){value});";
        }

        public string Accept(TString type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
        }

        public string Accept(TText type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = {CfgConstStrings.EditorTextTypeName}.SaveJson({value});";
        }

        public string Accept(TBytes type, string jsonName, string jsonFieldName, string value)
        {
            throw new NotSupportedException();
        }

        public string Accept(TVector2 type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __vjson = new JSONObject(); __vjson[\"x\"] = {value}{(type.IsNullable ? ".Value" : "")}.x;  __vjson[\"y\"] = {value}{(type.IsNullable ? ".Value" : "")}.y; {jsonName}[\"{jsonFieldName}\"] = __vjson; }}";
        }

        public string Accept(TVector3 type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __vjson = new JSONObject(); __vjson[\"x\"] = {value}{(type.IsNullable ? ".Value" : "")}.x;  __vjson[\"y\"] = {value}{(type.IsNullable ? ".Value" : "")}.y; __vjson[\"z\"] = {value}{(type.IsNullable ? ".Value" : "")}.z; {jsonName}[\"{jsonFieldName}\"] = __vjson; }}";
        }

        public string Accept(TVector4 type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __vjson = new JSONObject(); __vjson[\"x\"] = {value}{(type.IsNullable ? ".Value" : "")}.x;  __vjson[\"y\"] = {value}{(type.IsNullable ? ".Value" : "")}.y; __vjson[\"z\"] = {value}{(type.IsNullable ? ".Value" : "")}.z; __vjson[\"w\"] = {value}{(type.IsNullable ? ".Value" : "")}.w; {jsonName}[\"{jsonFieldName}\"] = __vjson; }}";
        }

        public string Accept(TDateTime type, string jsonName, string jsonFieldName, string value)
        {
            return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
        }

        public string Accept(TBean type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __bjson = new JSONObject();  {type.Apply(CsEditorUnderlyingDefineTypeName.Ins)}.SaveJson{type.Bean.Name}({value}, __bjson); {jsonName}[\"{jsonFieldName}\"] = __bjson; }}";
        }

        public string Accept(TArray type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        }

        public string Accept(TList type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        }

        public string Accept(TSet type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        }

        public string Accept(TMap type, string jsonName, string jsonFieldName, string value)
        {
            return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ var __entry = new JSONArray(); __cjson[null] = __entry; {type.KeyType.Apply(this, "__entry", "null", "_e.Key")} {type.ValueType.Apply(this, "__entry", "null", "_e.Value")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        }
    }
}
