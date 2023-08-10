using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Python.TypeVisitors;

public class PyDefineTypeName : DecoratorFuncVisitor<string>
{
    public static PyDefineTypeName Ins { get; } = new();

    public override string DoAccept(TType type)
    {
        throw new System.NotSupportedException();
    }

    public override string Accept(TEnum type)
    {
        return type.DefEnum.FullName.Replace('.', '_');
    }

    public override string Accept(TBean type)
    {
        return type.DefBean.FullName.Replace('.', '_');
    }
}