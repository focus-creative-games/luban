using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.TypeVisitors
{
    public abstract class TypeActionVisitorAdaptor<T> : ITypeActionVisitor<T>
    {
        public virtual void Accept(TBool type, T x)
        {

        }

        public virtual void Accept(TByte type, T x)
        {

        }

        public virtual void Accept(TShort type, T x)
        {

        }

        public virtual void Accept(TFshort type, T x)
        {

        }

        public virtual void Accept(TInt type, T x)
        {

        }

        public virtual void Accept(TFint type, T x)
        {

        }

        public virtual void Accept(TLong type, T x)
        {

        }

        public virtual void Accept(TFlong type, T x)
        {

        }

        public virtual void Accept(TFloat type, T x)
        {

        }

        public virtual void Accept(TDouble type, T x)
        {

        }

        public virtual void Accept(TEnum type, T x)
        {

        }

        public virtual void Accept(TString type, T x)
        {

        }

        public virtual void Accept(TBytes type, T x)
        {

        }

        public virtual void Accept(TText type, T x)
        {

        }

        public virtual void Accept(TBean type, T x)
        {

        }

        public virtual void Accept(TArray type, T x)
        {

        }

        public virtual void Accept(TList type, T x)
        {

        }

        public virtual void Accept(TSet type, T x)
        {

        }

        public virtual void Accept(TMap type, T x)
        {

        }

        public virtual void Accept(TVector2 type, T x)
        {

        }

        public virtual void Accept(TVector3 type, T x)
        {

        }

        public virtual void Accept(TVector4 type, T x)
        {

        }

        public virtual void Accept(TDateTime type, T x)
        {

        }
    }
}
