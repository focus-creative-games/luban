using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class JavaUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static JavaUnderingDeserializeVisitor Ins { get; } = new JavaUnderingDeserializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readBool();";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readByte();";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readShort();";
        }

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readFshort();";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readInt();";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readFint();";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readLong();";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readFlong();";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readFloat();";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readDouble();";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readInt();";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readString();";
        }

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readBytes();";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $"{bufName}.readString(); {fieldName} = {bufName}.readString();";
        }

        public string Accept(TBean type, string bufName, string fieldName)
        {
            if (type.IsDynamic)
            {
                return $"{fieldName} = {type.Bean.FullNameWithTopModule}.deserialize{type.Bean.Name}({bufName});";
            }
            else
            {
                return $"{fieldName} = new {type.Bean.FullNameWithTopModule}({bufName});";
            }
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.ElementType.Apply(JavaDefineTypeName.Ins)}[n];for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}[i] = _e;}}}}";
        }

        public string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{type.Apply(JavaDefineTypeName.Ins)} _tmp_{fieldName} = new java.util.ArrayList(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} _tmp_{fieldName}.add(_e);}} {fieldName} = Collections.unmodifiableList(_tmp_{fieldName}); }}";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{type.Apply(JavaDefineTypeName.Ins)} _tmp_{fieldName} = new java.util.HashSet(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} _tmp_{fieldName}.add(_e);}} {fieldName} = Collections.unmodifiableSet(_tmp_{fieldName}); }}";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{int n = Math.min({bufName}.readSize(), {bufName}.size());{type.Apply(JavaDefineTypeName.Ins)} _tmp_{fieldName} = new java.util.HashMap(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(JavaBoxDefineTypeName.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(JavaBoxDefineTypeName.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     _tmp_{fieldName}.put(_k, _v);}} {fieldName} = Collections.unmodifiableMap(_tmp_{fieldName}); }}";

        }

        public string Accept(TVector2 type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readVector2();";
        }

        public string Accept(TVector3 type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readVector3();";
        }

        public string Accept(TVector4 type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readVector4();";
        }

        public string Accept(TDateTime type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.readLong();";
        }
    }
}
