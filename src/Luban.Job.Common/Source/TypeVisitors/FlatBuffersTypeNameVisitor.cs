using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.TypeVisitors
{
    public class FlatBuffersTypeNameVisitor : ITypeFuncVisitor<string>
    {
        public static FlatBuffersTypeNameVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "ubyte";
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
            return "float32";
        }

        public string Accept(TDouble type)
        {
            return "float64";
        }

        public string Accept(TEnum type)
        {
            return type.DefineEnum.FlatBuffersFullName;
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
            return "[ubyte]";
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
            return type.Bean.FlatBuffersFullName;
        }

        public string Accept(TArray type)
        {
            return $"[{type.ElementType.Apply(this)}]";
        }

        public string Accept(TList type)
        {
            return $"[{type.ElementType.Apply(this)}]";
        }

        public string Accept(TSet type)
        {
            return $"[{type.ElementType.Apply(this)}]";
        }

        public string Accept(TMap type)
        {
            return $"[KeyValue_{type.KeyType.Apply(this)}_{type.ValueType.Apply(this)}]";
        }
    }
}
