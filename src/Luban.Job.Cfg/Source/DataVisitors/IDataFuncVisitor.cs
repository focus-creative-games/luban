using Luban.Job.Cfg.Datas;

namespace Luban.Job.Cfg.DataVisitors
{
    public interface IDataFuncVisitor<TR>
    {
        TR Accept(DBool type);

        TR Accept(DByte type);

        TR Accept(DShort type);

        TR Accept(DFshort type);

        TR Accept(DInt type);

        TR Accept(DFint type);

        TR Accept(DLong type);

        TR Accept(DFlong type);

        TR Accept(DFloat type);

        TR Accept(DDouble type);

        TR Accept(DEnum type);

        TR Accept(DString type);

        TR Accept(DBytes type);

        TR Accept(DText type);

        TR Accept(DBean type);

        TR Accept(DArray type);

        TR Accept(DList type);

        TR Accept(DSet type);

        TR Accept(DMap type);

        TR Accept(DVector2 type);

        TR Accept(DVector3 type);

        TR Accept(DVector4 type);

        TR Accept(DDateTime type);
    }

    public interface IDataFuncVisitor<T, TR>
    {
        TR Accept(DBool type, T x);

        TR Accept(DByte type, T x);

        TR Accept(DShort type, T x);

        TR Accept(DFshort type, T x);

        TR Accept(DInt type, T x);

        TR Accept(DFint type, T x);

        TR Accept(DLong type, T x);

        TR Accept(DFlong type, T x);

        TR Accept(DFloat type, T x);

        TR Accept(DDouble type, T x);

        TR Accept(DEnum type, T x);

        TR Accept(DString type, T x);

        TR Accept(DBytes type, T x);

        TR Accept(DText type, T x);

        TR Accept(DBean type, T x);

        TR Accept(DArray type, T x);

        TR Accept(DList type, T x);

        TR Accept(DSet type, T x);

        TR Accept(DMap type, T x);

        TR Accept(DVector2 type, T x);

        TR Accept(DVector3 type, T x);

        TR Accept(DVector4 type, T x);

        TR Accept(DDateTime type, T x);
    }
}
