using Luban.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class CsharpEditorTemplateExtension : ScriptObject
{

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(EditorDeclaringTypeNameVisitor.Ins);
    }

    public static string Deserialize(string jsonName, string fieldName, TType type)
    {
        return $"{type.Apply(EditorJsonLoad.Ins, jsonName, fieldName)}";
    }

    public static string Serialize(string jsonName, string jsonFieldName, string fieldName, TType type)
    {
        return $"{type.Apply(EditorJsonSave.Ins, jsonName, jsonFieldName, fieldName)}";
    }

    public static bool IsRawNullable(TType type)
    {
        return type.Apply(IsRawNullableTypeVisitor.Ins);
    }

    public static bool NeedInit(TType type)
    {
        return type.Apply(EditorNeedInitVisitor.Ins);
    }

    public static string InitValue(TType type)
    {
        return type.Apply(EditorInitValueVisitor.Ins);
    }
}
