using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class RustJsonUnderingConstructorVisitor : ITypeFuncVisitor<string, string>
    {
        public static RustJsonUnderingConstructorVisitor Ins { get; } = new();

        private static string AsType(string jsonVarName, string rawType)
        {
            return $"match {jsonVarName}.as_{rawType}() {{ Some(__x__) => __x__, None => return Err(LoadError{{}}) }}";
        }

        public string Accept(TBool type, string jsonVarName)
        {
            return AsType(jsonVarName, "bool");
        }

        public string Accept(TByte type, string jsonVarName)
        {
            return AsType(jsonVarName, "u8");
        }

        public string Accept(TShort type, string jsonVarName)
        {
            return AsType(jsonVarName, "i16");
        }

        public string Accept(TFshort type, string jsonVarName)
        {
            return AsType(jsonVarName, "i16");
        }

        public string Accept(TInt type, string jsonVarName)
        {
            return AsType(jsonVarName, "i32");
        }

        public string Accept(TFint type, string jsonVarName)
        {
            return AsType(jsonVarName, "i32");
        }

        public string Accept(TLong type, string jsonVarName)
        {
            return AsType(jsonVarName, "i64");
        }

        public string Accept(TFlong type, string jsonVarName)
        {
            return AsType(jsonVarName, "i64");
        }

        public string Accept(TFloat type, string jsonVarName)
        {
            return AsType(jsonVarName, "f32");
        }

        public string Accept(TDouble type, string jsonVarName)
        {
            return AsType(jsonVarName, "f64");
        }

        public string Accept(TEnum type, string jsonVarName)
        {
            return AsType(jsonVarName, "i32");
        }

        public string Accept(TString type, string jsonVarName)
        {
            return $"match {jsonVarName}.as_str() {{ Some(__x__) => __x__.to_string(), None => return Err(LoadError{{}}) }}";
        }

        public string Accept(TBytes type, string jsonVarName)
        {
            throw new System.NotSupportedException();
        }

        public string Accept(TText type, string jsonVarName)
        {
            return $"{{ if !{jsonVarName}[\"{DText.KEY_NAME}\"].is_string() {{ return Err(LoadError{{}}); }} match {jsonVarName}[\"{DText.TEXT_NAME}\"].as_str() {{ Some(__x__) => __x__.to_string(), None => return Err(LoadError{{}}) }} }}";
        }

        public string Accept(TBean type, string jsonVarName)
        {
            return $"{type.Bean.RustFullName}::new(&{jsonVarName})?";
        }

        public string Accept(TArray type, string jsonVarName)
        {
            return $"{{ if !{jsonVarName}.is_array() {{ return Err(LoadError{{}}); }} let mut __list__ = vec![]; for __e in {jsonVarName}.members() {{ __list__.push({type.ElementType.Apply(this, "__e")}); }}   __list__}}";
        }

        public string Accept(TList type, string jsonVarName)
        {
            return $"{{ if !{jsonVarName}.is_array() {{ return Err(LoadError{{}}); }} let mut __list__ = vec![]; for __e in {jsonVarName}.members() {{ __list__.push({type.ElementType.Apply(this, "__e")}); }}   __list__}}";
        }

        public string Accept(TSet type, string jsonVarName)
        {
            return $"{{ if !{jsonVarName}.is_array() {{ return Err(LoadError{{}}); }} let mut __set__ = std::collections::HashSet::new(); for __e in {jsonVarName}.members() {{ __set__.insert({type.ElementType.Apply(this, "__e")}); }}   __set__}}";
        }

        public string Accept(TMap type, string jsonVarName)
        {
            return $"{{ if !{jsonVarName}.is_array() {{ return Err(LoadError{{}}); }} let mut __map__ = std::collections::HashMap::new(); for __e in {jsonVarName}.members() {{ __map__.insert({type.KeyType.Apply(this, "__e[0]")}, {type.ValueType.Apply(this, "__e[1]")}); }}   __map__}}";
        }

        public string Accept(TVector2 type, string jsonVarName)
        {
            return $"Vector2::new(&{jsonVarName})?";
        }

        public string Accept(TVector3 type, string jsonVarName)
        {
            return $"Vector3::new(&{jsonVarName})?";
        }

        public string Accept(TVector4 type, string jsonVarName)
        {
            return $"Vector4::new(&{jsonVarName})?";
        }

        public string Accept(TDateTime type, string jsonVarName)
        {
            return AsType(jsonVarName, "i64");
        }
    }
}
