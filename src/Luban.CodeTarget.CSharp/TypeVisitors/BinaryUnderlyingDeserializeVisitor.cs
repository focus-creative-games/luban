using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CodeTarget.CSharp.TypeVisitors;

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
        return $"{fieldName} = ({ type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}){bufName}.ReadInt();";
    }

    public string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadString();";
    }

    public string Accept(TText type, string bufName, string fieldName, int depth)
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
        return $"{fieldName} = {src};";
    }

    public string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        string __n = $"__n{depth}";
        string __e = $"__e{depth}";
        string __index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}[{__n}]";
        if (type.Dimension > 1)
        {
            if (type.FinalElementType == null)
            {
                throw new System.Exception("��ά����û��Ԫ������");
            }
            typeStr = $"{type.FinalElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}[{__n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        return $"{{int {__n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {typeStr};for(var {__index} = 0 ; {__index} < {__n} ; {__index}++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__e};{type.ElementType.Apply(this, bufName, $"{__e}", depth + 1)} {fieldName}[{__index}] = {__e};}}}}";
    }

    public string Accept(TList type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string _e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({n});for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {_e};  {type.ElementType.Apply(this, bufName, $"{_e}", depth + 1)} {fieldName}.Add({_e});}}}}";
    }

    public string Accept(TSet type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string _e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(/*{n} * 3 / 2*/);for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {_e};  {type.ElementType.Apply(this, bufName, $"{_e}", +1)} {fieldName}.Add({_e});}}}}";
    }

    public string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string _k = $"_k{depth}";
        string _v = $"_v{depth}";
        string i = $"i{depth}";
        return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({n} * 3 / 2);for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} {_k};  {type.KeyType.Apply(this, bufName, _k, depth + 1)} {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)} {_v};  {type.ValueType.Apply(this, bufName, _v, depth + 1)}     {fieldName}.Add({_k}, {_v});}}}}";
    }
}