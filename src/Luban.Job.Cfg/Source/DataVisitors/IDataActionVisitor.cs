using Luban.Job.Cfg.Datas;

namespace Luban.Job.Cfg.DataVisitors
{
    public interface IDataActionVisitor<T>
    {
        void Accept(DBool type, T x);

        void Accept(DByte type, T x);

        void Accept(DShort type, T x);

        void Accept(DFshort type, T x);

        void Accept(DInt type, T x);

        void Accept(DFint type, T x);

        void Accept(DLong type, T x);

        void Accept(DFlong type, T x);

        void Accept(DFloat type, T x);

        void Accept(DDouble type, T x);

        void Accept(DEnum type, T x);

        void Accept(DString type, T x);

        void Accept(DText type, T x);

        void Accept(DBytes type, T x);

        void Accept(DVector2 type, T x);

        void Accept(DVector3 type, T x);

        void Accept(DVector4 type, T x);

        void Accept(DDateTime type, T x);

        void Accept(DBean type, T x);

        void Accept(DArray type, T x);

        void Accept(DList type, T x);

        void Accept(DSet type, T x);

        void Accept(DMap type, T x);
    }

    public interface IDataActionVisitor<T1, T2>
    {
        void Accept(DBool type, T1 x, T2 y);

        void Accept(DByte type, T1 x, T2 y);

        void Accept(DShort type, T1 x, T2 y);

        void Accept(DFshort type, T1 x, T2 y);

        void Accept(DInt type, T1 x, T2 y);

        void Accept(DFint type, T1 x, T2 y);

        void Accept(DLong type, T1 x, T2 y);

        void Accept(DFlong type, T1 x, T2 y);

        void Accept(DFloat type, T1 x, T2 y);

        void Accept(DDouble type, T1 x, T2 y);

        void Accept(DEnum type, T1 x, T2 y);

        void Accept(DDateTime type, T1 x, T2 y);

        void Accept(DString type, T1 x, T2 y);

        void Accept(DText type, T1 x, T2 y);

        void Accept(DBytes type, T1 x, T2 y);

        void Accept(DVector2 type, T1 x, T2 y);

        void Accept(DVector3 type, T1 x, T2 y);

        void Accept(DVector4 type, T1 x, T2 y);

        void Accept(DBean type, T1 x, T2 y);

        void Accept(DArray type, T1 x, T2 y);

        void Accept(DList type, T1 x, T2 y);

        void Accept(DSet type, T1 x, T2 y);

        void Accept(DMap type, T1 x, T2 y);
    }
}
