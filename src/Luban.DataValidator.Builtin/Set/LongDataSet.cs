namespace Luban.DataValidator.Builtin.Set;

public class LongDataSet
{
    private readonly HashSet<long> _values;

    public LongDataSet(string args)
    {
        _values = args.Split(',').Select(long.Parse).ToHashSet();
    }

    public LongDataSet(IEnumerable<long> args)
    {
        _values = args.ToHashSet();
    }

    public bool Contains(long value)
    {
        return _values.Contains(value);
    }
}
