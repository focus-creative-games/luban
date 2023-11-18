using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class StringDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static StringDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string strName, string varName)
    {
        return $"{varName} = bool.Parse({strName});";
    }

    public string Accept(TByte type, string strName, string varName)
    {
        return $"{varName} = byte.Parse({strName});";
    }

    public string Accept(TShort type, string strName, string varName)
    {
        return $"{varName} = short.Parse({strName});";
    }

    public string Accept(TInt type, string strName, string varName)
    {
        return $"{varName} = int.Parse({strName});";
    }

    public string Accept(TLong type, string strName, string varName)
    {
        return $"{varName} = long.Parse({strName});";
    }

    public string Accept(TFloat type, string strName, string varName)
    {
        return $"{varName} = float.Parse({strName});";
    }

    public string Accept(TDouble type, string strName, string varName)
    {
        return $"{varName} = double.Parse({strName});";
    }

    public string Accept(TEnum type, string strName, string varName)
    {
        return $"{varName} = ({type.Apply(DeclaringTypeNameVisitor.Ins)})int.Parse({strName});";
    }

    public string Accept(TString type, string strName, string varName)
    {
        return $"{varName} = {strName};";
    }

    public string Accept(TDateTime type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TBean type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TArray type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TList type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TSet type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TMap type, string strName, string varName)
    {
        throw new NotSupportedException();
    }
}
