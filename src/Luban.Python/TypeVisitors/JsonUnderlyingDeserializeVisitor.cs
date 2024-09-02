using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Python.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Python.TypeVisitors;

public class JsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static JsonUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TByte type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TShort type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TInt type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TLong type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TFloat type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TDouble type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TEnum type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TString type, string jsonVarName, string fieldName, int depth)
    {
        return $"{StringUtil.RepeatSpaceAsTab(depth)}{fieldName} = {jsonVarName}";
    }

    public string Accept(TBean type, string jsonVarName, string fieldName, int depth)
    {
        string padding = StringUtil.RepeatSpaceAsTab(depth);
        if (type.DefBean.IsAbstractType)
        {
            return $"{padding}{fieldName} = {PythonCommonTemplateExtension.FullName(type.DefBean)}.fromJson({jsonVarName})";
        }
        else
        {
            return $"{padding}{fieldName} = {PythonCommonTemplateExtension.FullName(type.DefBean)}({jsonVarName})";
        }
    }

    public string Accept(TArray type, string jsonVarName, string fieldName, int depth)
    {
        string eleName = $"_ele{depth}_";
        string subVarName = $"_e{depth}_";
        string padding = StringUtil.RepeatSpaceAsTab(depth);
        string subPadding = StringUtil.RepeatSpaceAsTab(depth + 1);
        return $"{padding}{fieldName} = []\n{padding}for {eleName} in {jsonVarName}:\n{type.ElementType.Apply(this, eleName, subVarName, depth + 1)}\n{subPadding}{fieldName}.append({subVarName})";
    }

    public string Accept(TList type, string jsonVarName, string fieldName, int depth)
    {
        string eleName = $"_ele{depth}_";
        string subVarName = $"_e{depth}_";
        string padding = StringUtil.RepeatSpaceAsTab(depth);
        string subPadding = StringUtil.RepeatSpaceAsTab(depth + 1);
        return $"{padding}{fieldName} = []\n{padding}for {eleName} in {jsonVarName}:\n{type.ElementType.Apply(this, eleName, subVarName, depth + 1)}\n{subPadding}{fieldName}.append({subVarName})";
    }

    public string Accept(TSet type, string jsonVarName, string fieldName, int depth)
    {
        string eleName = $"_ele{depth}_";
        string subVarName = $"_e{depth}_";
        string padding = StringUtil.RepeatSpaceAsTab(depth);
        string subPadding = StringUtil.RepeatSpaceAsTab(depth + 1);
        return $"{padding}{fieldName} = []\n{padding}for {eleName} in {jsonVarName}:\n{type.ElementType.Apply(this, eleName, subVarName, depth + 1)}\n{subPadding}{fieldName}.append({subVarName})";
    }

    public string Accept(TMap type, string jsonVarName, string fieldName, int depth)
    {
        string keyEleName = $"_elek{depth}_";
        string valueEleName = $"_elev{depth}_";
        string keySubVarName = $"_k{depth}_";
        string valueSubVarName = $"_v{depth}_";
        string padding = StringUtil.RepeatSpaceAsTab(depth);
        string subPadding = StringUtil.RepeatSpaceAsTab(depth + 1);
        return $"{padding}{fieldName} = {{}};\n{padding}for {keyEleName}, {valueEleName} in {jsonVarName}:\n{type.KeyType.Apply(this, keyEleName, keySubVarName, depth + 1)}\n{type.ValueType.Apply(this, valueEleName, valueSubVarName, depth + 1)}\n{subPadding}{fieldName}[{keySubVarName}] = {valueSubVarName}";
    }

    public string Accept(TDateTime type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }
}
