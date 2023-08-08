using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader.Builtin.Excel;

public class ExcelStreamDataSource : DataLoaderBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly List<StreamSheet> _sheets = new();


    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
        RawUrl = rawUrl;


        foreach (RawSheet rawSheet in SheetLoadUtil.LoadRawSheets(rawUrl, sheetName, stream))
        {
            var sheet = new StreamSheet(rawUrl, sheetName);
            sheet.Load(rawSheet);
            _sheets.Add(sheet);
        }

        if (_sheets.Count == 0)
        {
            throw new Exception($"excel:{rawUrl} 不包含有效的单元薄(有效单元薄的A0单元格必须是##).");
        }
    }

    public RawSheetTableDefInfo LoadTableDefInfo(string rawUrl, string sheetName, Stream stream)
    {
        return SheetLoadUtil.LoadSheetTableDefInfo(rawUrl, sheetName, stream);
    }

    public override List<Record> ReadMulti(TBean type)
    {
        var datas = new List<Record>();
        foreach (var sheet in _sheets)
        {
            try
            {
                var stream = sheet.Stream;
                while(!stream.TryReadEOF())
                {
                    var data = (DBean)type.Apply(ExcelStreamDataCreator.Ins, stream);
                    datas.Add(new Record(data, sheet.RawUrl, null));
                }
            }
            catch (DataCreateException dce)
            {
                dce.OriginDataLocation = sheet.RawUrl;
                throw;
            }
            catch (Exception e)
            {
                throw new Exception($"sheet:{sheet.Name}", e);
            }
        }
        return datas;
    }

    public override Record ReadOne(TBean type)
    {
        throw new Exception($"excel不支持单例读取模式");
    }
}