namespace Luban.DataLoader.Builtin.Excel;

public class RawSheet
{
    public Title Title { get; set; }

    public string TableName { get; set; }

    public List<List<Cell>> Cells { get; set; }
}