using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Utils
{
    class DTypeTemplateExtends : TTypeTemplateExtends
    {

        public static bool IsSimpleLiteralData(DType type)
        {
            return type.Apply(IsSimpleLiteralDataVisitor.Ins);
        }

        public static bool IsSimpleLiteralData2(DType type)
        {
            return type.Apply(IsSimpleLiteralDataVisitor2.Ins);
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

        public static List<DBean> SortDataList(List<DBean> datas, string indexName, bool asce)
        {
            var sortedDatas = new List<DBean>(datas);
            if (sortedDatas.Count > 1)
            {
                if (asce)
                {
                    sortedDatas.Sort((a, b) => a.GetField(indexName).CompareTo(b.GetField(indexName)));
                }
                else
                {
                    sortedDatas.Sort((a, b) => -a.GetField(indexName).CompareTo(b.GetField(indexName)));
                }
            }
            return sortedDatas;
        }
    }
}
