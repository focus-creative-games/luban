namespace Luban.RawDefs;

public class EnumItem
{
    public string Name { get; set; }

    public string Alias { get; set; }

    public string Value { get; set; }

    public string Comment { get; set; }

    public Dictionary<string, string> Tags { get; set; }
}

public class RawEnum
{
    public string Namespace { get; set; }

    public string Name { get; set; }

    public string FullName => Namespace.Length > 0 ? Namespace + "." + Name : Name;

    public bool IsFlags { get; set; }

    public bool IsUniqueItemId { get; set; }

    public string Comment { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public List<EnumItem> Items { get; set; }

    public List<string> Groups { get; set; }

    public List<TypeMapper> TypeMappers { get; set; }
}
