using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoDeserializeUnderingVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static GoDeserializeUnderingVisitor Ins { get; } = new GoDeserializeUnderingVisitor();

        public string Accept(TBool type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadBool(); err != nil {{ return }} }}";
        }

        public string Accept(TByte type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadByte(); err != nil {{ return }} }}";
        }

        public string Accept(TShort type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadShort(); err != nil {{ return }} }}";
        }

        public string Accept(TFshort type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadFshort(); err != nil {{ return }} }}";
        }

        public string Accept(TInt type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadInt(); err != nil {{ return }} }}";
        }

        public string Accept(TFint type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadFint(); err != nil {{ return }} }}";
        }

        public string Accept(TLong type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadLong(); err != nil {{ return }} }}";
        }

        public string Accept(TFlong type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadFlong(); err != nil {{ return }} }}";
        }

        public string Accept(TFloat type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadFloat(); err != nil {{ return }} }}";
        }

        public string Accept(TDouble type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadDouble(); err != nil {{ return }} }}";
        }

        public string Accept(TEnum type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadInt(); err != nil {{ return }} }}";
        }

        public string Accept(TString type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadString(); err != nil {{ return }} }}";
        }

        public string Accept(TBytes type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadBytes(); err != nil {{ return }} }}";
        }

        public string Accept(TText type, string fieldName, string bufName)
        {
            return $"{{ if _, err = {bufName}.ReadString(); err != nil {{ return }}; if {fieldName}, err = {bufName}.ReadString(); err != nil {{ return }} }}";
        }

        public string Accept(TBean type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {($"New{type.Bean.GoFullName}({bufName})")}; err != nil {{ return }} }}";
        }


        private string GenList(TType elementType, string fieldName, string bufName)
        {
            return $@" {{
                {fieldName} = make([]{elementType.Apply(GoTypeNameVisitor.Ins)}, 0)
                var _n_ int
                if _n_, err = {bufName}.ReadSize(); err != nil {{return}}
                for i := 0 ; i < _n_ ; i++ {{
                    var _e_ {elementType.Apply(GoTypeNameVisitor.Ins)}
                    {elementType.Apply(GoDeserializeBinVisitor.Ins, "_e_", bufName)}
                    {fieldName} = append({fieldName}, _e_)
                }}
            }}
";
        }

        public string Accept(TArray type, string fieldName, string bufName)
        {
            return GenList(type.ElementType, fieldName, bufName);
        }

        public string Accept(TList type, string fieldName, string bufName)
        {
            return GenList(type.ElementType, fieldName, bufName);
        }

        public string Accept(TSet type, string fieldName, string bufName)
        {
            return GenList(type.ElementType, fieldName, bufName);
        }

        public string Accept(TMap type, string fieldName, string bufName)
        {
            return $@"{{
                {fieldName} = make({type.Apply(GoTypeNameVisitor.Ins)})
                var _n_ int
                if _n_, err = {bufName}.ReadSize(); err != nil {{return}}
                for i := 0 ; i < _n_ ; i++ {{
                    var _key_ {type.KeyType.Apply(GoTypeNameVisitor.Ins)}
                    {type.KeyType.Apply(GoDeserializeBinVisitor.Ins, "_key_", bufName)}
                    var _value_ {type.ValueType.Apply(GoTypeNameVisitor.Ins)}
                    {type.ValueType.Apply(GoDeserializeBinVisitor.Ins, "_value_", bufName)}
                    {fieldName}[_key_] = _value_
                }}
                }}";
        }

        public string Accept(TVector2 type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadVector2(); err != nil {{ return }} }}";
        }

        public string Accept(TVector3 type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadVector3(); err != nil {{ return }} }}";
        }

        public string Accept(TVector4 type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadVector4(); err != nil {{ return }} }}";
        }

        public string Accept(TDateTime type, string fieldName, string bufName)
        {
            return $"{{ if {fieldName}, err = {bufName}.ReadInt(); err != nil {{ return }} }}";
        }
    }
}
