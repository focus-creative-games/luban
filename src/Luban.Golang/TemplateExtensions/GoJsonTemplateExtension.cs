using Luban.Defs;
using Luban.Golang.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Golang.TemplateExtensions;

public class GoJsonTemplateExtension : ScriptObject
{
    // public static string Deserialize(string fieldName, string jsonVar, TType type)
    // {
    //     return type.Apply(JavaJsonUnderlyingDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    // }

    public static string DeserializeField(TType type, string varName, string fieldName, string bufName)
    {
        return type.Apply(DeserializeJsonFieldVisitor.Ins, varName, fieldName, bufName);
    }

    public static string CollectImport(DefBean bean)
    {
        var imports = new HashSet<string>();
        if (bean.IsAbstractType || bean.HierarchyExportFields.Count > 0)
        {
            imports.Add("errors");
        }

        foreach (var f in bean.HierarchyExportFields)
        {
            f.CType.Apply(TypeVisitors.JsonImport.Ins, imports);
        }
        return string.Join('\n', imports.Select(im => $"import \"{im}\""));
    }
}
