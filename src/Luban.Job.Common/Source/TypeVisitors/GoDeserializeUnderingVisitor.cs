using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Common.TypeVisitors
{
    public class GoDeserializeUnderingVisitor : ITypeFuncVisitor<string, string, string, string>
    {
        public static GoDeserializeUnderingVisitor Ins { get; } = new();

        public string Accept(TBool type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadBool(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TByte type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadByte(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TShort type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadShort(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TFshort type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadFshort(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TInt type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadInt(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TFint type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadFint(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TLong type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadLong(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TFlong type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadFlong(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TFloat type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadFloat(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TDouble type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadDouble(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TEnum type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadInt(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TString type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadString(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TText type, string fieldName, string bufName, string err)
        {
            return $"{{ if _, {err} = {bufName}.ReadString(); {err} != nil {{ return }}; if {fieldName}, {err} = {bufName}.ReadString(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TBytes type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadBytes(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TVector2 type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadVector2(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TVector3 type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadVector3(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TVector4 type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadVector4(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TDateTime type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {bufName}.ReadLong(); {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        public string Accept(TBean type, string fieldName, string bufName, string err)
        {
            return $"{{ if {fieldName}, {err} = {($"Deserialize{type.Bean.GoFullName}({bufName})")}; {err} != nil {{ {err} = errors.New(\"{fieldName} error\"); return }} }}";
        }

        private string GenList(TType elementType, string fieldName, string bufName, string err)
        {
            return $@"{{{fieldName} = make([]{elementType.Apply(GoTypeNameVisitor.Ins)}, 0); var _n_ int; if _n_, {err} = {bufName}.ReadSize(); {err} != nil {{ {err} = errors.New(""{fieldName} error""); return}}; for i := 0 ; i < _n_ ; i++ {{ var _e_ {elementType.Apply(GoTypeNameVisitor.Ins)}; {elementType.Apply(GoDeserializeBinVisitor.Ins, "_e_", bufName, err)}; {fieldName} = append({fieldName}, _e_) }} }}";
        }

        public string Accept(TArray type, string fieldName, string bufName, string err)
        {
            return GenList(type.ElementType, fieldName, bufName, err);
        }

        public string Accept(TList type, string fieldName, string bufName, string err)
        {
            return GenList(type.ElementType, fieldName, bufName, err);
        }

        public string Accept(TSet type, string fieldName, string bufName, string err)
        {
            return GenList(type.ElementType, fieldName, bufName, err);
        }

        public string Accept(TMap type, string fieldName, string bufName, string err)
        {
            return $@"{{ {fieldName} = make({type.Apply(GoTypeNameVisitor.Ins)}); var _n_ int; if _n_, {err} = {bufName}.ReadSize(); {err} != nil {{ {err} = errors.New(""{fieldName} error""); return}}; for i := 0 ; i < _n_ ; i++ {{ var _key_ {type.KeyType.Apply(GoTypeNameVisitor.Ins)}; {type.KeyType.Apply(GoDeserializeBinVisitor.Ins, "_key_", bufName, err)}; var _value_ {type.ValueType.Apply(GoTypeNameVisitor.Ins)}; {type.ValueType.Apply(GoDeserializeBinVisitor.Ins, "_value_", bufName, err)}; {fieldName}[_key_] = _value_}} }}";
        }
    }
}
