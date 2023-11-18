namespace Luban.DataValidator.Builtin.Set;

public class StringDataSet
{
    private readonly HashSet<string> _values;
    public StringDataSet(string args)
    {
        _values = args.Split(',').ToHashSet();
    }

    public bool Contains(string value)
    {
        return _values.Contains(value);
    }
}
