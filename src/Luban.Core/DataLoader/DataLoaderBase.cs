using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader;

public abstract class DataLoaderBase : IDataLoader
{
    public string RawUrl { get; protected set; }

    public abstract Record ReadOne(DefTable table, TBean type);

    public abstract List<Record> ReadMulti(DefTable table, TBean type);
    
    public abstract void Load(DefTable table, string rawUrl, string sheetName, Stream stream);
}