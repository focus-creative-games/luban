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

public readonly struct TemporalByteBuf : IDisposable
{
    public ByteBuf Buf { get; }

    public TemporalByteBuf(ByteBuf buf)
    {
        Buf = buf;
    }

    public void Dispose()
    {
        ThreadLocalTemporalByteBufPool.Free(Buf);
    }
}

public class ThreadLocalTemporalByteBufPool
{

    [ThreadStatic]
    private static ByteBufPool s_pool;

    [ThreadStatic]
    private static ByteBuf s_fastBuf;

    public static int MaxPoolCacheNum { get; set; } = 100;

    private static ByteBufPool Pool => s_pool ??= new ByteBufPool(MaxPoolCacheNum);

    public static TemporalByteBuf GuardAlloc(int? hintSize)
    {
        return new TemporalByteBuf(Alloc(hintSize));
    }

    public static ByteBuf Alloc(int? hintSize)
    {
        var buf = s_fastBuf;
        if (buf != null)
        {
            s_fastBuf = null;
            return buf;
        }
        return Pool.Alloc(hintSize);
    }

    public static void Free(ByteBuf buf)
    {
        buf.Clear();
        if (s_fastBuf == null)
        {
            s_fastBuf = buf;
        }
        else
        {
            Pool.Free(buf);
        }
    }

    public static byte[] CopyDataThenFree(ByteBuf buf)
    {
        var bytes = buf.CopyData();
        Free(buf);
        return bytes;
    }

    #region unit test
    internal static void ResetForTest()
    {
        s_fastBuf = null;
        s_pool = null;
    }

    internal static ByteBuf GetFastBufForTest() => s_fastBuf;

    internal static Stack<ByteBuf> GetByteBufsForTest() => Pool.GetByteBufsForTest();
    #endregion
}
