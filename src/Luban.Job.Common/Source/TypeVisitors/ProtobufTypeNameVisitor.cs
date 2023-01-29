using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.TypeVisitors
{
    public class ProtobufTypeNameVisitor : ITypeFuncVisitor<string>
    {
        public static ProtobufTypeNameVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "int32";
        }

        public string Accept(TShort type)
        {
            return "int32";
        }

        public string Accept(TFshort type)
        {
            return "int32";
        }

        public string Accept(TInt type)
        {
            return "int32";
        }

        public string Accept(TFint type)
        {
            return "sfixed32";
        }

        public string Accept(TLong type)
        {
            return "int64";
        }

        public string Accept(TFlong type)
        {
            return "sfixed64";
        }

        public string Accept(TFloat type)
        {
            return "float";
        }

        public string Accept(TDouble type)
        {
            return "double";
        }

        public string Accept(TEnum type)
        {
            return type.DefineEnum.PbFullName;
        }

        public string Accept(TString type)
        {
            return "string";
        }

        public string Accept(TText type)
        {
            return "string";
        }

        public string Accept(TBytes type)
        {
            return "bytes";
        }

        public string Accept(TVector2 type)
        {
            return "Vector2";
        }

        public string Accept(TVector3 type)
        {
            return "Vector3";
        }

        public string Accept(TVector4 type)
        {
            return "Vector4";
        }

        public string Accept(TDateTime type)
        {
            return "int64";
        }

        public string Accept(TBean type)
        {
            return type.Bean.PbFullName;
        }

        public string Accept(TArray type)
        {
            return $"{type.ElementType.Apply(this)}";
        }

        public string Accept(TList type)
        {
            return $"{type.ElementType.Apply(this)}";
        }

        public string Accept(TSet type)
        {
            return $"{type.ElementType.Apply(this)}";
        }

        public string Accept(TMap type)
        {
            string key = type.KeyType is TEnum ? "int32" : (type.KeyType.Apply(this));
            return $"map<{key}, {type.ValueType.Apply(this)}>";
        }
    }
}
