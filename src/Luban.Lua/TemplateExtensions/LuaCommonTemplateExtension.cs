using Luban.Defs;
using Luban.Lua.TypVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Lua.TemplateExtensions;

public class LuaCommonTemplateExtension : ScriptObject
{
    public static string CommentType(TType type)
    {
        return type.Apply(LuaCommentTypeVisitor.Ins);
    }
}
