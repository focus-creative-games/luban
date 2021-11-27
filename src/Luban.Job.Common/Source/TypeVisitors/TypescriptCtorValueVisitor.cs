using Luban.Job.Common.Types;
using System;

namespace Luban.Job.Common.TypeVisitors
{
    public class TypescriptCtorValueVisitor : ITypeFuncVisitor<string>
    {
        public static TypescriptCtorValueVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "false";
        }

        public string Accept(TByte type)
        {
            return "0";
        }

        public string Accept(TShort type)
        {
            return "0";
        }

        public string Accept(TFshort type)
        {
            return "0";
        }

        public string Accept(TInt type)
        {
            return "0";
        }

        public string Accept(TFint type)
        {
            return "0";
        }

        public string Accept(TLong type)
        {
            return type.IsBean ? "BigInt(0)" : "0";
        }

        public string Accept(TFlong type)
        {
            return "BigInt(0)";
        }

        public string Accept(TFloat type)
        {
            return "0";
        }

        public string Accept(TDouble type)
        {
            return "0";
        }

        public string Accept(TEnum type)
        {
            return "0";
        }

        public string Accept(TString type)
        {
            return "\"\"";
        }

        public string Accept(TBytes type)
        {
            return "new Uint8Array()";
        }

        public string Accept(TText type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TBean type)
        {
            return type.IsDynamic ? "null" : $"new {type.Bean.FullName}()";
        }

        public string Accept(TArray type)
        {
            return TypescriptBinUnderingDeserializeVisitorBase.GetNewArray(type.ElementType);
        }

        public string Accept(TList type)
        {
            return "[]";
        }

        public string Accept(TSet type)
        {
            return "new Set()";
        }

        public string Accept(TMap type)
        {
            return "new Map()";
        }

        public string Accept(TVector2 type)
        {
            return "new Vector2(0,0)";
        }

        public string Accept(TVector3 type)
        {
            return "new Vector3(0,0,0)";
        }

        public string Accept(TVector4 type)
        {
            return "new Vector4(0,0,0,0)";
        }

        public string Accept(TDateTime type)
        {
            return "0";
        }
    }
}
