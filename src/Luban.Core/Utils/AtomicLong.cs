namespace Luban.Utils;

public class AtomicLong
{
    private long _value;
    public AtomicLong(long initValue = 0)
    {
        _value = initValue;
    }

    public long IncrementAndGet()
    {
        return Interlocked.Add(ref _value, 1);
    }

    public long AddAndGet(long step)
    {
        return Interlocked.Add(ref _value, step);
    }

    public long GetAndAdd(long step)
    {
        return Interlocked.Add(ref _value, step);
    }

    public long Value { get => Interlocked.Read(ref _value); set => Interlocked.Exchange(ref _value, value); }


    public override string ToString()
    {
        return _value.ToString();
    }
}
