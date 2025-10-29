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

using Luban.CodeTarget;
using Luban.Defs;
using Luban.Utils;
using System.Reflection;
using System.Text;

namespace Luban.DataTarget;

public abstract class DataTargetBase : IDataTarget
{
    public const string FamilyPrefix = "tableExporter";

    public string Name => GetType().GetCustomAttribute<DataTargetAttribute>().Name;

    public virtual Encoding FileEncoding
    {
        get
        {
            string encoding = EnvManager.Current.GetOptionOrDefault(Name, BuiltinOptionNames.FileEncoding, true, "");
            return string.IsNullOrEmpty(encoding) ? Encoding.UTF8 : System.Text.Encoding.GetEncoding(encoding);
        }
    }

    public virtual AggregationType AggregationType => AggregationType.Table;

    public virtual bool ExportAllRecords => false;

    protected abstract string DefaultOutputFileExt { get; }

    protected string OutputFileExt { get; }

    protected DataTargetBase()
    {
        OutputFileExt = DefaultOutputFileExt;
        var dataTargetAttr = GetType().GetCustomAttribute<DataTargetAttribute>();
        if (dataTargetAttr == null)
        {
            return;
        }

        var namespaze = dataTargetAttr.Name;
        var optionName = BuiltinOptionNames.OutputDataExtension;
        if (EnvManager.Current.TryGetOption(namespaze, optionName, false, out var optionExt))
        {
            if (!string.IsNullOrWhiteSpace(optionExt))
            {
                OutputFileExt = optionExt;
            }
        }
    }

    public abstract OutputFile ExportTable(DefTable table, List<Record> records);

    public virtual OutputFile ExportTables(List<DefTable> tables)
    {
        throw new NotSupportedException();
    }

    public virtual OutputFile ExportRecord(DefTable table, Record record)
    {
        throw new NotSupportedException();
    }

    private static string GetLineEnding()
    {
        string endings = EnvManager.Current.GetOptionOrDefault("data", BuiltinOptionNames.LineEnding, true, "").ToLowerInvariant();
        return StringUtil.GetLineEnding(endings);
    }

    protected OutputFile CreateOutputFile(string path, string content)
    {
        string finalContent = content.ReplaceLineEndings(GetLineEnding());
        return new OutputFile() { File = path, Content = finalContent, Encoding = FileEncoding };
    }

    protected OutputFile CreateOutputFile(string path, byte[] content)
    {
        return new OutputFile() { File = path, Content = content, Encoding = FileEncoding };
    }
}
