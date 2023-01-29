using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class JavaBoxDefineTypeName : JavaDefineTypeName
    {
        public static new JavaBoxDefineTypeName Ins { get; } = new JavaBoxDefineTypeName();

        public override string Accept(TBool type)
        {
            return "Boolean";
        }

        public override string Accept(TByte type)
        {
            return "Byte";
        }

        public override string Accept(TShort type)
        {
            return "Short";
        }

        public override string Accept(TFshort type)
        {
            return "Short";
        }

        public override string Accept(TInt type)
        {
            return "Integer";
        }

        public override string Accept(TFint type)
        {
            return "Integer";
        }

        public override string Accept(TLong type)
        {
            return "Long";
        }

        public override string Accept(TFlong type)
        {
            return "Long";
        }

        public override string Accept(TFloat type)
        {
            return "Float";
        }

        public override string Accept(TDouble type)
        {
            return "Double";
        }

        public override string Accept(TDateTime type)
        {
            return "Long";
        }

        public override string Accept(TEnum type)
        {
            //return type.DefineEnum.FullNameWithTopModule;
            return "Integer";
        }
    }
}
