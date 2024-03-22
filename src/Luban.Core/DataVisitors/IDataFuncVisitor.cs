using Luban.Datas;

namespace Luban.DataVisitors;

public interface IDataFuncVisitor<TR>
{
    TR Accept(DBool type);

    TR Accept(DByte type);

    TR Accept(DShort type);

    TR Accept(DInt type);

    TR Accept(DLong type);

    TR Accept(DFloat type);

    TR Accept(DDouble type);

    TR Accept(DEnum type);

    TR Accept(DString type);

    TR Accept(DDateTime type);

    TR Accept(DBean type);

    TR Accept(DArray type);

    TR Accept(DList type);

    TR Accept(DSet type);

    TR Accept(DMap type);
}

public interface IDataFuncVisitor<T, TR>
{
    TR Accept(DBool type, T x);

    TR Accept(DByte type, T x);

    TR Accept(DShort type, T x);

    TR Accept(DInt type, T x);

    TR Accept(DLong type, T x);

    TR Accept(DFloat type, T x);

    TR Accept(DDouble type, T x);

    TR Accept(DEnum type, T x);

    TR Accept(DString type, T x);

    TR Accept(DDateTime type, T x);

    TR Accept(DBean type, T x);

    TR Accept(DArray type, T x);

    TR Accept(DList type, T x);

    TR Accept(DSet type, T x);

    TR Accept(DMap type, T x);
}

public interface IDataFuncVisitor<T1, T2, TR>
{
    TR Accept(DBool type, T1 x, T2 y);

    TR Accept(DByte type, T1 x, T2 y);

    TR Accept(DShort type, T1 x, T2 y);

    TR Accept(DInt type, T1 x, T2 y);
    TR Accept(DLong type, T1 x, T2 y);

    TR Accept(DFloat type, T1 x, T2 y);

    TR Accept(DDouble type, T1 x, T2 y);

    TR Accept(DEnum type, T1 x, T2 y);

    TR Accept(DString type, T1 x, T2 y);

    TR Accept(DDateTime type, T1 x, T2 y);

    TR Accept(DBean type, T1 x, T2 y);

    TR Accept(DArray type, T1 x, T2 y);

    TR Accept(DList type, T1 x, T2 y);

    TR Accept(DSet type, T1 x, T2 y);

    TR Accept(DMap type, T1 x, T2 y);
}

