using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

class EditorJsonSave : DecoratorFuncVisitor<string, string, string, int, string>
{
    public static EditorJsonSave Ins { get; } = new();

    public override string DoAccept(TType type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{type.Apply(EditorJsonSaveUnderlying.Ins, jsonName, jsonFieldName, (!type.IsNullable || type.Apply(EditorIsRawNullableTypeVisitor.Ins) ? value : $"{value}.Value"), depth)}";
    }
}
