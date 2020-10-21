using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class PyDefineTypeName : DecoratorFuncVisitor<string>
    {
        public static PyDefineTypeName Ins { get; } = new PyDefineTypeName();

        public override string DoAccept(TType type)
        {
            throw new System.NotSupportedException();
        }

        public override string Accept(TEnum type)
        {
            return type.DefineEnum.PyFullName;
        }

        public override string Accept(TBean type)
        {
            return type.Bean.PyFullName;
        }

        public override string Accept(TVector2 type)
        {
            return "Vector2";
        }

        public override string Accept(TVector3 type)
        {
            return "Vector3";
        }

        public override string Accept(TVector4 type)
        {
            return "Vector4";
        }
    }
}
