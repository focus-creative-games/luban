using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class RecursiveResolveVisitor : ITypeFuncVisitor<string, string, string>
{
    public static RecursiveResolveVisitor Ins { get; } = new();

    public string Accept(TBool type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TByte type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TShort type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TInt type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TLong type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TFloat type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDouble type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TEnum type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TString type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDateTime type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type, string fieldName, string tablesName)
    {
        return $"{fieldName}?.Resolve({tablesName});";
    }

    public string Accept(TArray type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}) {{ _e?.Resolve({tablesName}); }}";
    }

    public string Accept(TList type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}) {{ _e?.Resolve({tablesName}); }}";
    }

    public string Accept(TSet type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}) {{ _e?.Resolve({tablesName}); }}";
    }

    public string Accept(TMap type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}.Values) {{ _e?.Resolve({tablesName}); }}";
    }
}
