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

using System.Reflection;
using Luban.CustomBehaviour;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.DataLoader;

public class DataLoaderManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static DataLoaderManager Ins { get; } = new();

    public void Init()
    {

    }

    public void LoadDatas(GenerationContext ctx)
    {
        var tasks = ctx.Tables.Select(t => Task.Run(() => LoadTable(ctx, t))).ToArray();
        Task.WaitAll(tasks);
    }

    private void LoadTable(GenerationContext ctx, DefTable table)
    {
        string inputDataDir = GenerationContext.GetInputDataPath();
        var tasks = new List<Task<List<Record>>>();
        foreach (var inputFile in table.InputFiles)
        {
            s_logger.Trace("load table:{} file:{}", table.FullName, inputFile);
            var (actualFile, subAssetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(inputFile));
            var options = new Dictionary<string, string>();
            foreach (var atomFile in FileUtil.GetFileOrDirectory(inputDataDir, Path.Combine(inputDataDir, actualFile)))
            {
                s_logger.Trace("load table:{} atomfile:{}", table.FullName, atomFile);
                tasks.Add(Task.Run(() => LoadTableFile(table, atomFile, subAssetName, options)));
            }
        }

        var records = new List<Record>();
        foreach (var task in tasks)
        {
            records.AddRange(task.Result);
        }
        ctx.AddDataTable(table, records, null);
    }

    public List<Record> LoadTableFile(DefTable table, string file, string subAssetName, Dictionary<string, string> options)
    {
        try
        {
            s_logger.Trace("load table:{} file:{}", table.FullName, file);
            if (!File.Exists(file) && !Directory.Exists(file))
            {
                throw new Exception($"'{table.FullName}'的input文件或目录不存在: {file} ");
            }
            string loaderName = options.TryGetValue("loader", out var name) ? name : FileUtil.GetExtensionWithoutDot(file);
            var loader = CreateDataLoader(loaderName);
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            loader.Load(file, subAssetName, stream);
            if (IsMultiRecordFile(file, subAssetName))
            {
                return loader.ReadMulti(table.ValueTType);
            }
            return new List<Record> { loader.ReadOne(table.ValueTType) };
        }
        catch (DataCreateException e)
        {
            if (string.IsNullOrEmpty(e.OriginDataLocation))
            {
                e.OriginDataLocation = file;
            }
            throw;
        }
        catch (Exception e)
        {
            throw new Exception($"LoadTableFile fail. {file}", e);
        }
    }

    public List<Record> LoadTableFile(TBean valueType, string file, string subAssetName, Dictionary<string, string> options)
    {
        try
        {
            string loaderName = options.TryGetValue("loader", out var name) ? name : FileUtil.GetExtensionWithoutDot(file);
            var loader = CreateDataLoader(loaderName);
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            loader.Load(file, subAssetName, stream);
            if (IsMultiRecordFile(file, subAssetName))
            {
                return loader.ReadMulti(valueType);
            }
            return new List<Record> { loader.ReadOne(valueType) };
        }
        catch (DataCreateException e)
        {
            if (string.IsNullOrEmpty(e.OriginDataLocation))
            {
                e.OriginDataLocation = file;
            }
            throw;
        }
        catch (Exception e)
        {
            throw new Exception($"LoadTableFile fail. {file}", e);
        }
    }

    private static bool IsMultiRecordField(string sheet)
    {
        return !string.IsNullOrEmpty(sheet) && sheet.StartsWith("*");
    }

    private static bool IsMultiRecordFile(string file, string sheetOrFieldName)
    {
        return FileUtil.IsExcelFile(file) || IsMultiRecordField(sheetOrFieldName);
    }

    public IDataLoader CreateDataLoader(string loaderName)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IDataLoader, DataLoaderAttribute>(loaderName);
    }
}
