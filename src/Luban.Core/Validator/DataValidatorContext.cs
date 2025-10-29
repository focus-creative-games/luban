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

using Luban.Defs;
using Luban.Utils;

namespace Luban.Validator;

public class DataValidatorContext
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    [field: ThreadStatic]
    public static DataValidatorVisitor CurrentVisitor { get; set; }

    public static string CurrentRecordPath => TypeUtil.MakeFullName(CurrentVisitor.Path);

    public DefAssembly Assembly { get; }

    public DataValidatorContext(DefAssembly ass)
    {
        this.Assembly = ass;
    }

    public void ValidateTables(IEnumerable<DefTable> tables)
    {
        var tasks = new List<Task>();
        foreach (var t in tables)
        {
            tasks.Add(Task.Run(() =>
            {
                var records = GenerationContext.Current.GetTableAllDataList(t);
                var visitor = new DataValidatorVisitor(this);
                try
                {
                    CurrentVisitor = visitor;
                    visitor.ValidateTable(t, records);
                }
                finally
                {
                    CurrentVisitor = null;
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }
}
