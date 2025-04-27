using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataExporter.Builtin.Res;

public class ResDataVisitor : IDataActionVisitor<TType, List<ResourceInfo>>
{
    public const string ResTagName = "res";

    public static ResDataVisitor Ins { get; } = new();

    public void Accept(DBool type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DByte type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DShort type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DInt type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DLong type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DFloat type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DDouble type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DEnum type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DString type, TType x, List<ResourceInfo> y)
    {
        if (!string.IsNullOrEmpty(type.Value) && x.HasTag(ResTagName))
        {
            y.Add(new ResourceInfo() { Resource = type.Value, Tag = x.GetTag(ResTagName) });
        }
    }

    public void Accept(DDateTime type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DBean type, TType x, List<ResourceInfo> y)
    {
        var def = type.ImplType;
        if (def == null)
        {
            return;
        }
        int index = 0;
        foreach (DType fieldData in type.Fields)
        {
            if (fieldData == null)
            {
                continue;
            }
            var fieldDef = ((DefField)def.HierarchyFields[index++]).CType;
            fieldData.Apply(this, fieldDef, y);
        }
    }

    private void Accept(List<DType> datas, TType elementType, List<ResourceInfo> ress)
    {
        foreach (var e in datas)
        {
            if (e != null)
            {
                e.Apply(this, elementType, ress);
            }
        }
    }

    public void Accept(DArray type, TType x, List<ResourceInfo> y)
    {
        Accept(type.Datas, type.Type.ElementType, y);
    }

    public void Accept(DList type, TType x, List<ResourceInfo> y)
    {
        Accept(type.Datas, type.Type.ElementType, y);
    }

    public void Accept(DSet type, TType x, List<ResourceInfo> y)
    {
        Accept(type.Datas, type.Type.ElementType, y);
    }

    public void Accept(DMap type, TType x, List<ResourceInfo> y)
    {
        TMap mtype = (TMap)x;
        foreach (var (k, v) in type.DataMap)
        {
            k.Apply(this, mtype.KeyType, y);
            v.Apply(this, mtype.ValueType, y);
        }
    }
}
