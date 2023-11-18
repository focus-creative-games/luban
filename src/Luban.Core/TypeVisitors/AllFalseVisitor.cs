using Luban.Types;

namespace Luban.TypeVisitors;

public abstract class AllFalseVisitor : DecoratorFuncVisitor<bool>
{
    public override bool DoAccept(TType type)
    {
        return false;
    }
}
