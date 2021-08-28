using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    class DTypeTemplateExtends : TTypeTemplateExtends
    {
        public static DType GetField(DBean bean, string fieldName)
        {
            int index = 0;
            foreach (var f in bean.ImplType.HierarchyExportFields)
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
            return type.Apply(ToJsonLiteralVisitor.Ins);
        }

        public static string ToLuaLiteral(DType type)
        {
            return type.Apply(ToLuaLiteralVisitor.Ins);
        }

        public static string ToXmlLiteral(DType type)
        {
            return type.Apply(ToXmlLiteralVisitor.Ins);
        }

        public static string ToPythonLiteral(DType type)
        {
            return type.Apply(ToPythonLiteralVisitor.Ins);
        }

        public static string ToErlangLiteral(DType type)
        {
            return type.Apply(ToErlangLiteralVisitor.Ins);
        }
    }
}
