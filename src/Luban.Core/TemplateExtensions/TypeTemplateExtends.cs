using Luban.Types;
using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class TypeTemplateExtends : ScriptObject
{
    public static bool NeedMarshalBoolPrefix(TType type)
    {
        return type.IsNullable;
    }
}