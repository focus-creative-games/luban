using Luban.Job.Common.Types;
using System;

namespace Luban.Job.Common.TypeVisitors
{
    public class LuaValueOrDefaultVisitor : ITypeFuncVisitor<string, string>
    {
        public static LuaValueOrDefaultVisitor Ins { get; } = new LuaValueOrDefaultVisitor();

        public string Accept(TBool type, string x)
        {
            return $"{x} == true";
        }

        public string Accept(TByte type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TShort type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TFshort type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TInt type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TFint type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TLong type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TFlong type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TFloat type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TDouble type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TEnum type, string x)
        {
            return $"{x} or 0";
        }

        public string Accept(TString type, string x)
        {
            return $"{x} or \"\"";
        }

        public string Accept(TBytes type, string x)
        {
            return $"{x} or \"\"";
        }

        public string Accept(TText type, string x)
        {
            throw new NotImplementedException();
        }

        public string Accept(TBean type, string x)
        {
            return $"{x} or {{}}";
        }

        public string Accept(TArray type, string x)
        {
            return $"{x} or {{}}";
        }

        public string Accept(TList type, string x)
        {
            return $"{x} or {{}}";
        }

        public string Accept(TSet type, string x)
        {
            return $"{x} or {{}}";
        }

        public string Accept(TMap type, string x)
        {
            return $"{x} or {{}}";
        }

        public string Accept(TVector2 type, string x)
        {
            return $"{x} or default_vector2";
        }

        public string Accept(TVector3 type, string x)
        {
            return $"{x} or default_vector3";
        }

        public string Accept(TVector4 type, string x)
        {
            return $"{x} or default_vector4";
        }

        public string Accept(TDateTime type, string x)
        {
            return $"{x} or 0";
        }
    }
}
