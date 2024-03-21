using Luban.Datas;
using Luban.Types;

namespace Luban.DataVisitors;

public interface IDataActionVisitor2<T>
{
    void Accept(DBool data, TType type, T x);

    void Accept(DByte data, TType type, T x);

    void Accept(DShort data, TType type, T x);

    void Accept(DInt data, TType type, T x);

    void Accept(DLong data, TType type, T x);

    void Accept(DFloat data, TType type, T x);

    void Accept(DDouble data, TType type, T x);

    void Accept(DEnum data, TType type, T x);

    void Accept(DString data, TType type, T x);

    void Accept(DDateTime data, TType type, T x);

    void Accept(DBean data, TType type, T x);

    void Accept(DArray data, TType type, T x);

    void Accept(DList data, TType type, T x);

    void Accept(DSet data, TType type, T x);

    void Accept(DMap data, TType type, T x);
}

public interface IDataActionVisitor2<T1, T2>
{
    void Accept(DBool data, TType type, T1 x, T2 y);

    void Accept(DByte data, TType type, T1 x, T2 y);

    void Accept(DShort data, TType type, T1 x, T2 y);

    void Accept(DInt data, TType type, T1 x, T2 y);

    void Accept(DLong data, TType type, T1 x, T2 y);

    void Accept(DFloat data, TType type, T1 x, T2 y);

    void Accept(DDouble data, TType type, T1 x, T2 y);

    void Accept(DEnum data, TType type, T1 x, T2 y);

    void Accept(DString data, TType type, T1 x, T2 y);

    void Accept(DDateTime data, TType type, T1 x, T2 y);

    void Accept(DBean data, TType type, T1 x, T2 y);

    void Accept(DArray data, TType type, T1 x, T2 y);

    void Accept(DList data, TType type, T1 x, T2 y);

    void Accept(DSet data, TType type, T1 x, T2 y);

    void Accept(DMap data, TType type, T1 x, T2 y);
}
