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

using Luban.Schema;

namespace Luban.Pipeline;

public class PipelineArguments
{
    public string Target { get; set; }

    public bool ForceLoadTableDatas { get; set; }

    public List<string> IncludeTags { get; set; }

    public List<string> ExcludeTags { get; set; }

    public List<string> CodeTargets { get; set; }

    public List<string> DataTargets { get; set; }

    public string SchemaCollector { get; set; }

    public LubanConfig Config { get; set; }

    public List<string> OutputTables { get; set; }

    public string TimeZone { get; set; }

    public Dictionary<string, object> CustomArgs { get; set; }

    public Dictionary<string, string> Variants { get; set; }
}
