using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Rust.TypeVisitors;

public class RustDeclaringBoxTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static readonly RustDeclaringBoxTypeNameVisitor Ins = new();

    public override string DoAccept(TType type)
    {
        var origin = type.Apply(RustDeclaringTypeNameVisitor.Ins);
        return type.IsNullable ? $"Option<{origin}>" : origin;
    }
}