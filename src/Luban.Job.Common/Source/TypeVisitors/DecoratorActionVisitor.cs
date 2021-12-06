using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public abstract class DecoratorActionVisitor<T> : ITypeActionVisitor<T>
    {
        public abstract void DoAccept(TType type, T x);

        public virtual void Accept(TBool type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TByte type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TShort type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TFshort type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TInt type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TFint type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TLong type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TFlong type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TFloat type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TDouble type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TEnum type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TString type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TText type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TBytes type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TDateTime type, T x)
        {
            DoAccept(type, x);
        }
        public virtual void Accept(TVector2 type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TVector3 type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TVector4 type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TBean type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TArray type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TList type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TSet type, T x)
        {
            DoAccept(type, x);
        }

        public virtual void Accept(TMap type, T x)
        {
            DoAccept(type, x);
        }

    }

    public abstract class DecoratorActionVisitor<T1, T2> : ITypeActionVisitor<T1, T2>
    {

        public abstract void DoAccept(TType type, T1 x, T2 y);

        public virtual void Accept(TBool type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TByte type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TShort type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TFshort type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TInt type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TFint type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TLong type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TFlong type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TFloat type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TDouble type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TEnum type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TString type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TText type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TBytes type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TDateTime type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TVector2 type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TVector3 type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TVector4 type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TBean type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TArray type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TList type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TSet type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }

        public virtual void Accept(TMap type, T1 x, T2 y)
        {
            DoAccept(type, x, y);
        }
    }
}
