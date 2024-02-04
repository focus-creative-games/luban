using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Java.TypeVisitors;

class JavaBinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static JavaBinUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readBool();";
    }

    public string Accept(TByte type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readByte();";
    }

    public string Accept(TShort type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readShort();";
    }

    public string Accept(TInt type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readInt();";
    }

    public string Accept(TLong type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readLong();";
    }

    public string Accept(TFloat type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readFloat();";
    }

    public string Accept(TDouble type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readDouble();";
    }

    public string Accept(TEnum type, string bufName, string fieldName)
    {
        string src = $"{bufName}.readInt()";
        string constructor = type.DefEnum.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TString type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readString();";
    }

    public string Accept(TBean type, string bufName, string fieldName)
    {
        string src = $"{type.DefBean.FullNameWithTopModule}.deserialize({bufName})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TArray type, string bufName, string fieldName)
    {
        return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)}[n];for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}[i] = _e;}}}}";
    }

    public string Accept(TList type, string bufName, string fieldName)
    {
        return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.add(_e);}}}}";
    }

    public string Accept(TSet type, string bufName, string fieldName)
    {
        return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.add(_e);}}}}";
    }

    public string Accept(TMap type, string bufName, string fieldName)
    {
        return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}.put(_k, _v);}}}}";

    }

    public string Accept(TDateTime type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.readLong();";
    }
}
