using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
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
            return type.Apply(NeedMarshalBoolPrefixVisitor.Ins);
        }

        public static bool CsNeedInit(TType type)
        {
            return type.Apply(CsNeedInitVisitor.Ins);
        }

        public static string CsDefineType(TType type)
        {
            return type.Apply(CsDefineTypeName.Ins);
        }

        public static string CsToString(string filedName, TType type)
        {
            return $"{filedName}";
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

        public static string JavaToString(string filedName, TType type)
        {
            return $"{filedName}";
        }

        public static string JavaConstValue(TType type, string value)
        {
            return type.Apply(CsConstValueVisitor.Ins, value);
        }

        public static string CppDefineType(TType type)
        {
            return type.Apply(CppDefineTypeName.Ins);
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
            return type.Apply(TsDefineTypeName.Ins);
        }

        public static string TsToString(string filedName, TType type)
        {
            return $"{filedName}";
        }

        public static string TsConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }

        public static string PyDefineType(TType type)
        {
            return type.Apply(PyDefineTypeName.Ins);
        }

        public static string PyConstValue(TType type, string value)
        {
            return type.Apply(LuaConstValueVisitor.Ins, value);
        }
    }
}
