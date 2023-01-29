using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class LuaSerializeMethodNameVisitor : ITypeFuncVisitor<string>
    {
        public static LuaSerializeMethodNameVisitor Ins { get; } = new LuaSerializeMethodNameVisitor();

        public string Accept(TBool type)
        {
            return "writeBool";
        }

        public string Accept(TByte type)
        {
            return "writeByte";
        }

        public string Accept(TShort type)
        {
            return "writeShort";
        }

        public string Accept(TFshort type)
        {
            return "writeFshort";
        }

        public string Accept(TInt type)
        {
            return "writeInt";
        }

        public string Accept(TFint type)
        {
            return "writeFint";
        }

        public string Accept(TLong type)
        {
            return "writeLong";
        }

        public string Accept(TFlong type)
        {
            return "writeFlong";
        }

        public string Accept(TFloat type)
        {
            return "writeFloat";
        }

        public string Accept(TDouble type)
        {
            return "writeDouble";
        }

        public string Accept(TEnum type)
        {
            return "writeInt";
        }

        public string Accept(TString type)
        {
            return "writeString";
        }

        public string Accept(TBytes type)
        {
            return "writeBytes";
        }

        public string Accept(TText type)
        {
            return "writeString";
        }

        public string Accept(TBean type)
        {
            return $"beans['{type.Bean.FullName}']._serialize";
        }

        public string Accept(TArray type)
        {
            return "writeList";
        }

        public string Accept(TList type)
        {
            return "writeList";
        }

        public string Accept(TSet type)
        {
            return "writeSet";
        }

        public string Accept(TMap type)
        {
            return "writeMap";
        }

        public string Accept(TVector2 type)
        {
            return "writeVector2";
        }

        public string Accept(TVector3 type)
        {
            return "writeVector3";
        }

        public string Accept(TVector4 type)
        {
            return "writeVector4";
        }

        public string Accept(TDateTime type)
        {
            return "writeLong";
        }
    }
}
