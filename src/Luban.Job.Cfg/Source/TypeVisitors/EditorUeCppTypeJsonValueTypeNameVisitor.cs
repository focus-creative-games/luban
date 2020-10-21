using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class EditorUeCppTypeJsonValueTypeNameVisitor : ITypeFuncVisitor<string>
    {
        public static EditorUeCppTypeJsonValueTypeNameVisitor Ins { get; } = new EditorUeCppTypeJsonValueTypeNameVisitor();

        public string Accept(TBool type)
        {
            return "FJsonValueBoolean";
        }

        public string Accept(TByte type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TShort type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TFshort type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TInt type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TFint type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TLong type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TFlong type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TFloat type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TDouble type)
        {
            return "FJsonValueNumber";
        }

        public string Accept(TEnum type)
        {
            return "FJsonValueString";
        }

        public string Accept(TString type)
        {
            return "FJsonValueString";
        }

        public string Accept(TBytes type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type)
        {
            return "FJsonValueString";
        }

        public string Accept(TBean type)
        {
            return "FJsonValueObject";
        }

        public string Accept(TArray type)
        {
            return "FJsonValueArray";
        }

        public string Accept(TList type)
        {
            return "FJsonValueArray";
        }

        public string Accept(TSet type)
        {
            return "FJsonValueArray";
        }

        public string Accept(TMap type)
        {
            return "FJsonValueObject";
        }

        public string Accept(TVector2 type)
        {
            return "FJsonValueObject";
        }

        public string Accept(TVector3 type)
        {
            return "FJsonValueObject";
        }

        public string Accept(TVector4 type)
        {
            return "FJsonValueObject";
        }

        public string Accept(TDateTime type)
        {
            return "FJsonValueNumber";
        }
    }
}
