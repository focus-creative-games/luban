using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader;

public interface IDataLoader
{
    string RawUrl { get; }

    Record ReadOne(DefTable table, TBean type);

    List<Record> ReadMulti(DefTable table, TBean type);

    void Load(DefTable table, string rawUrl, string subAsset, Stream stream);
}