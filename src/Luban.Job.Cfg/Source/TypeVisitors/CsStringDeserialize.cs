using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CsStringDeserialize : ITypeFuncVisitor<string, string, string>
    {
        public static CsStringDeserialize Ins { get; } = new();

        public string Accept(TBool type, string strName, string varName)
        {
            return $"{varName} = bool.Parse({strName});";
        }

        public string Accept(TByte type, string strName, string varName)
        {
            return $"{varName} = byte.Parse({strName});";
        }

        public string Accept(TShort type, string strName, string varName)
        {
            return $"{varName} = short.Parse({strName});";
        }

        public string Accept(TFshort type, string strName, string varName)
        {
            return $"{varName} = short.Parse({strName});";
        }

        public string Accept(TInt type, string strName, string varName)
        {
            return $"{varName} = int.Parse({strName});";
        }

        public string Accept(TFint type, string strName, string varName)
        {
            return $"{varName} = int.Parse({strName});";
        }

        public string Accept(TLong type, string strName, string varName)
        {
            return $"{varName} = long.Parse({strName});";
        }

        public string Accept(TFlong type, string strName, string varName)
        {
            return $"{varName} = long.Parse({strName});";
        }

        public string Accept(TFloat type, string strName, string varName)
        {
            return $"{varName} = float.Parse({strName});";
        }

        public string Accept(TDouble type, string strName, string varName)
        {
            return $"{varName} = double.Parse({strName});";
        }

        public string Accept(TEnum type, string strName, string varName)
        {
            return $"{varName} = ({type.Apply(CsDefineTypeName.Ins)})int.Parse({strName});";
        }

        public string Accept(TString type, string strName, string varName)
        {
            return $"{varName} = {strName};";
        }

        public string Accept(TBytes type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TBean type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TArray type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TList type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TSet type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TMap type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TVector2 type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TVector3 type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TVector4 type, string strName, string varName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TDateTime type, string strName, string varName)
        {
            throw new NotSupportedException();
        }
    }
}
