using Luban.Core.Types;

namespace Luban.Core.TypeVisitors;

public abstract class AllTrueVisitor : DecoratorFuncVisitor<bool>
{
    public override bool DoAccept(TType type)
    {
        return true;
    }
}