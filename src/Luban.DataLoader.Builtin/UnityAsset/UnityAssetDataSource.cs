using Luban.Core.DataLoader;
using Luban.Core.Datas;
using Luban.Core.Defs;
using Luban.Core.Types;
using Luban.Core.Utils;
using Luban.DataLoader.Builtin.DataVisitors;
using YamlDotNet.RepresentationModel;

namespace Luban.DataLoader.Builtin.UnityAsset;

[DataLoader("asset")]
public class UnityAssetDataSource : DataLoaderBase
{
    private YamlNode _root;
    public override void Load(DefTable table, string rawUrl, string sheetOrFieldName, Stream stream)
    {
        var ys = new YamlStream();
        ys.Load(new StreamReader(stream));
        var rootNode = (YamlMappingNode)ys.Documents[0].RootNode;

        // unity asset 格式为 包含一个doc的 yaml文件
        // doc顶层为map，只包含一个字段，字段key为类型名。
        if (rootNode.Children.Count != 1)
        {
            throw new Exception($"asset doc 应该只包含一个顶层字段");
        }

        this._root = rootNode.First().Value;

        if (!string.IsNullOrEmpty(sheetOrFieldName))
        {
            if (sheetOrFieldName.StartsWith("*"))
            {
                sheetOrFieldName = sheetOrFieldName[1..];
            }
            if (!string.IsNullOrEmpty(sheetOrFieldName))
            {
                foreach (var subField in sheetOrFieldName.Split('.'))
                {
                    this._root = _root[new YamlScalarNode(subField)];
                }
            }
        }
    }

    public override List<Record> ReadMulti(DefTable table, TBean type)
    {
        var records = new List<Record>();
        foreach (var ele in (YamlSequenceNode)_root)
        {
            var rec = ReadRecord(ele, type);
            if (rec != null)
            {
                records.Add(rec);
            }
        }
        return records;
    }

    private static readonly YamlScalarNode s_tagNameNode = new(FieldNames.TAG_KEY);

    public override Record ReadOne(DefTable table, TBean type)
    {
        return ReadRecord(_root, type);
    }

    private Record ReadRecord(YamlNode yamlNode, TBean type)
    {
        string tagName;
        if (((YamlMappingNode)yamlNode).Children.TryGetValue(s_tagNameNode, out var tagNode))
        {
            tagName = (string)tagNode;
        }
        else
        {
            tagName = null;
        }
        if (DataUtil.IsIgnoreTag(tagName))
        {
            return null;
        }
        var data = (DBean)type.Apply(UnityAssetDataCreator.Ins, yamlNode, type.DefBean.Assembly);
        var tags = DataUtil.ParseTags(tagName);
        return new Record(data, RawUrl, tags);
    }
}