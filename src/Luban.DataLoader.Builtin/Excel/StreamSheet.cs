namespace Luban.DataLoader.Builtin.Excel;

class StreamSheet
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public string Name { get; }

    public string RawUrl { get; }

    public ExcelStream Stream { get; private set; }

    public StreamSheet(string rawUrl, string name)
    {
        this.RawUrl = rawUrl;
        this.Name = name;
    }

    public void Load(RawSheet rawSheet)
    {
        Title title = rawSheet.Title;
        Stream = new ExcelStream(rawSheet.Cells, 1, title.ToIndex, "", "");
    }
}
