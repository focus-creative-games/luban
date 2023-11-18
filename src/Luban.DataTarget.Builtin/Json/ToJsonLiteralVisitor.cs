using System.Text;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Json;

public class ToJsonLiteralVisitor : ToLiteralVisitorBase
{
    public static ToJsonLiteralVisitor Ins { get; } = new();

    public override string Accept(DBean type)
    {
        var x = new StringBuilder();
        bool prevProperty = false;
        if (type.Type.IsAbstractType)
        {
            x.Append($"{{ \"_name\":\"{type.ImplType.Name}\"");
            prevProperty = true;
        }
        else
        {
            x.Append('{');
        }

        int index = 0;
        foreach (var f in type.Fields)
        {
            var defField = (DefField)type.ImplType.HierarchyFields[index++];
            if (f == null || !defField.NeedExport())
            {
                continue;
            }
            if (prevProperty)
            {
                x.Append(',');
            }
            else
            {
                prevProperty = true;
            }
            x.Append('\"').Append(defField.Name).Append('\"').Append(':');
            x.Append(f.Apply(this));
        }
        x.Append('}');
        return x.ToString();
    }


    protected virtual void Append(List<DType> datas, StringBuilder x)
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
        x.Append('{');
        int index = 0;
        foreach (var e in type.Datas)
        {
            if (index++ > 0)
            {
                x.Append(',');
            }
            x.Append('"').Append(e.Key.Apply(ToJsonPropertyNameVisitor.Ins)).Append('"');
            x.Append(':');
            x.Append(e.Value.Apply(this));
        }
        x.Append('}');
        return x.ToString();
    }
}
