using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Java.TypeVisitors;

class JavaBinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static JavaBinUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readBool();";
    }

    public string Accept(TByte type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readByte();";
    }

    public string Accept(TShort type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readShort();";
    }

    public string Accept(TInt type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readInt();";
    }

    public string Accept(TLong type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readLong();";
    }

    public string Accept(TFloat type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readFloat();";
    }

    public string Accept(TDouble type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readDouble();";
    }

    public string Accept(TEnum type, string bufName, string fieldName, int depth)
    {
        string src = $"{bufName}.readInt()";
        string constructor = type.DefEnum.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readString();";
    }

    public string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        string src = $"{type.DefBean.FullNameWithTopModule}.deserialize({bufName})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        //return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)}[n];for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}[i] = _e;}}}}";
        var __n = $"__n{depth}";
        var __e = $"__e{depth}";
        var __i = $"__i{depth}";
        var typeStr = $"{type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)}[{__n}]";
        if (type.Dimension > 1)
        {
            if (type.FinalElementType == null)
            {
                throw new System.Exception("多维数组没有元素类型");
            }
            typeStr = $"{type.FinalElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)}[{__n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {typeStr};for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)} {__e};{type.ElementType.Apply(this, bufName, __e, depth + 1)} {fieldName}[{__i}] = {__e};}}}}";
    }

    public string Accept(TList type, string bufName, string fieldName, int depth)
    {
        //return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.add(_e);}}}}";

        var __n = $"__n{depth}";
        var __e = $"__e{depth}";
        var __i = $"__i{depth}";
        return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}({__n});for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} {__e};  {type.ElementType.Apply(this, bufName, __e, depth + 1)} {fieldName}.add({__e});}}}}";
    }

    public string Accept(TSet type, string bufName, string fieldName, int depth)
    {
        //return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.add(_e);}}}}";

        var __n = $"__n{depth}";
        var __e = $"__e{depth}";
        var __i = $"__i{depth}";
        return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}({__n} * 3 / 2);for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} {__e};  {type.ElementType.Apply(this, bufName, __e, depth + 1)} {fieldName}.add({__e});}}}}";
    }

    public string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        //return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}.put(_k, _v);}}}}";

        var __n = $"__n{depth}";
        var __k = $"__k{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}({__n} * 3 / 2);for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.KeyType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} {__k};  {type.KeyType.Apply(this, bufName, __k, depth + 1)} {type.ValueType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, bufName, __v, depth + 1)}     {fieldName}.put({__k}, {__v});}}}}";
    }

    public string Accept(TDateTime type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readLong();";
    }
}
