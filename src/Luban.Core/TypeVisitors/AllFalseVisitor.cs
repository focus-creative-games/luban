using Luban.Core.Types;

namespace Luban.Core.TypeVisitors;

public abstract class AllFalseVisitor : DecoratorFuncVisitor<bool>
{
    public override bool DoAccept(TType type)
    {
        return false;
    }
}