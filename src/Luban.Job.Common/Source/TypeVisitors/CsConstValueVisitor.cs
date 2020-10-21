using Luban.Job.Common.Types;
using System;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsConstValueVisitor : ITypeFuncVisitor<string, string>
    {
        public static CsConstValueVisitor Ins { get; } = new CsConstValueVisitor();

        public string Accept(TBool type, string x)
        {
            return x.ToLower();
        }

        public string Accept(TByte type, string x)
        {
            return x;
        }

        public string Accept(TShort type, string x)
        {
            return x;
        }

        public string Accept(TFshort type, string x)
        {
            return x;
        }

        public string Accept(TInt type, string x)
        {
            return x;
        }

        public string Accept(TFint type, string x)
        {
            return x;
        }

        public virtual string Accept(TLong type, string x)
        {
            return x + "L";
        }

        public virtual string Accept(TFlong type, string x)
        {
            return x + "L";
        }

        public virtual string Accept(TFloat type, string x)
        {
            return x + "f";
        }

        public string Accept(TDouble type, string x)
        {
            return x;
        }

        public string Accept(TEnum type, string x)
        {
            return x;
        }

        public string Accept(TString type, string x)
        {
            return "\"" + x + "\"";
        }

        public string Accept(TBytes type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type, string x)
        {
            return "\"" + x + "\"";
        }

        public string Accept(TBean type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TArray type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TList type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TSet type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TMap type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TVector2 type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TVector3 type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TVector4 type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TDateTime type, string x)
        {
            throw new NotImplementedException();
        }
    }
}
