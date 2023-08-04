using System.Reflection;
using Luban.Core.Defs;
using Luban.Core.Types;

namespace Luban.Core.DataLoader;

public abstract class DataLoaderBase : IDataLoader
{
    public string RawUrl { get; protected set; }

    public abstract Record ReadOne(DefTable table, TBean type);

    public abstract List<Record> ReadMulti(DefTable table, TBean type);
    
    public abstract void Load(DefTable table, string rawUrl, string sheetName, Stream stream);
}