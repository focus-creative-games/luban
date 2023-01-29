using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class UeBpCppDefineTypeVisitor : ITypeFuncVisitor<string>
    {
        public static UeBpCppDefineTypeVisitor Ins { get; } = new UeBpCppDefineTypeVisitor();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "uint8";
        }

        public string Accept(TShort type)
        {
            return "int16";
        }

        public string Accept(TFshort type)
        {
            return "int16";
        }

        public string Accept(TInt type)
        {
            return "int32";
        }

        public string Accept(TFint type)
        {
            return "int32";
        }

        public string Accept(TLong type)
        {
            return "int64";
        }

        public string Accept(TFlong type)
        {
            return "int64";
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
            //return type.DefineEnum.UeBpFullName;
            throw new NotImplementedException();
        }

        public string Accept(TString type)
        {
            return "FString";
        }

        public string Accept(TBytes type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type)
        {
            return "FString";
        }

        public virtual string Accept(TBean type)
        {
            //return type.Bean.UeBpFullName + "*";
            throw new NotImplementedException();
        }

        public string Accept(TArray type)
        {
            return $"TArray<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TList type)
        {
            return $"TArray<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TSet type)
        {
            return $"TArray<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TMap type)
        {
            return $"TMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }

        public string Accept(TVector2 type)
        {
            return "FVector2D";
        }

        public string Accept(TVector3 type)
        {
            return "FVector";
        }

        public string Accept(TVector4 type)
        {
            return "FVector4";
        }

        public string Accept(TDateTime type)
        {
            return "int64";
        }
    }
}
