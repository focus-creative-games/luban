using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors
{
    public abstract class TypescriptBinUnderingDeserializeVisitorBase : ITypeFuncVisitor<string, string, int, string>
    {
        public string Accept(TBool type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadBool()";
        }

        public string Accept(TByte type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadByte()";
        }

        public string Accept(TShort type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadShort()";
        }

        public string Accept(TInt type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadInt()";
        }

        public string Accept(TLong type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.{(type.IsBigInt ? "ReadLong" : "ReadLongAsNumber")}()";
        }

        public string Accept(TFloat type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadFloat()";
        }

        public string Accept(TDouble type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadDouble()";
        }

        public string Accept(TEnum type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadInt()";
        }

        public string Accept(TString type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadString()";
        }

        public abstract string Accept(TBean type, string bufVarName, string fieldName, int depth);



        public static string GetNewArray(TType elementType)
        {
            switch (elementType)
            {
                case TByte _:
                case TShort _:
                case TInt _:
                case TLong _:
                case TFloat _:
                case TDouble _:
                default:
                    return "[]";
            }
        }

        public static string GetNewArray(TArray arrayType, string size)
        {
            switch (arrayType.ElementType)
            {
                case TByte _:
                case TShort _:
                case TInt _:
                case TLong _:
                case TFloat _:
                case TDouble _:
                default:
                    return $"new Array({size})";
            }
        }

        private bool IsRawArrayElementType(TType elementType)
        {
            switch (elementType)
            {
                case TByte _:
                case TShort _:
                case TInt _:
                case TLong _:
                case TFloat _:
                case TDouble _:
                    return true;
                default:
                    return false;
            }
        }

        public string Accept(TArray type, string bufVarName, string fieldName, int depth)
        {
            return $"{{ let n{depth} = Math.min({bufVarName}.ReadSize(), {bufVarName}.Size); {fieldName} = {GetNewArray(type, $"n{depth}")}; for(let i{depth} = 0 ; i{depth} < n{depth} ; i{depth}++) {{ let _e{depth} :{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, $"_e{depth}", depth + 1)}; {(IsRawArrayElementType(type.ElementType) ? $"{fieldName}[i{depth}] = _e{depth}" : $"{fieldName}[i{depth}] = _e{depth};")} }} }}";
        }

        public virtual string Accept(TList type, string bufVarName, string fieldName, int depth)
        {
            return $"{{ {fieldName} = []; for(let i{depth} = 0, n{depth} = {bufVarName}.ReadSize() ; i{depth} < n{depth} ; i{depth}++) {{ let _e{depth} :{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufVarName, $"_e{depth}", depth + 1)}; {fieldName}[i{depth}] = _e{depth} }} }}";
        }

        public virtual string Accept(TSet type, string bufVarName, string fieldName, int depth)
        {
            return $"{{ {fieldName} = new {type.Apply(TypescriptDefineTypeNameVisitor.Ins)}(); for(let i{depth} = 0, n{depth} = {bufVarName}.ReadSize() ; i{depth} < n{depth} ; i{depth}++) {{ let _e{depth}:{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, $"_e{depth}", depth + 1)}; {fieldName}.add(_e{depth});}} }}";
        }

        public virtual string Accept(TMap type, string bufVarName, string fieldName, int depth)
        {
            return $"{{ {fieldName} = new {type.Apply(TypescriptDefineTypeNameVisitor.Ins)}(); for(let i{depth} = 0, n{depth} = {bufVarName}.ReadSize() ; i{depth} < n{depth} ; i{depth}++) {{ let _k{depth}:{type.KeyType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, bufVarName, $"_k{depth}", depth + 1)}; let _v{depth}:{type.ValueType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, bufVarName, $"_v{depth}", depth + 1)}; {fieldName}.set(_k{depth}, _v{depth}); }} }}";
        }

        public string Accept(TDateTime type, string bufVarName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufVarName}.ReadLongAsNumber()";
        }
    }
}
