namespace Luban.DataLoader.Builtin.Excel;

public class FieldInfo
{
    public string Name { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public string Type { get; set; }

    public string Desc { get; set; }
    public string Groups { get; set; }
}

public class RawSheetTableDefInfo
{
    public Dictionary<string, FieldInfo> FieldInfos { get; set; }
}
