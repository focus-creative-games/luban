using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsUnderingDefineTypeName : ITypeFuncVisitor<string>
    {
        public static CsUnderingDefineTypeName Ins { get; } = new CsUnderingDefineTypeName();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "byte";
        }

        public string Accept(TShort type)
        {
            return "short";
        }

        public string Accept(TFshort type)
        {
            return "short";
        }

        public string Accept(TInt type)
        {
            return "int";
        }

        public string Accept(TFint type)
        {
            return "int";
        }

        public string Accept(TLong type)
        {
            return "long";
        }

        public string Accept(TFlong type)
        {
            return "long";
        }

        public string Accept(TFloat type)
        {
            return "float";
        }

        public string Accept(TDouble type)
        {
            return "double";
        }

        public virtual string Accept(TEnum type)
        {
            return ExternalTypeUtil.CsMapperToExternalType(type.DefineEnum);
        }

        public string Accept(TString type)
        {
            return "string";
        }

        public string Accept(TBytes type)
        {
            return "byte[]";
        }

        public virtual string Accept(TText type)
        {
            return "string";
        }

        public string Accept(TBean type)
        {
            return ExternalTypeUtil.CsMapperToExternalType(type.Bean);
        }

        public string Accept(TArray type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public string Accept(TList type)
        {
            return $"{ConstStrings.CsList}<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TSet type)
        {
            return $"{ConstStrings.CsHashSet}<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TMap type)
        {
            return $"{ConstStrings.CsHashMap}<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }

        public string Accept(TVector2 type)
        {
            var mapper = ExternalTypeUtil.GetExternalTypeMappfer("vector2");
            if (mapper != null)
            {
                return mapper.TargetTypeName;
            }
            if (DefAssemblyBase.IsUseUnityVectors)
            {
                return "UnityEngine.Vector2";
            }
            return "System.Numerics.Vector2";
        }

        public string Accept(TVector3 type)
        {
            var mapper = ExternalTypeUtil.GetExternalTypeMappfer("vector3");
            if (mapper != null)
            {
                return mapper.TargetTypeName;
            }
            if (DefAssemblyBase.IsUseUnityVectors)
            {
                return "UnityEngine.Vector3";
            }
            return "System.Numerics.Vector3";
        }

        public string Accept(TVector4 type)
        {
            var mapper = ExternalTypeUtil.GetExternalTypeMappfer("vector4");
            if (mapper != null)
            {
                return mapper.TargetTypeName;
            }
            if (DefAssemblyBase.IsUseUnityVectors)
            {
                return "UnityEngine.Vector4";
            }
            return "System.Numerics.Vector4";
        }

        public virtual string Accept(TDateTime type)
        {
            var mapper = ExternalTypeUtil.GetExternalTypeMappfer("datetime");
            if (mapper != null)
            {
                return mapper.TargetTypeName;
            }
            return "long";
        }
    }
}
