using System.Xml.Linq;

namespace Luban.Utils;

public static class XmlUtil
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static XElement Open(string xmlFile)
    {
        try
        {
            s_logger.Trace("open {xml}", xmlFile);
            return XElement.Load(xmlFile);
        }
        catch (Exception e)
        {
            throw new LoadXmlException($"打开定义文件:{xmlFile} 失败 --> {e.Message}");
        }
    }

    public static XElement Open(string xmlFile, byte[] content)
    {
        try
        {
            s_logger.Trace("open {xml}", xmlFile);
            return XElement.Load(new MemoryStream(content));
        }
        catch (Exception e)
        {
            throw new LoadXmlException($"打开定义文件:{xmlFile} 失败 --> {e.Message}");
        }
    }

    public static string GetRequiredAttribute(XElement ele, string key)
    {
        if (ele.Attribute(key) != null)
        {
            var value = ele.Attribute(key).Value.Trim();
            if (value.Length != 0)
            {
                return value;
            }
        }
        throw new ArgumentException($"ele:{ele} key {key} 为空或未定义");
    }

    public static string GetOptionalAttribute(XElement ele, string key)
    {
        return ele.Attribute(key)?.Value ?? "";
    }

    public static bool GetOptionBoolAttribute(XElement ele, string key, bool defaultValue = false)
    {
        var attr = ele.Attribute(key)?.Value?.ToLower();
        if (attr == null)
        {
            return defaultValue;
        }
        return attr == "1" || attr == "true";
    }

    public static int GetOptionIntAttribute(XElement ele, string key, int defaultValue = 0)
    {
        if (ele.Attribute(key) == null)
        {
            return defaultValue;
        }
        return int.Parse(ele.Attribute(key).Value);
    }

    public static int GetRequiredIntAttribute(XElement ele, string key)
    {
        var attr = ele.Attribute(key);
        try
        {
            return int.Parse(attr.Value);
        }
        catch (Exception)
        {
            throw new FormatException($"{ele} 属性:{key}=>{attr?.Value} 不是整数");
        }
    }

    public static XElement OpenRelate(string relatePath, string toOpenXmlFile)
    {
        return Open(FileUtil.Combine(relatePath, toOpenXmlFile));
    }
}
