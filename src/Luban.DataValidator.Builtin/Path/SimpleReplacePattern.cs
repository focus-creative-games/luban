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

namespace Luban.DataValidator.Builtin.Path;

class SimpleReplacePattern : IPathPattern
{
    private readonly string _prefix;
    private readonly string _suffix;

    public bool EmptyAble { get; set; }

    public string Mode => "normal";

    public SimpleReplacePattern(string prefix, string suffix)
    {
        _prefix = prefix;
        _suffix = suffix;
    }

    public bool ExistPath(string rootDir, string subFile)
    {
        string finalPath = System.IO.Path.Combine(rootDir, _prefix + subFile + _suffix);
        return File.Exists(finalPath);
    }
}
