using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Lite;

[DataLoader("lit")]
public class LiteDataSource : DataLoaderBase
{
    private LiteStream _liteStream;

    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        var reader = new StreamReader(stream, Encoding.UTF8);
        _liteStream = new LiteStream(reader.ReadToEnd());
    }

    public override List<Record> ReadMulti(TBean type)
    {
        var records = new List<Record>();
        _liteStream.ReadStructOrCollectionBegin();
        while (_liteStream.IsEndOfStructOrCollection())
        {
            var record = ReadRecord(_liteStream, type);
            records.Add(record);
        }
        _liteStream.ReadStructOrCollectionEnd();
        return records;
    }

    public override Record ReadOne(TBean type)
    {
        return ReadRecord(_liteStream, type);
    }

    private Record ReadRecord(LiteStream stream, TBean type)
    {
        var data = (DBean)type.Apply(LiteStreamDataCreator.Ins, stream);
        return new Record(data, RawUrl, null);
    }
}
