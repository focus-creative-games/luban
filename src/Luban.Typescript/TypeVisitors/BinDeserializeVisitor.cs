using Luban.Types;
using Luban.Typescript.TypeVisitors;
using Luban.TypeVisitors;

namespace Luban.Typescript.TemplateExtensions;

class BinDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static BinDeserializeVisitor Ins { get; } = new BinDeserializeVisitor();

    public override string DoAccept(TType type, string byteBufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({byteBufName}.readBool()) {{ {type.Apply(BinUnderingDeserializeVisitor.Ins, byteBufName, fieldName, 0)} }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(BinUnderingDeserializeVisitor.Ins, byteBufName, fieldName, 0);
        }
    }
}
