using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
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

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFshort()";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadInt()";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFint()";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.{(type.IsBigInt ? "ReadLong" : "ReadLongAsNumber")}()";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFlong()";
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

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"{fieldName} = new Uint8Array({bufName}.ReadArrayBuffer())";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $" {bufName}.ReadString(); {fieldName} = {bufName}.ReadString()";
        }

        public abstract string Accept(TBean type, string bufVarName, string fieldName);



        public static string GetNewArray(TType elementType)
        {
            switch (elementType)
            {
                case TByte _: return "new Uint8Array()";
                case TShort _:
                case TFshort _: return "new Int16Array()";
                case TInt _:
                case TFint _: return "new Int32Array()";
                case TLong _:
                case TFlong _: return "new Int64Array()";
                case TFloat _: return "new Float32Array()";
                case TDouble _: return "new Float64Array()";
                default: return "[]";
            }
        }

        public static string GetNewArray(TArray arrayType, string size)
        {
            switch (arrayType.ElementType)
            {
                case TByte _: return $"new Uint8Array({size})";
                case TShort _:
                case TFshort _: return $"new Int16Array({size})";
                case TInt _:
                case TFint _: return $"new Int32Array({size})";
                case TLong _:
                case TFlong _: return $"new Int64Array({size})";
                case TFloat _: return $"new Float32Array({size})";
                case TDouble _: return $"new Float64Array({size})";
                default: return "[]";
            }
        }

        private bool IsRawArrayElementType(TType elementType)
        {
            switch (elementType)
            {
                case TByte _:
                case TShort _:
                case TFshort _:
                case TInt _:
                case TFint _:
                case TLong _:
                case TFlong _:
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

        public string Accept(TVector2 type, string bufVarName, string fieldName)
        {
            return $"{fieldName} = Vector2.deserializeFrom({bufVarName})";
        }

        public string Accept(TVector3 type, string bufVarName, string fieldName)
        {
            return $"{fieldName} = Vector3.deserializeFrom({bufVarName})";
        }

        public string Accept(TVector4 type, string bufVarName, string fieldName)
        {
            return $"{fieldName} = Vector4.deserializeFrom({bufVarName})";
        }

        public string Accept(TDateTime type, string bufVarName, string fieldName)
        {
            return $"{fieldName} = {bufVarName}.ReadLongAsNumber()";
        }
    }
}
