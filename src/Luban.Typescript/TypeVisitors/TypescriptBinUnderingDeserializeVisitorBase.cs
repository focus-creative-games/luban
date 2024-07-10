using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors
{
    public abstract class TypescriptBinUnderingDeserializeVisitorBase : ITypeFuncVisitor<string, string, string>
    {
        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadBool()";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadByte()";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadShort()";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadInt()";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.{(type.IsBigInt ? "ReadLong" : "ReadLongAsNumber")}()";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFloat()";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadDouble()";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadInt()";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadString()";
        }

        public abstract string Accept(TBean type, string bufVarName, string fieldName);



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
                default: return "[]";
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
                default: return $"new Array({size})";
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
                case TDouble _: return true;
                default: return false;
            }
        }

        public string Accept(TArray type, string bufVarName, string fieldName)
        {
            return $"{{ let n = Math.min({bufVarName}.ReadSize(), {bufVarName}.Size); {fieldName} = {GetNewArray(type, "n")}; for(let i = 0 ; i < n ; i++) {{ let _e :{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, "_e")}; { (IsRawArrayElementType(type.ElementType) ? $"{fieldName}[i] = _e" : $"{fieldName}.push(_e)") } }} }}";
        }

        public virtual string Accept(TList type, string bufVarName, string fieldName)
        {
            return $"{{ {fieldName} = []; for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _e :{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufVarName, "_e")}; {fieldName}.push(_e) }} }}";
        }

        public virtual string Accept(TSet type, string bufVarName, string fieldName)
        {
            return $"{{ {fieldName} = new {type.Apply(TypescriptDefineTypeNameVisitor.Ins)}(); for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _e:{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, "_e")}; {fieldName}.add(_e);}}}}";
        }

        public virtual string Accept(TMap type, string bufVarName, string fieldName)
        {
            return $"{{ {fieldName} = new {type.Apply(TypescriptDefineTypeNameVisitor.Ins)}(); for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _k:{type.KeyType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, bufVarName, "_k")}; let _v:{type.ValueType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, bufVarName, "_v")}; {fieldName}.set(_k, _v);  }} }}";
        }

        public string Accept(TDateTime type, string bufVarName, string fieldName)
        {
            return $"{fieldName} = {bufVarName}.ReadLongAsNumber()";
        }
    }
}
