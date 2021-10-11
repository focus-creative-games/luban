using System;
using System.IO;

namespace Luban.Job.Cfg.DataSources
{
    static class DataSourceFactory
    {
        public static readonly string[] validDataSourceSuffixes = new string[]
        {
            ".xlsx",
            ".xls",
            ".csv",
            ".xml",
            ".lua",
            ".json",
            ".yml",
            ".bin",
        };

        public static AbstractDataSource Create(string url, string sheetName, Stream stream)
        {
            try
            {
                string ext = url.Contains(".") ? Path.GetExtension(url)?.Substring(1) : url;
                AbstractDataSource source;
                switch (ext)
                {
                    case "csv":
                    case "xls":
                    case "xlsx": source = new Excel.ExcelDataSource(); break;
                    case "json": source = new Json.JsonDataSource(); break;
                    default: throw new Exception($"不支持的文件类型:{url}");
                }
                source.Load(url, sheetName, stream);
                return source;
            }
            catch (Exception e)
            {
                throw new Exception($"文件{url} 加载失败", e);
            }
        }
    }
}
