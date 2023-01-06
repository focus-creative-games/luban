using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using Luban.Job.Common.Utils;
using Scriban.Runtime;

namespace Luban.Job.Common.Defs
{
    public class TTypeTemplateCommonExtends : ScriptObject
    {
        public static string TagName(TType type)
        {
            return type.Apply(TagNameVisitor.Ins);
        }

        public static bool NeedMarshalBoolPrefix(TType type)
        {
            return type.IsNullable;
        }

        public static bool CsNeedInit(TType type)
        {
            return type.Apply(CsNeedInitVisitor.Ins);
        }

        public static string CsDefineType(TType type)
        {
            return type.Apply(CsDefineTypeName.Ins);
        }

        public static string CsToString(string fieldName, TType type)
        {
            return type.Apply(CsToStringVisitor.Ins, fieldName);
        }

        public static string CsConstValue(TType type, string value)
        {
            return type.Apply(CsConstValueVisitor.Ins, value);
        }

        public static string CsInitFieldCtorValue(string bufName, TType type)
        {
            return $"{bufName} = {type.Apply(CsCtorValueVisitor.Ins)};";
        }

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

        public static string JavaDefineType(TType type)
        {
            return type.Apply(JavaDefineTypeName.Ins);
        }

        public static string JavaBoxDefineType(TType type)
        {
            return type.Apply(JavaBoxDefineTypeName.Ins);
        }

        public static string JavaToString(string fieldName, TType type)
        {
            return $"{fieldName}";
        }

        public static string JavaConstValue(TType type, string value)
        {
            return type.Apply(CsConstValueVisitor.Ins, value);
        }

        public static string CppDefineType(TType type)
        {
            return type.Apply(CppDefineTypeName.Ins);
        }

        public static string GdscriptDefineType(TType type)
        {
            return type.Apply(GDScriptDefineTypeName.Ins);
        }

        public static string CppConstValue(TType type, string value)
        {
            return type.Apply(CsConstValueVisitor.Ins, value);
        }

        public static string LuaConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }

        public static string LuaCommentType(TType type)
        {
            return type.Apply(LuaCommentTypeVisitor.Ins);
        }

        public static string EmmyLuaCommentType(TType type)
        {
            return type.Apply(EmmyLuaCommentTypeVisitor.Ins);
        }

        public static string LuaSerializeWhileNil(string bufName, string fieldName, TType type)
        {
            if (type.IsNullable)
            {
                return $"if {fieldName} == nil then writeBool(false) elseif writeBool(true) {type.Apply(LuaUnderingSerializeVisitor.Ins, bufName, fieldName)} end";
            }
            else
            {
                return $"{type.Apply(LuaUnderingSerializeVisitor.Ins, bufName, type.Apply(LuaValueOrDefaultVisitor.Ins, fieldName))}";
            }
        }

        public static string LuaUnderingDeserialize(string bufName, TType type)
        {
            return type.Apply(LuaUnderingDeserializeVisitor.Ins, bufName);
        }

        public static string GoConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }

        public static string TsDefineType(TType type)
        {
            return type.Apply(TypescriptDefineTypeNameVisitor.Ins);
        }

        public static string TsToString(string fieldName, TType type)
        {
            return fieldName;
        }

        public static string TsCtorDefaultValue(TType type)
        {
            return type.Apply(TypescriptCtorValueVisitor.Ins);
        }

        public static string TsConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }

        public static string TsBinSerialize(string fieldName, string byteBufName, TType type)
        {
            return type.Apply(TypescriptBinSerializeVisitor.Ins, byteBufName, fieldName);
        }

        public static string TsBinDeserialize(string fieldName, string byteBufName, TType type)
        {
            return type.Apply(TypescriptBinDeserializeVisitor.Ins, byteBufName, fieldName);
        }

        public static string PyDefineType(TType type)
        {
            return type.Apply(PyDefineTypeName.Ins);
        }

        public static string PyConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }


        public static string RustClassName(TBean type)
        {
            return type.Bean.RustFullName;
        }

        public static string RustDefineType(TType type)
        {
            return type.Apply(RustTypeNameVisitor.Ins);
        }

        public static string RustConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }

        public static string ErlangDefineType(TType type)
        {
            return type.Apply(ErlangDefineTypeNameVisitor.Ins);
        }

        public static string GoDefineType(TType type)
        {
            return type.Apply(GoTypeNameVisitor.Ins);
        }

        public static string GoDeserializeType(TBean type, string bufName)
        {
            return $"Deserialize{type.Bean.GoFullName}({bufName})";
        }

        public static string GoSerializeField(TType type, string name, string bufName)
        {
            return type.Apply(GoSerializeBinVisitor.Ins, name, bufName);
        }

        public static string GoDeserializeField(TType type, string name, string bufName, string err)
        {
            return type.Apply(GoDeserializeBinVisitor.Ins, name, bufName, err);
        }

        public static string ProtobufDefineType(TType type)
        {
            return type.Apply(ProtobufTypeNameVisitor.Ins);
        }

        public static string ProtobufPreDecorator(TType type)
        {
            if (type.IsNullable)
            {
                return "optional";
            }
            else if (type.IsCollection)
            {
                if (type is TMap)
                {
                    return "";
                }
                else
                {
                    return "repeated";
                }
            }
            else
            {
                return "required";
            }
        }

        public static string ProtobufSuffixOptions(TType type)
        {
            if (type.IsCollection && !(type is TMap))
            {
                return $"[packed = {(type.ElementType.Apply(IsProtobufPackedType.Ins) ? "true" : "false")}]";
            }
            else
            {
                return "";
            }
        }

        public static string Protobuf3PreDecorator(TType type)
        {
            if (type.IsNullable)
            {
                return "optional";
            }
            else if (type.IsCollection)
            {
                if (type is TMap)
                {
                    return "";
                }
                else
                {
                    return "repeated";
                }
            }
            else
            {
                return "";
            }
        }

        public static string FlatBuffersDefineType(TType type)
        {
            return type.Apply(FlatBuffersTypeNameVisitor.Ins);
        }

        public static string FlatBuffersTypeMetadata(TType type)
        {
            string metadata = type.IsNullable || type.Apply(IsFlatBuffersScalarTypeVisitor.Ins) ? "" : $"(required)";
            return metadata;
        }

        public static bool HasTag(dynamic obj, string attrName)
        {
            return obj.HasTag(attrName);
        }

        public static string GetTag(dynamic obj, string attrName)
        {
            return obj.GetTag(attrName);
        }

        public static bool HasOption(string optionName)
        {
            return DefAssemblyBase.LocalAssebmly.ContainsOption(optionName);
        }

        public static string GetOption(string optionName)
        {
            return DefAssemblyBase.LocalAssebmly.GetOption(optionName);
        }

        public static string GetOptionOr(string optionName, string defaultValue)
        {
            return DefAssemblyBase.LocalAssebmly.GetOptionOr(optionName, defaultValue);
        }

        public static bool GenDatetimeMills(TType type)
        {
            return type is TDateTime && !type.IsNullable && ExternalTypeUtil.GetExternalTypeMappfer("datetime") == null;
        }

        public static string CsStartNameSpaceGrace(string np)
        {
            if (string.IsNullOrEmpty(np))
            {
                return string.Empty;
            }
            else
            {
                return $"namespace {np}\n{{";
            }
        }
        public static string CsEndNameSpaceGrace(string np)
        {
            if (string.IsNullOrEmpty(np))
            {
                return string.Empty;
            }
            else
            {
                return "}";
            }
        }
    }
}
