
namespace Luban.Core.Plugin;

public interface IPlugin
{
    string Name { get; }

    string Location { get; set; }

    void Init(string jsonStr);

    void Start();
}