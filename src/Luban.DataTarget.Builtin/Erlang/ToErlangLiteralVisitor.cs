using System.Text;
using Luban.Core.Datas;
using Luban.Core.DataVisitors;
using Luban.Core.Utils;

namespace Luban.DataExporter.Builtin.Erlang;

public class ToErlangLiteralVisitor : ToLiteralVisitorBase
{
    public static ToErlangLiteralVisitor Ins { get; } = new();

    public override string Accept(DBean type)
    {
        var x = new StringBuilder();
        if (type.Type.IsAbstractType)
        {
            x.Append($"#{{name__ => \"{DataUtil.GetImplTypeName(type)}\"");
            if (type.Fields.Count > 0)
            {
                x.Append(',');
            }
        }
        else
        {
            x.Append("#{");
        }

        int index = 0;
        foreach (var f in type.Fields)
        {
            var defField = type.ImplType.HierarchyFields[index++];
            if (f == null)
            {
                continue;
            }
            if (index > 1)
            {
                x.Append(',');
            }
            x.Append($"{defField.Name} => {f.Apply(this)}");
        }
        x.Append('}');
        return x.ToString();
    }


    protected void Append(List<DType> datas, StringBuilder x)
    {
        x.Append('[');
        int index = 0;
        foreach (var e in datas)
        {
            if (index++ > 0)
            {
                x.Append(',');
            }
            x.Append(e.Apply(this));
        }
        x.Append(']');
    }

    public override string Accept(DArray type)
    {
        var x = new StringBuilder();
        Append(type.Datas, x);
        return x.ToString();
    }

    public override string Accept(DList type)
    {
        var x = new StringBuilder();
        Append(type.Datas, x);
        return x.ToString();
    }

    public override string Accept(DSet type)
    {
        var x = new StringBuilder();
        Append(type.Datas, x);
        return x.ToString();
    }

    public override string Accept(DMap type)
    {
        var x = new StringBuilder();
        x.Append("#{");
        int index = 0;
        foreach (var e in type.Datas)
        {
            if (index++ > 0)
            {
                x.Append(',');
            }
            x.Append($"{e.Key.Apply(this)} => {e.Value.Apply(this)}");
        }
        x.Append('}');
        return x.ToString();
    }
}