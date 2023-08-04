using Luban.Core.Types;
using Scriban.Runtime;

namespace Luban.Core.TemplateExtensions;

public class TypeTemplateExtends : ScriptObject
{
    public static bool NeedMarshalBoolPrefix(TType type)
    {
        return type.IsNullable;
    }
}