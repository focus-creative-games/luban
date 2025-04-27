using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public static class DataParserFactory
{
    public const string FORMAT_TAG_NAME = "format";

    private readonly static IDataParser s_streamParser = new StreamParser();
    private readonly static IDataParser s_liteParser = new LiteParser();
    private readonly static IDataParser s_jsonParser = new JsonParser();
    private readonly static IDataParser s_luaParser = new LuaParser();

    public static IDataParser GetDefaultDataParser()
    {
        return s_streamParser;
    }

    public static IDataParser GetDataParser(string parserType)
    {
        if (string.IsNullOrEmpty(parserType))
        {
            return null;
        }
        return parserType switch
        {
            "" => s_streamParser,
            "stream" => s_streamParser,
            "lite" => s_liteParser,
            "json" => s_jsonParser,
            "lua" => s_luaParser,
            _ => throw new NotSupportedException($"Unsupported data parser type: {parserType}")
        };
    }
}
