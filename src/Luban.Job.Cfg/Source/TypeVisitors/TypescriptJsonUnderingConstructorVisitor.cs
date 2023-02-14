using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class TypescriptJsonUnderingConstructorVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static TypescriptJsonUnderingConstructorVisitor Ins { get; } = new TypescriptJsonUnderingConstructorVisitor();

        public string Accept(TBool type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TByte type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TShort type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFshort type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TInt type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFint type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TLong type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFlong type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFloat type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TDouble type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TEnum type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TString type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TBytes type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TText type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}['{DText.TEXT_NAME}']; {fieldName}{TText.L10N_FIELD_SUFFIX} = {jsonVarName}['{DText.KEY_NAME}'];";
        }

        public string Accept(TBean type, string jsonVarName, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = {type.Bean.FullName}.constructorFrom({jsonVarName})";
            }
            else
            {
                return $"{fieldName} = new {type.Bean.FullName}({jsonVarName})";
            }
        }

        public string Accept(TArray type, string jsonVarName, string fieldName)
        {
            if (type.Apply(SimpleJsonTypeVisitor.Ins))
            {
                return $"{fieldName} = {jsonVarName}";
            }
            else
            {
                return $"{{ {fieldName} = []; for(let _ele of {jsonVarName}) {{ let _e :{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.push(_e);}}}}";
            }
        }

        public string Accept(TList type, string jsonVarName, string fieldName)
        {
            if (type.Apply(SimpleJsonTypeVisitor.Ins))
            {
                return $"{fieldName} = {jsonVarName}";
            }
            else
            {
                return $"{{ {fieldName} = []; for(let _ele of {jsonVarName}) {{ let _e : {type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.push(_e);}}}}";
            }
        }

        public string Accept(TSet type, string jsonVarName, string fieldName)
        {
            if (type.Apply(SimpleJsonTypeVisitor.Ins))
            {
                return $"{fieldName} = {jsonVarName}";
            }
            else
            {
                return $"{{ {fieldName} = new {type.Apply(TypescriptDefineTypeNameVisitor.Ins)}(); for(var _ele of {jsonVarName}) {{ let _e:{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.add(_e);}}}}";
            }
        }

        public string Accept(TMap type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = new {type.Apply(TypescriptDefineTypeNameVisitor.Ins)}(); for(var _entry_ of {jsonVarName}) {{ let _k:{type.KeyType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, "_entry_[0]", "_k")};  let _v:{type.ValueType.Apply(TypescriptDefineTypeNameVisitor.Ins)};  {type.ValueType.Apply(this, "_entry_[1]", "_v")}; {fieldName}.set(_k, _v);  }}";

        }

        public string Accept(TVector2 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector2.deserializeFromJson({jsonVarName})";
        }

        public string Accept(TVector3 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector3.deserializeFromJson({jsonVarName})";
        }

        public string Accept(TVector4 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector4.deserializeFromJson({jsonVarName})";
        }

        public string Accept(TDateTime type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }
    }
}
