using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    class DTypeTemplateExtends : TTypeTemplateExtends
    {
        public static bool IsSimpleLiteralData(DType type)
        {
            return type.Apply(IsSimpleLiteralDataVisitor.Ins);
        }

        public static string ToLocalizedText(DText type)
        {
            var ass = DefAssembly.LocalAssebmly;
            return type.GetText(ass.ExportTextTable, ass.NotConvertTextSet);
        }

        public static DType GetField(DBean bean, string fieldName)
        {
            int index = 0;
            foreach (var f in bean.ImplType.HierarchyFields)
            {
                if (f.Name == fieldName)
                {
                    return bean.Fields[index];
                }
                ++index;
            }
            return null;
        }

        public static string ToJsonPropertyName(DType type)
        {
            return "\"" + type.Apply(ToJsonPropertyNameVisitor.Ins) + "\"";
        }

        public static string ToJsonLiteral(DType type)
        {
            return type != null ? type.Apply(ToJsonLiteralVisitor.Ins) : "null";
        }

        public static string ToLuaLiteral(DType type)
        {
            return type != null ? type.Apply(ToLuaLiteralVisitor.Ins) : "nil";
        }

        public static string ToXmlLiteral(DType type)
        {
            return type.Apply(ToXmlLiteralVisitor.Ins);
        }

        public static string ToPythonLiteral(DType type)
        {
            return type != null ? type.Apply(ToPythonLiteralVisitor.Ins) : "None";
        }

        public static string ToErlangLiteral(DType type)
        {
            return type.Apply(ToErlangLiteralVisitor.Ins);
        }
    }
}
