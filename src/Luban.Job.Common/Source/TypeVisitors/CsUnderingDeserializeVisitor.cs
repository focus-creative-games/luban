using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;

namespace Luban.Job.Common.TypeVisitors
{
    class CsUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
    {
        public static CsUnderingDeserializeVisitor Ins { get; } = new CsUnderingDeserializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadBool();";
        }

        public string Accept(TByte type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadByte();";
        }

        public string Accept(TShort type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadShort();";
        }

        public string Accept(TFshort type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadFshort();";
        }

        public string Accept(TInt type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadInt();";
        }

        public string Accept(TFint type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadFint();";
        }

        public string Accept(TLong type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadLong();";
        }

        public string Accept(TFlong type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadFlong();";
        }

        public string Accept(TFloat type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadFloat();";
        }

        public string Accept(TDouble type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadDouble();";
        }

        public string Accept(TEnum type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = ({ type.Apply(CsUnderingDefineTypeName.Ins)}){bufName}.ReadInt();";
        }

        public string Accept(TString type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadString();";
        }

        public string Accept(TBytes type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName} = {bufName}.ReadBytes();";
        }

        public string Accept(TText type, string bufName, string fieldName, int depth)
        {
            return $"{fieldName}{TText.L10N_FIELD_SUFFIX} = {bufName}.ReadString(); {fieldName} = {bufName}.ReadString();";
        }

        public string Accept(TBean type, string bufName, string fieldName, int depth)
        {
            string src = $"{type.Bean.FullName}.Deserialize{type.Bean.Name}({bufName})";
            return $"{fieldName} = {ExternalTypeUtil.CsCloneToExternal(type.Bean.FullName, src)};";
        }

        public string Accept(TArray type, string bufName, string fieldName, int depth)
        {
            string __n = $"__n{depth}";
            string __e = $"__e{depth}";
            string __index = $"__index{depth}";
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
            return $"{{int {__n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {typeStr};for(var {__index} = 0 ; {__index} < {__n} ; {__index}++) {{ {type.ElementType.Apply(CsDefineTypeName.Ins)} {__e};{type.ElementType.Apply(this, bufName, $"{__e}", depth + 1)} {fieldName}[{__index}] = {__e};}}}}";
        }

        public string Accept(TList type, string bufName, string fieldName, int depth)
        {
            string n = $"n{depth}";
            string _e = $"_e{depth}";
            string i = $"i{depth}";
            return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(CsDefineTypeName.Ins)}({n});for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.ElementType.Apply(CsDefineTypeName.Ins)} {_e};  {type.ElementType.Apply(this, bufName, $"{_e}", depth + 1)} {fieldName}.Add({_e});}}}}";
        }

        public string Accept(TSet type, string bufName, string fieldName, int depth)
        {
            string n = $"n{depth}";
            string _e = $"_e{depth}";
            string i = $"i{depth}";
            return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(CsDefineTypeName.Ins)}(/*{n} * 3 / 2*/);for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.ElementType.Apply(CsDefineTypeName.Ins)} {_e};  {type.ElementType.Apply(this, bufName, $"{_e}", +1)} {fieldName}.Add({_e});}}}}";
        }

        public string Accept(TMap type, string bufName, string fieldName, int depth)
        {
            string n = $"n{depth}";
            string _k = $"_k{depth}";
            string _v = $"_v{depth}";
            string i = $"i{depth}";
            return $"{{int {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(CsDefineTypeName.Ins)}({n} * 3 / 2);for(var {i} = 0 ; {i} < {n} ; {i}++) {{ {type.KeyType.Apply(CsDefineTypeName.Ins)} {_k};  {type.KeyType.Apply(this, bufName, _k, depth + 1)} {type.ValueType.Apply(CsDefineTypeName.Ins)} {_v};  {type.ValueType.Apply(this, bufName, _v, depth + 1)}     {fieldName}.Add({_k}, {_v});}}}}";
        }

        public static string VectorName => (DefAssemblyBase.IsUseUnityVectors ? "UnityVector" : "Vector");

        public string Accept(TVector2 type, string bufName, string fieldName, int depth)
        {
            string src = $"{bufName}.Read{VectorName}2()";
            return $"{fieldName} = {ExternalTypeUtil.CsCloneToExternal("vector2", src)};";
        }

        public string Accept(TVector3 type, string bufName, string fieldName, int depth)
        {
            string src = $"{bufName}.Read{VectorName}3()";
            return $"{fieldName} = {ExternalTypeUtil.CsCloneToExternal("vector3", src)};";
        }

        public string Accept(TVector4 type, string bufName, string fieldName, int depth)
        {
            string src = $"{bufName}.Read{VectorName}4()";
            return $"{fieldName} = {ExternalTypeUtil.CsCloneToExternal("vector4", src)};";
        }

        public string Accept(TDateTime type, string bufName, string fieldName, int depth)
        {
            string src = $"{bufName}.ReadLong()";
            return $"{fieldName} = {ExternalTypeUtil.CsCloneToExternal("datetime", src)};";
        }
    }
}
