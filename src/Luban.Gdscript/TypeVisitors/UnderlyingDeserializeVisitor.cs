using Luban.DataExporter.Builtin.Json;
using Luban.Gdscript.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Gdscript.TypeVisitors;

public class UnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static UnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TByte type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TShort type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TInt type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TLong type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TFloat type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TDouble type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TEnum type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TString type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TBean type, string jsonVarName, string fieldName)
    {
        if (type.DefBean.IsAbstractType)
        {
            return $"{fieldName} = {GdscriptCommonTemplateExtension.FullName(type.DefBean)}.fromJson({jsonVarName})";
        }
        else
        {
            return $"{fieldName} = {GdscriptCommonTemplateExtension.FullName(type.DefBean)}.new({jsonVarName})";
        }
    }

    public string Accept(TArray type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = []\nfor _ele in {jsonVarName}: var _e: {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.append(_e)";
    }

    public string Accept(TList type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = []\nfor _ele in {jsonVarName}: var _e: {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.append(_e)";
    }

    public string Accept(TSet type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = []\nfor _ele in {jsonVarName}: var _e: {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.append(_e)";
    }

    public string Accept(TMap type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {{}}\nfor _e in {jsonVarName}: var _k: {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, "_e[0]", "_k")}; var _v: {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, "_e[1]", "_v")}; {fieldName}[_k] = _v";
    }

    public string Accept(TDateTime type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

}
