using System.Xml.Linq;
using Luban.Core.DataLoader;
using Luban.Core.Datas;
using Luban.Core.Defs;
using Luban.Core.Types;
using Luban.Core.Utils;
using Luban.DataLoader.Builtin.DataVisitors;

namespace Luban.DataLoader.Builtin.Xml;

[DataLoader("xml")]
public class XmlDataSource : DataLoaderBase
{
    private XElement _doc;

    public override void Load(DefTable table, string rawUrl, string sheetName, Stream stream)
    {
        RawUrl = rawUrl;
        _doc = XElement.Load(stream);
    }

    public override List<Record> ReadMulti(DefTable table, TBean type)
    {
        throw new NotSupportedException();
    }

    public override Record ReadOne(DefTable table, TBean type)
    {
        string tagName = _doc.Element(FieldNames.TAG_KEY)?.Value;
        if (DataUtil.IsIgnoreTag(tagName))
        {
            return null;
        }
        var data = (DBean)type.Apply(XmlDataCreator.Ins, _doc, (DefAssembly)type.DefBean.Assembly);
        var tags = DataUtil.ParseTags(tagName);
        return new Record(data, RawUrl, tags);
    }
}