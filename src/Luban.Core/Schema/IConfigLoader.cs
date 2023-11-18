namespace Luban.Schema;

public interface IConfigLoader
{
    LubanConfig Load(string fileName);
}
