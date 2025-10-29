// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
