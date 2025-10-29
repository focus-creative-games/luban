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

﻿#define CPU_SUPPORT_MEMORY_NOT_ALIGN  //CPU 是否支持读取非对齐内存




/// <summary>
/// TODO  
/// 1. 整理代码
/// 2. 优化序列化 (像这样 data[endPos + 1] = (byte)(x >> 8) 挨个字节赋值总感觉很低效，能优化吗)
/// </summary>


namespace Luban.Serialization;

public class SerializationException : Exception
{
    public SerializationException() { }
    public SerializationException(string msg) : base(msg) { }

    public SerializationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
