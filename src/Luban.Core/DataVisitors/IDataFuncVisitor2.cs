using Luban.Datas;
using Luban.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataVisitors;

public interface IDataFuncVisitor2<TR>
{
    TR Accept(DBool data, TType type);

    TR Accept(DByte data, TType type);

    TR Accept(DShort data, TType type);

    TR Accept(DInt data, TType type);

    TR Accept(DLong data, TType type);

    TR Accept(DFloat data, TType type);

    TR Accept(DDouble data, TType type);

    TR Accept(DEnum data, TType type);

    TR Accept(DString data, TType type);

    TR Accept(DDateTime data, TType type);

    TR Accept(DBean data, TType type);

    TR Accept(DArray data, TType type);

    TR Accept(DList data, TType type);

    TR Accept(DSet data, TType type);

    TR Accept(DMap data, TType type);
}

public interface IDataFuncVisitor2<T, TR>
{
    TR Accept(DBool data, TType type, T x);

    TR Accept(DByte data, TType type, T x);

    TR Accept(DShort data, TType type, T x);

    TR Accept(DInt data, TType type, T x);

    TR Accept(DLong data, TType type, T x);

    TR Accept(DFloat data, TType type, T x);

    TR Accept(DDouble data, TType type, T x);

    TR Accept(DEnum data, TType type, T x);

    TR Accept(DString data, TType type, T x);

    TR Accept(DDateTime data, TType type, T x);

    TR Accept(DBean data, TType type, T x);

    TR Accept(DArray data, TType type, T x);

    TR Accept(DList data, TType type, T x);

    TR Accept(DSet data, TType type, T x);

    TR Accept(DMap data, TType type, T x);
}

public interface IDataFuncVisitor2<T1, T2, TR>
{
    TR Accept(DBool data, TType type, T1 x, T2 y);

    TR Accept(DByte data, TType type, T1 x, T2 y);

    TR Accept(DShort data, TType type, T1 x, T2 y);

    TR Accept(DInt data, TType type, T1 x, T2 y);
    TR Accept(DLong data, TType type, T1 x, T2 y);

    TR Accept(DFloat data, TType type, T1 x, T2 y);

    TR Accept(DDouble data, TType type, T1 x, T2 y);

    TR Accept(DEnum data, TType type, T1 x, T2 y);

    TR Accept(DString data, TType type, T1 x, T2 y);

    TR Accept(DDateTime data, TType type, T1 x, T2 y);

    TR Accept(DBean data, TType type, T1 x, T2 y);

    TR Accept(DArray data, TType type, T1 x, T2 y);

    TR Accept(DList data, TType type, T1 x, T2 y);

    TR Accept(DSet data, TType type, T1 x, T2 y);

    TR Accept(DMap data, TType type, T1 x, T2 y);
}

