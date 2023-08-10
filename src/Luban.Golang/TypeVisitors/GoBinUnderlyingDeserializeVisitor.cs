using Luban.Golang.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class GoBinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string, string>
{
    public static GoBinUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadBool(); {err} != nil {{ {err} = errors.New(\"error\"); {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TByte type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadByte(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TShort type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadShort(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TInt type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadInt(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TLong type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadLong(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TFloat type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadFloat(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TDouble type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadDouble(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TEnum type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadInt(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TString type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadString(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TDateTime type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadLong(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TBean type, string fieldName, string bufName, string err)
    {
        return $"{{ if {fieldName}, {err} = {($"New{GoCommonTemplateExtension.FullName(type.DefBean)}({bufName})")}; {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    private string GenList(TType elementType, string fieldName, string bufName, string err)
    {
        return $@"{{{fieldName} = make([]{elementType.Apply(GoDeclaringTypeNameVisitor.Ins)}, 0); var _n_ int; if _n_, {err} = {bufName}.ReadSize(); {err} != nil {{ {err} = errors.New(""error""); return}}; for i := 0 ; i < _n_ ; i++ {{ var _e_ {elementType.Apply(GoDeclaringTypeNameVisitor.Ins)}; {elementType.Apply(GoDeserializeBinVisitor.Ins, "_e_", bufName, err)}; {fieldName} = append({fieldName}, _e_) }} }}";
    }

    public string Accept(TArray type, string fieldName, string bufName, string err)
    {
        return GenList(type.ElementType, fieldName, bufName, err);
    }

    public string Accept(TList type, string fieldName, string bufName, string err)
    {
        return GenList(type.ElementType, fieldName, bufName, err);
    }

    public string Accept(TSet type, string fieldName, string bufName, string err)
    {
        return GenList(type.ElementType, fieldName, bufName, err);
    }

    public string Accept(TMap type, string fieldName, string bufName, string err)
    {
        return $@"{{ {fieldName} = make({type.Apply(GoDeclaringTypeNameVisitor.Ins)}); var _n_ int; if _n_, {err} = {bufName}.ReadSize(); {err} != nil {{ {err} = errors.New(""error""); return}}; for i := 0 ; i < _n_ ; i++ {{ var _key_ {type.KeyType.Apply(GoDeclaringTypeNameVisitor.Ins)}; {type.KeyType.Apply(GoDeserializeBinVisitor.Ins, "_key_", bufName, err)}; var _value_ {type.ValueType.Apply(GoDeclaringTypeNameVisitor.Ins)}; {type.ValueType.Apply(GoDeserializeBinVisitor.Ins, "_value_", bufName, err)}; {fieldName}[_key_] = _value_}} }}";
    }
}