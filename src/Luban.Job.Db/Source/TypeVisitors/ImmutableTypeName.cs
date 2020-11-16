using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class ImmutableTypeName : ITypeFuncVisitor<string>
    {
        public static ImmutableTypeName Ins { get; } = new ImmutableTypeName();

        public string Accept(TBool type)
        {
            return "";
        }

        public string Accept(TByte type)
        {
            return "";
        }

        public string Accept(TShort type)
        {
            return "";
        }

        public string Accept(TFshort type)
        {
            return "";
        }

        public string Accept(TInt type)
        {
            return "";
        }

        public string Accept(TFint type)
        {
            return "";
        }

        public string Accept(TLong type)
        {
            return "";
        }

        public string Accept(TFlong type)
        {
            return "";
        }

        public string Accept(TFloat type)
        {
            return "";
        }

        public string Accept(TDouble type)
        {
            return "";
        }

        public string Accept(TEnum type)
        {
            return "";
        }

        public string Accept(TString type)
        {
            return "";
        }

        public string Accept(TBytes type)
        {
            return "";
        }

        public string Accept(TText type)
        {
            return "";
        }

        public string Accept(TBean type)
        {
            return "";
        }

        public string Accept(TArray type)
        {
            return "";
        }

        public string Accept(TList type)
        {
            return $"System.Collections.Immutable.ImmutableList<{type.ElementType.DbCsDefineType()}>";
        }

        public string Accept(TSet type)
        {
            return $"System.Collections.Immutable.ImmutableHashSet<{type.ElementType.DbCsDefineType()}>";
        }

        public string Accept(TMap type)
        {
            return $"System.Collections.Immutable.ImmutableDictionary<{type.KeyType.DbCsDefineType()}, {type.ValueType.DbCsDefineType()}>";
        }

        public string Accept(TVector2 type)
        {
            return "";
        }

        public string Accept(TVector3 type)
        {
            return "";
        }

        public string Accept(TVector4 type)
        {
            return "";
        }

        public string Accept(TDateTime type)
        {
            return "";
        }
    }
}
