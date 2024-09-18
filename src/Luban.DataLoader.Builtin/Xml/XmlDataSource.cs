using System.Xml.Linq;
using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.Xml;

[DataLoader("xml")]
public class XmlDataSource : DataLoaderBase
{
    private XElement _doc;

    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        RawUrl = rawUrl;
        _doc = XElement.Load(stream);
    }

    public override List<Record> ReadMulti(TBean type)
    {
        throw new NotSupportedException();
    }

    public override Record ReadOne(TBean type)
    {
        string tagName = _doc.Element(FieldNames.TagKey)?.Value;
        if (DataUtil.IsIgnoreTag(tagName))
        {
            return null;
        }
        var data = (DBean)type.Apply(XmlDataCreator.Ins, _doc, type.DefBean.Assembly);
        var tags = DataUtil.ParseTags(tagName);
        return new Record(data, RawUrl, tags);
    }
}
