namespace Luban.Core.RawDefs;

public class ExternalTypeMapper
{
    public string Selector { get; set; }

    public string Language { get; set; }

    public string TargetTypeName { get; set; }

    public string CreateFunction { get; set; }
}

public class RawExternalType
{
    public string Name { get; set; }

    public string OriginTypeName { get; set; }

    public List<ExternalTypeMapper> Mappers { get; set; }
}