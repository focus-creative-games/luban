using Luban.Defs;
using Luban.Golang.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Golang.TemplateExtensions;

public class GoBinTemplateExtension : ScriptObject
{
    // public static string Deserialize(string bufName, string fieldName, TType type)
    // {
    //     return type.Apply(GoDeserializeBinVisitor.Ins, bufName, fieldName);
    // }

    public static string DeserializeField(TType type, string name, string bufName, string err)
    {
        return type.Apply(DeserializeBinVisitor.Ins, name, bufName, err, 0);
    }

    public static string CollectImport(DefBean bean)
    {
        var imports = new HashSet<string>();
        if (bean.IsAbstractType || bean.HierarchyExportFields.Count > 0)
        {
            imports.Add("errors");
        }
        return string.Join('\n', imports.Select(im => $"import \"{im}\""));
    }
}
