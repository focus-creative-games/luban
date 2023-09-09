

using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;

namespace Luban.L10N.DataTarget;


/// <summary>
/// 检查 相同key的text,原始值必须相同
/// </summary>
public class TextKeyListCollectorVisitor : IDataActionVisitor2<TextKeyCollection>
{
    public static TextKeyListCollectorVisitor Ins { get; } = new TextKeyListCollectorVisitor();

    public void Accept(DBool data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DByte data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DShort data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DInt data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DLong data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DFloat data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DDouble data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DEnum data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DString data, TType type, TextKeyCollection x)
    {
        if (data != null && type.HasTag("text"))
        {
            x.AddKey(data.Value);
        }
    }

    public void Accept(DDateTime data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DBean data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DArray data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DList data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DSet data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DMap data, TType type, TextKeyCollection x)
    {

    }
}
