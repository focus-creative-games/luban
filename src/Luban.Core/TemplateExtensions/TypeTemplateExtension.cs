using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class TypeTemplateExtension : ScriptObject
{
    public static bool NeedMarshalBoolPrefix(TType type)
    {
        return type.IsNullable;
    }

    public static string FormatMethodName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatMethod(name);
    }

    public static string FormatFieldName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatField(name);
    }

    public static string FormatPropertyName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatProperty(name);
    }

    public static string FormatEnumItemName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatEnumItemName(name);
    }

    public static bool CanGenerateRef(DefField field)
    {
        if (field.CType.IsCollection)
        {
            return false;
        }

        return GetRefTable(field) != null;
    }

    public static bool CanGenerateCollectionRef(DefField field)
    {
        if (!field.CType.IsCollection)
        {
            return false;
        }
        return GetCollectionRefTable(field) != null;
    }

    public static bool CanGenerateRefGroup(DefField field)
    {
        if (field.CType.IsCollection)
        {
            return false;
        }
        var refTables = GetRefTablesFromRefGroup(field);
        return refTables != null && refTables.Count > 0;
    }

    public static bool CanGenerateCollectionRefGroup(DefField field)
    {
        if (!field.CType.IsCollection)
        {
            return false;
        }
        var refTables = GetCollectionRefTablesFromRefGroup(field);
        return refTables != null && refTables.Count > 0;
    }

    public static DefTable GetRefTable(DefField field)
    {
        if (field.CType.GetTag("ref") is { } value && GenerationContext.Current.Assembly.GetCfgTable(value.Replace("?", "")) is { } cfgTable)
        {
            return cfgTable;
        }
        return null;
    }

    public static DefTable GetCollectionRefTable(DefField field)
    {
        var refTag = field.CType.GetTag("ref");
        if (refTag == null)
        {
            refTag = field.CType.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return null;
        }
        if (GenerationContext.Current.Assembly.GetCfgTable(refTag.Replace("?", "")) is { } cfgTable)
        {
            return cfgTable;
        }
        return null;
    }

    public static TType GetRefType(DefField field)
    {
        return GetRefTable(field)?.ValueTType;
    }

    public static List<DefTable> DoGetRefTablesFromRefGroup(string refTag)
    {
        var refGroup = GenerationContext.Current.Assembly.GetRefGroup(refTag.Replace("?", ""));
        if (refGroup == null)
        {
            return null;
        }
        var refTables = new List<DefTable>();
        foreach (var refTableName in refGroup.Refs)
        {
            var tableName = refTableName;
            if (refTableName.Contains("@"))
            {
                tableName = refTableName.Split('@')[1];
            }
            var refTable = GenerationContext.Current.Assembly.GetCfgTable(tableName);
            if (refTable == null)
            {
                throw new Exception($"refgroup:'{refTag}' ref:'{tableName}' 不存在");
            }
            if (refTable.Mode != TableMode.MAP)
            {
                continue;
            }
            refTables.Add(refTable);
        }
        return refTables;
    }

    public static List<DefTable> GetRefTablesFromRefGroup(DefField field)
    {
        var refTag = field.CType.GetTag("ref");
        if (refTag == null)
        {
            return null;
        }
        return DoGetRefTablesFromRefGroup(refTag);
    }

    public static bool HasRefGroup(DefField field)
    {
        return CanGenerateCollectionRefGroup(field) || CanGenerateRefGroup(field);
    }

    public static int GetRefGroupTablesCount(DefField field)
    {
        if (CanGenerateCollectionRefGroup(field))
        {
            return GetCollectionRefTablesFromRefGroup(field).Count;
        }
        if (CanGenerateRefGroup(field))
        {
            return GetRefTablesFromRefGroup(field).Count;
        }
        return 0;
    }

    public static List<DefTable> GetCollectionRefTablesFromRefGroup(DefField field)
    {
        var refTag = field.CType.GetTag("ref");
        if (refTag == null)
        {
            refTag = field.CType.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return null;
        }
        return DoGetRefTablesFromRefGroup(refTag);
    }

    public static bool IsFieldBeanNeedResolveRef(DefField field)
    {
        return field.CType is TBean bean && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool IsFieldArrayLikeNeedResolveRef(DefField field)
    {
        return field.CType.ElementType is TBean bean && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType && field.CType is not TMap;
    }

    public static bool IsFieldMapNeedResolveRef(DefField field)
    {
        return field.CType is TMap { ValueType: TBean bean } && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool HasIndex(DefField field)
    {
        TType type = field.CType;
        return type.HasTag("index") && type is TArray or TList;
    }

    public static string GetIndexName(DefField field)
    {
        return field.CType.GetTag("index");
    }

    public static DefField GetIndexField(DefField field)
    {
        string indexName = GetIndexName(field);
        return ((TBean)field.CType.ElementType).DefBean.GetField(indexName);
    }

    public static TMap GetIndexMapType(DefField field)
    {
        DefField indexField = GetIndexField(field);
        return TMap.Create(false, null, indexField.CType, field.CType.ElementType, false);
    }

    public static string ImplDataType(DefBean type, DefBean parent)
    {
        return DataUtil.GetImplTypeName(type, parent);
    }

    public static string EscapeComment(string comment)
    {
        return System.Web.HttpUtility.HtmlEncode(comment).Replace("\n", "<br/>");
    }
}
