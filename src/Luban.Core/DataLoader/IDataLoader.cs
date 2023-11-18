using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader;

public interface IDataLoader
{
    string RawUrl { get; }

    Record ReadOne(TBean type);

    List<Record> ReadMulti(TBean type);

    void Load(string rawUrl, string subAsset, Stream stream);
}
