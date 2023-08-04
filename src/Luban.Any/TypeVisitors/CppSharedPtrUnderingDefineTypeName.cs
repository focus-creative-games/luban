namespace Luban.Any.TypeVisitors;

public class CppSharedPtrUnderingDefineTypeName : CppRawUnderingDefineTypeName
{
    public static new CppSharedPtrUnderingDefineTypeName Ins { get; } = new();

    public override string Accept(TBean type)
    {
        return $"::bright::SharedPtr<{type.Bean.CppFullName}>";
    }
}