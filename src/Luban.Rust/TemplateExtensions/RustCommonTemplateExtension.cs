using Luban.Defs;
using Luban.Rust.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Rust.TemplateExtensions;

public class RustCommonTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type?.Apply(RustDeclaringBoxTypeNameVisitor.Ins) ?? string.Empty;
    }

    public static string GetterName(string name)
    {
        return "get_" + name;
    }

    public static string FullName(DefTypeBase type)
    {
        return $"crate::{type.FullName.Replace(".", "::")}";
    }
}