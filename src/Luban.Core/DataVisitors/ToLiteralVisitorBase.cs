using Luban.Datas;
using Luban.Utils;

namespace Luban.DataVisitors;

public abstract class ToLiteralVisitorBase : IDataFuncVisitor<string>
{
    public virtual string Accept(DBool type)
    {
        return type.Value ? "true" : "false";
    }

    public string Accept(DByte type)
    {
        return type.Value.ToString();
    }

    public string Accept(DShort type)
    {
        return type.Value.ToString();
    }

    public string Accept(DInt type)
    {
        return type.Value.ToString();
    }

    public string Accept(DLong type)
    {
        return type.Value.ToString();
    }

    public string Accept(DFloat type)
    {
        return type.Value.ToString();
    }

    public string Accept(DDouble type)
    {
        return type.Value.ToString();
    }

    public virtual string Accept(DEnum type)
    {
        return type.Value.ToString();
    }

    public virtual string Accept(DString type)
    {
        return "\"" + DataUtil.EscapeString(type.Value) + "\"";
    }

    public virtual string Accept(DDateTime type)
    {
        return type.UnixTimeOfCurrentContext().ToString();
    }

    public abstract string Accept(DBean type);

    public abstract string Accept(DArray type);

    public abstract string Accept(DList type);

    public abstract string Accept(DSet type);

    public abstract string Accept(DMap type);
}
