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

﻿namespace Luban.Serialization;

// 把 int,long,string,bool 调整到最小
// 这样 marshal compatible write(field_id << tag_shift | tag_id) < 2^7 能在一个字节
// 内序列化, 优化序列化最终大小
#pragma warning disable CA1720 // 标识符包含类型名称
public static class FieldTag
{
    public const int
        INT = 0,
        LONG = 1,
        STRING = 2,
        BOOL = 3,

        BYTE = 4,
        SHORT = 5,
        FSHORT = 6,
        FINT = 7,
        FLONG = 8,
        FLOAT = 9,
        DOUBLE = 10,
        BYTES = 11,
        ARRAY = 12,
        LIST = 13,
        SET = 14,
        MAP = 15,
        BEAN = 16,
        TEXT = 17,
        VECTOR2 = 18,
        VECTOR3 = 19,
        VECTOR4 = 20,
        DYNAMIC_BEAN = 21,

        NOT_USE = 22;


    public const int TAG_SHIFT = 5;
    public const int TAG_MASK = (1 << TAG_SHIFT) - 1;
}
#pragma warning restore CA1720 // 标识符包含类型名称
