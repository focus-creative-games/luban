using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using Scriban.Runtime;

namespace Luban.Job.Common.Defs
{
    public class TTypeTemplateCommonExtends : ScriptObject
    {
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
