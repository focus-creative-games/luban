using Luban.Types;

namespace Luban.TypeVisitors;

public abstract class AllTrueVisitor : DecoratorFuncVisitor<bool>
{
    public override bool DoAccept(TType type)
    {
        return true;
    }
}
