using Luban.Datas;
using Luban.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataVisitors;

public class DataActionHelpVisitor2<T> : IDataActionVisitor2<T>
{
    private readonly IDataActionVisitor2<T> _underlyingVisitor;

    public DataActionHelpVisitor2(IDataActionVisitor2<T> underlyingVisitor)
    {
        _underlyingVisitor = underlyingVisitor;
    }

    public void Accept(DBool data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DByte data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DShort data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DInt data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DLong data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DFloat data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DDouble data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DEnum data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DString data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DDateTime data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
    }

    public void Accept(DBean data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
        var defFields = data.ImplType.HierarchyFields;
        int i = 0;
        foreach (var fieldValue in data.Fields)
        {
            if (fieldValue == null)
            {
                i++;
                continue;
            }
            var defField = defFields[i++];
            var fieldType = defField.CType;
            fieldValue.Apply(this, fieldType, x);
        }
    }

    public void Accept(DArray data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                continue;
            }
            ele.Apply(this, type.ElementType, x);
        }
    }

    public void Accept(DList data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                continue;
            }
            ele.Apply(this, type.ElementType, x);
        }
    }

    public void Accept(DSet data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
        foreach (var ele in data.Datas)
        {
            ele.Apply(this, type.ElementType, x);
        }
    }

    public void Accept(DMap data, TType type, T x)
    {
        _underlyingVisitor.Accept(data, type, x);
        TMap mapType = (TMap)type;
        foreach (var e in data.DataMap)
        {
            e.Key.Apply(this, mapType.KeyType, x);
            e.Value.Apply(this, mapType.ValueType, x);
        }
    }
}

public class DataActionHelpVisitor2<T1, T2> : IDataActionVisitor2<T1, T2>
{
    private readonly IDataActionVisitor2<T1, T2> _underlyingVisitor;

    public DataActionHelpVisitor2(IDataActionVisitor2<T1, T2> underlyingVisitor)
    {
        _underlyingVisitor = underlyingVisitor;
    }

    public void Accept(DBool data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DByte data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DShort data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DInt data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DLong data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DFloat data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DDouble data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DEnum data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DString data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DDateTime data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
    }

    public void Accept(DBean data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
        var defFields = data.ImplType.HierarchyFields;
        int i = 0;
        foreach (var fieldValue in data.Fields)
        {
            if (fieldValue == null)
            {
                i++;
                continue;
            }
            var defField = defFields[i++];
            var fieldType = defField.CType;
            fieldValue.Apply(this, fieldType, x, y);
        }
    }

    public void Accept(DArray data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
        foreach (var e in data.Datas)
        {
            if (e != null)
            {
                e.Apply(this, type.ElementType, x, y);
            }
        }
    }

    public void Accept(DList data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
        foreach (var e in data.Datas)
        {
            if (e != null)
            {
                e.Apply(this, type.ElementType, x, y);
            }
        }
    }

    public void Accept(DSet data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
        foreach (var e in data.Datas)
        {
            e.Apply(this, type.ElementType, x, y);
        }
    }

    public void Accept(DMap data, TType type, T1 x, T2 y)
    {
        _underlyingVisitor.Accept(data, type, x, y);
        TMap mapType = (TMap)type;
        foreach (var e in data.DataMap)
        {
            e.Key.Apply(this, mapType.KeyType, x, y);
            e.Value.Apply(this, mapType.ValueType, x, y);
        }
    }
}
