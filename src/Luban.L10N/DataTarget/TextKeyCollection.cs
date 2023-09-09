namespace Luban.L10N.DataTarget;

public class TextKeyCollection
{
    private readonly HashSet<string> _keys = new();

    public void AddKey(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            _keys.Add(key);
        }
    }

    public IEnumerable<string> Keys => _keys;
}
