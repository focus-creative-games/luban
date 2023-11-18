using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader;

public abstract class DataLoaderBase : IDataLoader
{
    public string RawUrl { get; protected set; }

    public abstract Record ReadOne(TBean type);

    public abstract List<Record> ReadMulti(TBean type);

    public abstract void Load(string rawUrl, string sheetName, Stream stream);
}
