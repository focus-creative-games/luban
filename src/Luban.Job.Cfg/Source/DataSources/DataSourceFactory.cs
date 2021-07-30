using Luban.Job.Cfg.DataCreators;
using System;
using System.IO;

namespace Luban.Job.Cfg.DataSources
{
    static class DataSourceFactory
    {
        public static AbstractDataSource Create(string url, string sheetName, Stream stream, bool exportTestData)
        {
            try
            {
                string ext = url.Contains('.') ? Path.GetExtension(url)?[1..] : url;
                AbstractDataSource source;
                switch (ext)
                {
                    case "csv":
                    case "xls":
                    case "xlsx": source = new Excel.ExcelDataSource(); break;
                    case "xml": source = new Xml.XmlDataSource(); break;
                    case "lua": source = new Lua.LuaDataSource(); break;
                    case "json": source = new Json.JsonDataSource(); break;
                    case "b": source = new Binary.BinaryDataSource(); break;
                    default: throw new Exception($"不支持的文件类型:{url}");
                }
                source.Load(url, sheetName, stream, exportTestData);
                return source;
            }
            catch (DataCreateException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception($"文件{url} 加载失败 ==> {e.Message}", e);
            }
        }
    }
}
