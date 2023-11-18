namespace Luban.RawDefs;

public class RawBean
{
    public string Namespace { get; set; }

    public string Name { get; set; }

    public string FullName => Namespace.Length > 0 ? Namespace + "." + Name : Name;

    public string Parent { get; set; }

    public bool IsValueType { get; set; }

    public string Comment { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public string Alias { get; set; }

    public string Sep { get; set; }

    public List<string> Groups { get; set; }

    public List<RawField> Fields { get; set; }

    public List<TypeMapper> TypeMappers { get; set; }
}
