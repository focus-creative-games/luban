// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.Excel;

[DataLoader("xls")]
[DataLoader("xlsx")]
[DataLoader("xlsm")]
[DataLoader("xlm")]
[DataLoader("csv")]
public class ExcelRowColumnDataSource : DataLoaderBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly List<RowColumnSheet> _sheets = new();


    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        s_logger.Trace("{} {}", rawUrl, sheetName);
        RawUrl = rawUrl;

        foreach (RawSheet rawSheet in SheetLoadUtil.LoadRawSheets(rawUrl, sheetName, stream))
        {
            var sheet = new RowColumnSheet(rawUrl, sheetName, rawSheet.SheetName);
            sheet.Load(rawSheet);
            _sheets.Add(sheet);
        }

        if (_sheets.Count == 0)
        {
            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                throw new Exception($"excel:‘{rawUrl}’ sheet:‘{sheetName}’ 不存在或者不是有效的单元簿(有效单元薄的A0单元格必须是##)");
            }
            else
            {
                throw new Exception($"excel: ‘{rawUrl}’ 不包含有效的单元薄(有效单元薄的A0单元格必须是##).");
            }
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
                foreach (var r in sheet.GetRows())
                {
                    TitleRow row = r.Row;
                    string tagStr = r.Tag;
                    if (DataUtil.IsIgnoreTag(tagStr))
                    {
                        continue;
                    }
                    var data = (DBean)type.Apply(SheetDataCreator.Ins, sheet, row);
                    datas.Add(new Record(data, sheet.UrlWithParams, DataUtil.ParseTags(tagStr)));
                }
            }
            catch (DataCreateException dce)
            {
                dce.OriginDataLocation = sheet.UrlWithParams;
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
