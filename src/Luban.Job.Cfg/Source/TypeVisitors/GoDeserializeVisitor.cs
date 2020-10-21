using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoDeserializeVisitor : ITypeFuncVisitor<string, string>
    {
        public static GoDeserializeVisitor Ins { get; } = new GoDeserializeVisitor();

        public string Accept(TBool type, string bufName)
        {
            return $"{bufName}.ReadBool()";
        }

        public string Accept(TByte type, string bufName)
        {
            return $"{bufName}.ReadByte()";
        }

        public string Accept(TShort type, string bufName)
        {
            return $"{bufName}.ReadShort()";
        }

        public string Accept(TFshort type, string bufName)
        {
            return $"{bufName}.ReadFshort()";
        }

        public string Accept(TInt type, string bufName)
        {
            return $"{bufName}.ReadInt()";
        }

        public string Accept(TFint type, string bufName)
        {
            return $"{bufName}.ReadFint()";
        }

        public string Accept(TLong type, string bufName)
        {
            return $"{bufName}.ReadLong()";
        }

        public string Accept(TFlong type, string bufName)
        {
            return $"{bufName}.ReadFlong()";
        }

        public string Accept(TFloat type, string bufName)
        {
            return $"{bufName}.ReadFloat()";
        }

        public string Accept(TDouble type, string bufName)
        {
            return $"{bufName}.ReadDouble()";
        }

        public string Accept(TEnum type, string bufName)
        {
            return $"{bufName}.ReadInt()";
        }

        public string Accept(TString type, string bufName)
        {
            return $"{bufName}.ReadString()";
        }

        public string Accept(TBytes type, string bufName)
        {
            return $"{bufName}.ReadBytes()";
        }

        public string Accept(TText type, string bufName)
        {
            return $"{bufName}.ReadString()";
        }

        public string Accept(TBean type, string bufName)
        {
            return type.Bean.IsAbstractType ? $"NewChild{type.Bean.GoFullName}({bufName})" : $"New{ type.Bean.GoFullName} ({ bufName})";
        }


        private string GenList(TType elementType, string bufName)
        {
            return $@"func (_buf2 *serialization.ByteBuf) (_v2 []{elementType.Apply(GoTypeNameVisitor.Ins)}, err2 error) {{
                _v2 = make([]{elementType.Apply(GoTypeNameVisitor.Ins)}, 0)
                var n int
                if n, err2 = _buf2.ReadSize(); err2 != nil {{return}}
                for i := 0 ; i < n ; i++ {{
                    var v3 {elementType.Apply(GoTypeNameVisitor.Ins)}
                    if v3, err2 = {elementType.Apply(this, "_buf2")}; err2 != nil {{return}}
                    _v2 = append(_v2, v3)
                }}
                return
                }}({bufName})";
        }

        public string Accept(TArray type, string bufName)
        {
            return GenList(type.ElementType, bufName);
        }

        public string Accept(TList type, string bufName)
        {
            return GenList(type.ElementType, bufName);
        }

        public string Accept(TSet type, string bufName)
        {
            return GenList(type.ElementType, bufName);
        }

        public string Accept(TMap type, string bufName)
        {
            return $@"func (_buf2 *serialization.ByteBuf) (_v2 {type.Apply(GoTypeNameVisitor.Ins)}, err2 error) {{
                _v2 = make({type.Apply(GoTypeNameVisitor.Ins)})
                var n int
                if n, err2 = _buf2.ReadSize(); err2 != nil {{return}}
                for i := 0 ; i < n ; i++ {{
                    var _key {type.KeyType.Apply(GoTypeNameVisitor.Ins)}
                    if _key, err2 = {type.KeyType.Apply(this, "_buf2")}; err2 != nil {{return}}
                    var _value {type.ValueType.Apply(GoTypeNameVisitor.Ins)}
                    if _value, err2 = {type.ValueType.Apply(this, "_buf2")}; err2 != nil {{return}}
                    _v2[_key] = _value
                }}
                return
                }}({bufName})";
        }

        public string Accept(TVector2 type, string bufName)
        {
            return $"{bufName}.ReadVector2()";
        }

        public string Accept(TVector3 type, string bufName)
        {
            return $"{bufName}.ReadVector3()";
        }

        public string Accept(TVector4 type, string bufName)
        {
            return $"{bufName}.ReadVector4()";
        }

        public string Accept(TDateTime type, string bufName)
        {
            return $"{bufName}.ReadInt()";
        }
    }
}
