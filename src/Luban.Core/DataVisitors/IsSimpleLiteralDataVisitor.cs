using Luban.Datas;

namespace Luban.DataVisitors;

public class IsSimpleLiteralDataVisitor : IDataFuncVisitor<bool>
{
    public static IsSimpleLiteralDataVisitor Ins { get; } = new();

    public bool Accept(DBool type)
    {
        return true;
    }

    public bool Accept(DByte type)
    {
        return true;
    }

    public bool Accept(DShort type)
    {
        return true;
    }

    public bool Accept(DInt type)
    {
        return true;
    }

    public bool Accept(DLong type)
    {
        return true;
    }

    public bool Accept(DFloat type)
    {
        return true;
    }

    public bool Accept(DDouble type)
    {
        return true;
    }

    public bool Accept(DEnum type)
    {
        return true;
    }

    public bool Accept(DString type)
    {
        return true;
    }

    public bool Accept(DDateTime type)
    {
        return true;
    }

    public bool Accept(DBean type)
    {
        return false;
    }

    public bool Accept(DArray type)
    {
        return false;
    }

    public bool Accept(DList type)
    {
        return false;
    }

    public bool Accept(DSet type)
    {
        return false;
    }

    public bool Accept(DMap type)
    {
        return false;
    }
}
