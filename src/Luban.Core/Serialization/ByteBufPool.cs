namespace Luban.Serialization;

public class ByteBufPool
{
    private readonly Stack<ByteBuf> _bufs = new();
    private readonly int _maxCacheNum;

    private readonly Action<ByteBuf> _freeAction;

    public ByteBufPool(int maxCacheNum)
    {
        _maxCacheNum = maxCacheNum;
        _freeAction = this.Free;
    }

    public ByteBuf Alloc(int? hintSize)
    {
        if (_bufs.TryPop(out var b))
        {
            return b;
        }
        else
        {
            return new ByteBuf(hintSize ?? 64, this._freeAction);
        }
    }

    public void Free(ByteBuf buf)
    {
        buf.Clear();
        if (_bufs.Count < _maxCacheNum)
        {
            _bufs.Push(buf);
        }
    }

    public Stack<ByteBuf> GetByteBufsForTest() => _bufs;
}
