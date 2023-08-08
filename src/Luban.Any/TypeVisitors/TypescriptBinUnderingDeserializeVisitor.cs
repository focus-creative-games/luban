namespace Luban.Any.TypeVisitors;

public class TypescriptBinUnderingDeserializeVisitor : TypescriptBinUnderingDeserializeVisitorBase
{
    public static TypescriptBinUnderingDeserializeVisitor Ins { get; } = new();

    public override string Accept(TBean type, string bufVarName, string fieldName)
    {
        if (type.Bean.IsAbstractType)
        {
            return $"{fieldName} = {type.Bean.FullName}.deserializeFrom({bufVarName})";
        }
        else
        {
            return $"{fieldName} = new {type.Bean.FullName}(); {fieldName}.deserialize({bufVarName})";
        }
    }
}