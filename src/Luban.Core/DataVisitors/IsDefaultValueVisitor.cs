using System.Numerics;
using Luban.Datas;

namespace Luban.DataVisitors;

public class IsDefaultValueVisitor : IDataFuncVisitor<bool>
{
    public static IsDefaultValueVisitor Ins { get; } = new();

    public bool Accept(DBool type)
    {
        return type.Value == false;
    }

    public bool Accept(DByte type)
    {
        return type.Value == 0;
    }

    public bool Accept(DShort type)
    {
        return type.Value == 0;
    }

    public bool Accept(DInt type)
    {
        return type.Value == 0;
    }

    public bool Accept(DLong type)
    {
        return type.Value == 0;
    }

    public bool Accept(DFloat type)
    {
        return type.Value == 0;
    }

    public bool Accept(DDouble type)
    {
        return type.Value == 0;
    }

    public bool Accept(DEnum type)
    {
        return type.Value == 0;
    }

    public bool Accept(DString type)
    {
        return string.IsNullOrEmpty(type.Value);
    }

    public bool Accept(DBean type)
    {
        return false;
    }

    public bool Accept(DArray type)
    {
        return type.Datas.Count == 0;
    }

    public bool Accept(DList type)
    {
        return type.Datas.Count == 0;
    }

    public bool Accept(DSet type)
    {
        return type.Datas.Count == 0;
    }

    public bool Accept(DMap type)
    {
        return type.Datas.Count == 0;
    }

    public bool Accept(DDateTime type)
    {
        return false;
    }
}
