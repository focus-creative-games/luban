using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{

    public interface ITypeActionVisitor<T>
    {
        void Accept(TBool type, T x);

        void Accept(TByte type, T x);

        void Accept(TShort type, T x);

        void Accept(TFshort type, T x);

        void Accept(TInt type, T x);

        void Accept(TFint type, T x);

        void Accept(TLong type, T x);

        void Accept(TFlong type, T x);

        void Accept(TFloat type, T x);

        void Accept(TDouble type, T x);

        void Accept(TEnum type, T x);

        void Accept(TString type, T x);

        void Accept(TBytes type, T x);

        void Accept(TText type, T x);

        void Accept(TBean type, T x);

        void Accept(TArray type, T x);

        void Accept(TList type, T x);

        void Accept(TSet type, T x);

        void Accept(TMap type, T x);

        void Accept(TVector2 type, T x);

        void Accept(TVector3 type, T x);

        void Accept(TVector4 type, T x);

        void Accept(TDateTime type, T x);
    }

    public interface ITypeActionVisitor<T1, T2>
    {
        void Accept(TBool type, T1 x, T2 y);

        void Accept(TByte type, T1 x, T2 y);

        void Accept(TShort type, T1 x, T2 y);

        void Accept(TFshort type, T1 x, T2 y);

        void Accept(TInt type, T1 x, T2 y);

        void Accept(TFint type, T1 x, T2 y);

        void Accept(TLong type, T1 x, T2 y);

        void Accept(TFlong type, T1 x, T2 y);

        void Accept(TFloat type, T1 x, T2 y);

        void Accept(TDouble type, T1 x, T2 y);

        void Accept(TEnum type, T1 x, T2 y);

        void Accept(TString type, T1 x, T2 y);

        void Accept(TBytes type, T1 x, T2 y);

        void Accept(TText type, T1 x, T2 y);

        void Accept(TBean type, T1 x, T2 y);

        void Accept(TArray type, T1 x, T2 y);

        void Accept(TList type, T1 x, T2 y);

        void Accept(TSet type, T1 x, T2 y);

        void Accept(TMap type, T1 x, T2 y);

        void Accept(TVector2 type, T1 x, T2 y);

        void Accept(TVector3 type, T1 x, T2 y);

        void Accept(TVector4 type, T1 x, T2 y);

        void Accept(TDateTime type, T1 x, T2 y);
    }
}
