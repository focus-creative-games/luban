using Luban.Defs;

namespace Luban.Utils;

public static class DefExtensions
{
    public static bool NeedExport(this DefField field)
    {
        return field.Assembly.NeedExport(field.Groups);
    }
    
    public static List<DefField> GetExportFields(this DefBean bean)
    {
        return bean.Fields.Where(f => f.NeedExport()).ToList();
    }
    
    public static List<DefField> GetHierarchyExportFields(this DefBean bean)
    {
        return bean.HierarchyFields.Where(f => f.NeedExport()).ToList();
    }
    
    public static bool NeedExport(this DefTable table)
    {
        return table.Assembly.NeedExport(table.Groups);
    }
}