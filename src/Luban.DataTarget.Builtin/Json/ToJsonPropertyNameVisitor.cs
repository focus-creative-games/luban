using Luban.Datas;
using Luban.DataVisitors;

namespace Luban.DataExporter.Builtin.Json;

public class ToJsonPropertyNameVisitor : IDataFuncVisitor<string>
{
    public static ToJsonPropertyNameVisitor Ins { get; } = new();

    public string Accept(DBool type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DByte type)
    {
        return type.Value.ToString();
    }

    public string Accept(DShort type)
    {
        return type.Value.ToString();
    }

    public string Accept(DInt type)
    {
        return type.Value.ToString();
    }

    public string Accept(DLong type)
    {
        return type.Value.ToString();
    }

    public string Accept(DFloat type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DDouble type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DEnum type)
    {
        return type.Value.ToString();
    }

    public string Accept(DString type)
    {
        return type.Value;
    }

    public string Accept(DDateTime type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DBean type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DArray type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DList type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DSet type)
    {
        throw new NotSupportedException();
    }

    public string Accept(DMap type)
    {
        throw new NotSupportedException();
    }
}
