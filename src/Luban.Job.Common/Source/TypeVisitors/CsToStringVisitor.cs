using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsToStringVisitor : DecoratorFuncVisitor<string, string>
    {
        public static CsToStringVisitor Ins { get; } = new CsToStringVisitor();

        public override string DoAccept(TType type, string fieldName)
        {
            return fieldName;
        }

        public override string Accept(TArray type, string fieldName)
        {
            return $"Bright.Common.StringUtil.CollectionToString({fieldName})";
        }

        public override string Accept(TList type, string fieldName)
        {
            return $"Bright.Common.StringUtil.CollectionToString({fieldName})";
        }

        public override string Accept(TSet type, string fieldName)
        {
            return $"Bright.Common.StringUtil.CollectionToString({fieldName})";
        }

        public override string Accept(TMap type, string fieldName)
        {
            return $"Bright.Common.StringUtil.CollectionToString({fieldName})";
        }
    }
}
