using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class BinaryUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static BinaryUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadBool();";
    }

    public string Accept(TByte type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadByte();";
    }

    public string Accept(TShort type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadShort();";
    }
    public string Accept(TInt type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadInt();";
    }

    public string Accept(TLong type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadLong();";
    }

    public string Accept(TFloat type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadFloat();";
    }

    public string Accept(TDouble type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadDouble();";
    }

    public string Accept(TEnum type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = ({type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}){bufName}.ReadInt();";
    }

    public string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadString();";
    }

    public string Accept(TDateTime type, string bufName, string fieldName, int depth)
    {
        string src = $"{bufName}.ReadLong()";
        return $"{fieldName} = {src};";
    }

    public string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        string src = $"{type.DefBean.FullName}.Deserialize{type.DefBean.Name}({bufName})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        string n = $"__n{depth}";
        string e = $"__e{depth}";
        string index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}[{n}]";
        if (type.Dimension > 1)
        {
            typeStr = $"{type.FinalElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}[{n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {typeStr};for(var {index} = 0 ; {index} < {n} ; {index}++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {e};{type.ElementType.Apply(this, bufName, $"{e}", depth + 1)} {fieldName}[{index}] = {e};}}}}";
    }

    public string Accept(TList type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({n});for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {e};  {type.ElementType.Apply(this, bufName, $"{e}", depth + 1)} {fieldName}.Add({e});}}}}";
    }

    public string Accept(TSet type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(/*{n} * 3 / 2*/);for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {e};  {type.ElementType.Apply(this, bufName, $"{e}", +1)} {fieldName}.Add({e});}}}}";
    }

    public string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string k = $"_k{depth}";
        string v = $"_v{depth}";
        string i = $"i{depth}";
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({n} * 3 / 2);for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} {k};  {type.KeyType.Apply(this, bufName, k, depth + 1)} {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)} {v};  {type.ValueType.Apply(this, bufName, v, depth + 1)}     {fieldName}.Add({k}, {v});}}}}";
    }
}
