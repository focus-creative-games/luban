using System.Text;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.Python.DataVisitors;

public class ToPythonLiteralVisitor : ToLiteralVisitorBase
{
    public static ToPythonLiteralVisitor Ins { get; } = new();

    public override string Accept(DBool type)
    {
        return type.Value ? "True" : "False";
    }

    public override string Accept(DBean type)
    {
        var x = new StringBuilder();
        var bean = type.ImplType;
        if (bean.IsAbstractType)
        {
            x.Append($"{{ \"_name\":\"{type.ImplType.Name}\",");
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
            x.Append('\"').Append(defField.Name).Append('\"').Append(':');
            x.Append(f.Apply(this));
            x.Append(',');
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
            if (index > 0)
            {
                x.Append(',');
            }
            ++index;
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
            if (index > 0)
            {
                x.Append(',');
            }
            ++index;
            x.Append(e.Key.Apply(this));
            x.Append(':');
            x.Append(e.Value.Apply(this));
        }
        x.Append('}');
        return x.ToString();
    }
}
