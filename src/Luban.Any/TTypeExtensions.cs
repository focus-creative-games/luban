namespace Luban.Any;

static class TTypeExtensions
{
    public static string CsUnderingDefineType(this TType type)
    {
        return type.Apply(CsUnderingDefineTypeName.Ins);
    }
}