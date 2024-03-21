using Luban.DataLoader;
using Luban.DataLoader.Builtin;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Luban.DataExporter.Builtin.Yaml;

public class YamlDataVisitor : IDataFuncVisitor<YamlNode>
{
    public static YamlDataVisitor Ins { get; } = new();

    private static YamlScalarNode ToPlainNode(string x)
    {
        return new YamlScalarNode(x) { Style = ScalarStyle.Plain };
    }

    private static YamlScalarNode ToText(string x)
    {
        return new YamlScalarNode(x) { Style = ScalarStyle.SingleQuoted };
    }

    public YamlNode Accept(DBool type)
    {
        return ToPlainNode(type.Value ? "true" : "false");
    }

    public YamlNode Accept(DByte type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DShort type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DInt type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DLong type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DFloat type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DDouble type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DEnum type)
    {
        return ToPlainNode(type.Value.ToString());
    }

    public YamlNode Accept(DString type)
    {
        return ToText(type.Value);
    }

    public YamlNode Accept(DDateTime type)
    {
        return ToPlainNode(type.UnixTimeOfCurrentContext().ToString());
    }

    public YamlNode Accept(DBean type)
    {
        var m = new YamlMappingNode();

        if (type.Type.IsAbstractType)
        {
            m.Add(FieldNames.JsonTypeNameKey, ToText(DataUtil.GetImplTypeName(type)));
        }
        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var d in type.Fields)
        {
            var defField = (DefField)defFields[index++];

            // 特殊处理 bean 多态类型
            // 另外，不生成  xxx:null 这样
            if (d == null || !defField.NeedExport())
            {
                //x.WriteNullValue();
            }
            else
            {
                m.Add(defField.Name, d.Apply(this));
            }
        }

        return m;
    }

    public YamlSequenceNode ToSeqNode(List<DType> datas)
    {
        var seqNode = new YamlSequenceNode();
        foreach (var d in datas)
        {
            seqNode.Add(d.Apply(this));
        }
        return seqNode;
    }

    public YamlNode Accept(DArray type)
    {
        return ToSeqNode(type.Datas);
    }

    public YamlNode Accept(DList type)
    {
        return ToSeqNode(type.Datas);
    }

    public YamlNode Accept(DSet type)
    {
        return ToSeqNode(type.Datas);
    }

    public YamlNode Accept(DMap type)
    {
        var seqNode = new YamlSequenceNode();
        foreach (var d in type.Datas)
        {
            var e = new YamlSequenceNode();
            e.Add(d.Key.Apply(this));
            e.Add(d.Value.Apply(this));
            seqNode.Add(e);
        }
        return seqNode;
    }
}
