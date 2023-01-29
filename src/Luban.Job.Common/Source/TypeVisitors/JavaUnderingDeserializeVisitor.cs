using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Common.TypeVisitors
{
    class JavaUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
    {
        public static JavaUnderingDeserializeVisitor Ins { get; } = new JavaUnderingDeserializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readBool();";
        }

        public string Accept(TByte type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readByte();";
        }

        public string Accept(TShort type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readShort();";
        }

        public string Accept(TFshort type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readFshort();";
        }

        public string Accept(TInt type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readInt();";
        }

        public string Accept(TFint type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readFint();";
        }

        public string Accept(TLong type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readLong();";
        }

        public string Accept(TFlong type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readFlong();";
        }

        public string Accept(TFloat type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readFloat();";
        }

        public string Accept(TDouble type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readDouble();";
        }

        public string Accept(TEnum type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readInt();";
        }

        public string Accept(TString type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readString();";
        }

        public string Accept(TBytes type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readBytes();";
        }

        public string Accept(TText type, string bufName, string fieldName, int depth)
        {
            return $"{bufName}.readString(); {fieldName} = {bufName}.readString();";
        }

        public string Accept(TBean type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {type.Bean.FullNameWithTopModule}.deserialize{type.Bean.Name}({bufName});";
        }

        public string Accept(TArray type, string bufName, string fieldName, int depth)
        {
            string __n = $"__n{depth}";
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __i = $"__i{depth}";
            string typeStr = $"{type.ElementType.Apply(CsDefineTypeName.Ins)}[{__n}]";
            if (type.Dimension > 1)
            {
                if (type.FinalElementType == null)
                {
                    throw new System.Exception("多维数组没有元素类型");
                }
                typeStr = $"{type.FinalElementType.Apply(CsUnderingDefineTypeName.Ins)}[{__n}]";
                for (int i = 0; i < type.Dimension - 1; i++)
                {
                    typeStr += "[]";
                }
            }
            return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.ElementType.Apply(JavaDefineTypeName.Ins)}[{__n}];for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} {__e};{type.ElementType.Apply(this, bufName, __e, depth + 1)} {fieldName}[{__i}] = {__e};}}}}";
        }

        public string Accept(TList type, string bufName, string fieldName, int depth)
        {
            string __n = $"__n{depth}";
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __i = $"__i{depth}";
            return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDefineTypeName.Ins)}({__n});for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} {__e};  {type.ElementType.Apply(this, bufName, __e, depth + 1)} {fieldName}.add({__e});}}}}";
        }

        public string Accept(TSet type, string bufName, string fieldName, int depth)
        {
            string __n = $"__n{depth}";
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __i = $"__i{depth}";
            return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDefineTypeName.Ins)}({__n} * 3 / 2);for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} {__e};  {type.ElementType.Apply(this, bufName, __e, depth + 1)} {fieldName}.add({__e});}}}}";
        }

        public string Accept(TMap type, string bufName, string fieldName, int depth)
        {
            string __n = $"__n{depth}";
            string __k = $"__k{depth}";
            string __v = $"__v{depth}";
            string __i = $"__i{depth}";
            return $"{{int {__n} = Math.min({bufName}.readSize(), {bufName}.size());{fieldName} = new {type.Apply(JavaDefineTypeName.Ins)}({__n} * 3 / 2);for(int {__i} = 0 ; {__i} < {__n} ; {__i}++) {{ {type.KeyType.Apply(JavaBoxDefineTypeName.Ins)} {__k};  {type.KeyType.Apply(this, bufName, __k, depth + 1)} {type.ValueType.Apply(JavaBoxDefineTypeName.Ins)} {__v};  {type.ValueType.Apply(this, bufName, __v, depth + 1)}     {fieldName}.put({__k}, {__v});}}}}";

        }

        public string Accept(TVector2 type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readVector2();";
        }

        public string Accept(TVector3 type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readVector3();";
        }

        public string Accept(TVector4 type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readVector4();";
        }

        public string Accept(TDateTime type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.readLong();";
        }
    }
}
