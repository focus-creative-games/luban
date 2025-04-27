using Luban.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Luban.DataVisitors;

public class DataActionHelpVisitor<T> : IDataActionVisitor<T>
{
    private readonly IDataActionVisitor<T> _underlyingVisitor;

    public DataActionHelpVisitor(IDataActionVisitor<T> underlyingVisitor)
    {
        _underlyingVisitor = underlyingVisitor;
    }

    public void Accept(DBool type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DByte type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DShort type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DInt type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DLong type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DFloat type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DDouble type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DEnum type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DString type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DDateTime type, T x)
    {
        _underlyingVisitor.Accept(type, x);
    }

    public void Accept(DBean type, T x)
    {
        _underlyingVisitor.Accept(type, x);
        foreach (var fieldValue in type.Fields)
        {
            if (fieldValue == null)
            {
                continue;
            }
            fieldValue.Apply(this, x);
        }
    }

    public void Accept(DArray type, T x)
    {
        _underlyingVisitor.Accept(type, x);
        foreach (var e in type.Datas)
        {
            if (e != null)
            {
                e.Apply(this, x);
            }
        }
    }

    public void Accept(DList type, T x)
    {
        _underlyingVisitor.Accept(type, x);
        foreach (var e in type.Datas)
        {
            if (e != null)
            {
                e.Apply(this, x);
            }
        }
    }

    public void Accept(DSet type, T x)
    {
        _underlyingVisitor.Accept(type, x);
        foreach (var e in type.Datas)
        {
            e.Apply(this, x);
        }
    }

    public void Accept(DMap type, T x)
    {
        _underlyingVisitor.Accept(type, x);
        foreach (var e in type.DataMap)
        {
            e.Key.Apply(this, x);
            e.Value.Apply(this, x);
        }
    }
}

public class DataActionHelpVisitor<T1, T2> : IDataActionVisitor<T1, T2>
{
    private readonly IDataActionVisitor<T1, T2> _underlyingVisitor;

    public DataActionHelpVisitor(IDataActionVisitor<T1, T2> underlyingVisitor)
    {
        _underlyingVisitor = underlyingVisitor;
    }

    public void Accept(DBool type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DByte type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DShort type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DInt type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DLong type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DFloat type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DDouble type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DEnum type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DString type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DDateTime type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
    }

    public void Accept(DBean type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
        foreach (var fieldValue in type.Fields)
        {
            if (fieldValue == null)
            {
                continue;
            }
            fieldValue.Apply(this, x, y);
        }
    }

    public void Accept(DArray type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
        foreach (var e in type.Datas)
        {
            if (e != null)
            {
                e.Apply(this, x, y);
            }
        }
    }

    public void Accept(DList type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
        foreach (var e in type.Datas)
        {
            if (e != null)
            {
                e.Apply(this, x, y);
            }
        }
    }

    public void Accept(DSet type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
        foreach (var e in type.Datas)
        {
            e.Apply(this, x, y);
        }
    }

    public void Accept(DMap type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(type, x, y);
        foreach (var e in type.DataMap)
        {
            e.Key.Apply(this, x, y);
            e.Value.Apply(this, x, y);
        }
    }
}
