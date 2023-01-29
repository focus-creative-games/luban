using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class LuaDeserializeMethodNameVisitor : ITypeFuncVisitor<string>
    {
        public static LuaDeserializeMethodNameVisitor Ins { get; } = new LuaDeserializeMethodNameVisitor();

        public string Accept(TBool type)
        {
            return "readBool";
        }

        public string Accept(TByte type)
        {
            return "readByte";
        }

        public string Accept(TShort type)
        {
            return "readShort";
        }

        public string Accept(TFshort type)
        {
            return "readFshort";
        }

        public string Accept(TInt type)
        {
            return "readInt";
        }

        public string Accept(TFint type)
        {
            return "readFint";
        }

        public string Accept(TLong type)
        {
            return "readLong";
        }

        public string Accept(TFlong type)
        {
            return "readFlong";
        }

        public string Accept(TFloat type)
        {
            return "readFloat";
        }

        public string Accept(TDouble type)
        {
            return "readDouble";
        }

        public string Accept(TEnum type)
        {
            return "readInt";
        }

        public string Accept(TString type)
        {
            return "readString";
        }

        public string Accept(TBytes type)
        {
            return "readBytes";
        }

        public string Accept(TText type)
        {
            return "readString";
        }

        public string Accept(TBean type)
        {
            return $"beans['{type.Bean.FullName}']._deserialize";
        }

        public string Accept(TArray type)
        {
            return "readList";
        }

        public string Accept(TList type)
        {
            return "readList";
        }

        public string Accept(TSet type)
        {
            return "readSet";
        }

        public string Accept(TMap type)
        {
            return "readMap";
        }

        public string Accept(TVector2 type)
        {
            return "readVector2";
        }

        public string Accept(TVector3 type)
        {
            return "readVector3";
        }

        public string Accept(TVector4 type)
        {
            return "readVector4";
        }

        public string Accept(TDateTime type)
        {
            return "readLong";
        }
    }
}
