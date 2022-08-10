using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
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
            ".asset",
        };

        private static string GetSheetParserMode(Dictionary<string, string> options)
        {
            if (options != null && options.TryGetValue("parser_mode", out var modeStr))
            {
                return modeStr;
            }
            //options = DefAssembly.LocalAssebmly.Options;
            //if (options != null && options.TryGetValue("sheet.parser_mode", out modeStr))
            //{
            //    return modeStr;
            //}
            return "";
        }

        private static bool IsColumnMode(string mode)
        {
            if (string.IsNullOrEmpty(mode))
            {
                return true;
            }
            switch(mode.ToLowerInvariant())
            {
                case "column": return true;
                case "stream": return false;
                default: throw new Exception($"unknown parser_mode:{mode}");
            }
        }

        public static AbstractDataSource Create(string url, string sheetName, Dictionary<string, string> options, Stream stream)
        {
            try
            {
                string ext = url.Contains('.') ? Path.GetExtension(url)?[1..] : url;
                AbstractDataSource source;
                switch (ext)
                {
                    case "csv":
                    case "xls":
                    case "xlsx":
                    case "xlsm":
                    {
                        source = IsColumnMode(GetSheetParserMode(options)) ? new ExcelRowColumnDataSource() : new ExcelStreamDataSource();
                        break;
                    }
                    case "xml": source = new Xml.XmlDataSource(); break;
                    case "lua": source = new Lua.LuaDataSource(); break;
                    case "json": source = new Json.JsonDataSource(); break;
                    case "yml": source = new Yaml.YamlDataSource(); break;
                    case "asset": source = new UnityAsset.UnityAssetDataSource(); break;
                    default: throw new Exception($"不支持的文件类型:{url}");
                }
                source.Load(url, sheetName, stream);
                return source;
            }
            catch (DataCreateException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception($"文件{url} 加载失败", e);
            }
        }
    }
}
