using Google.Protobuf;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Protobuf.DataVisitors;

public class ProtobufBinDataVisitor : IDataActionVisitor<CodedOutputStream>
{
    public static ProtobufBinDataVisitor Ins { get; } = new();

    public void Accept(DBool type, CodedOutputStream x)
    {
        x.WriteBool(type.Value);
    }

    public void Accept(DByte type, CodedOutputStream x)
    {
        x.WriteInt32(type.Value);
    }

    public void Accept(DShort type, CodedOutputStream x)
    {
        x.WriteInt32(type.Value);
    }

    public void Accept(DInt type, CodedOutputStream x)
    {
        x.WriteInt32(type.Value);
    }

    public void Accept(DLong type, CodedOutputStream x)
    {
        x.WriteInt64(type.Value);
    }

    public void Accept(DFloat type, CodedOutputStream x)
    {
        x.WriteFloat(type.Value);
    }

    public void Accept(DDouble type, CodedOutputStream x)
    {
        x.WriteDouble(type.Value);
    }

    public void Accept(DEnum type, CodedOutputStream x)
    {
        x.WriteInt32(type.Value);
    }

    public void Accept(DDateTime type, CodedOutputStream x)
    {
        x.WriteInt64(type.UnixTimeOfCurrentContext());
    }

    public void Accept(DString type, CodedOutputStream x)
    {
        x.WriteString(type.Value);
    }

    private MemoryStream AllocMemoryStream()
    {
        // TODO 优化
        return new MemoryStream();
    }

    private void FreeMemoryStream(MemoryStream cos)
    {
        cos.Seek(0, SeekOrigin.Begin);
    }

    private void WriteRawMessageWithoutLength(DBean type, CodedOutputStream temp)
    {
        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var field in type.Fields)
        {
            var defField = defFields[index++];
            if (!defField.NeedExport())
            {
                continue;
            }
            var fieldType = defField.CType;
            if (field == null)
            {
                continue;
            }
            WriteField(field, fieldType, defField.AutoId, temp);
        }
    }
    
    private void EnterScope(CodedOutputStream x, Action<CodedOutputStream> action)
    {
        var ms = AllocMemoryStream();
        var temp = new CodedOutputStream(ms);
        action(temp);
        temp.Flush();
        ms.Seek(0, SeekOrigin.Begin);
        var bs = ByteString.FromStream(ms);
        x.WriteBytes(bs);
        FreeMemoryStream(ms);
    }

    public void Accept(DBean type, CodedOutputStream x)
    {
        EnterScope(x, cos =>
        {
            DefBean bean = type.Type;
            if (bean.IsAbstractType)
            {
                cos.WriteTag(type.ImplType.AutoId, WireFormat.WireType.LengthDelimited);
                EnterScope(cos, cos2 => WriteRawMessageWithoutLength(type, cos2));
            }
            else
            {
                WriteRawMessageWithoutLength(type, cos);
            }
        });
    }

    public void Accept(DArray type, CodedOutputStream x)
    {
        throw new NotImplementedException();
    }

    private void WriteMultiDimension(List<DType> datas, int dimension, CodedOutputStream x)
    {
        if (dimension >= 2)
        {
            foreach (var data in datas)
            {
                x.WriteTag(1, WireFormat.WireType.LengthDelimited);

                EnterScope(x, cos =>
                {
                    if(data is DArray array)
                    {
                        WriteMultiDimension(array.Datas, dimension - 1, cos);
                    }
                    else if(data is DList list)
                    {
                        WriteMultiDimension(list.Datas, dimension - 1, cos);
                    }
                    else
                    {
                        throw new Exception(data + "may not support multi array");
                    }
                });
            }
        }
        else
        {
            x.WriteTag(1, WireFormat.WireType.LengthDelimited);
            var ms = AllocMemoryStream();
            var temp = new CodedOutputStream(ms);
            foreach (var data in datas)
            {
                data.Apply(this, temp);
            }
            temp.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            x.WriteBytes(ByteString.FromStream(ms));
            FreeMemoryStream(ms);
        }

    }

    void WriteField(DType field, TType fieldType, int autoId, CodedOutputStream temp)
    {
        switch (field)
        {
            case DArray arr:
            {
                var dimension = arr.Type.Dimension;
                if (dimension > 1)
                {
                    foreach (var data in arr.Datas)
                    {
                        temp.WriteTag(autoId, WireFormat.WireType.LengthDelimited);

                        EnterScope(temp, cos =>
                        {
                            var array = data as DArray;
                            WriteMultiDimension(array.Datas, dimension - 1, cos);
                        });
                    }
                }
                else
                {
                    WriteList(fieldType.ElementType, autoId, arr.Datas, temp);
                }
                break;
            }
            case DList list:
            {
                int dimension = 1;
                var elementType = fieldType.ElementType;

                while (elementType is TList listElement)
                {
                    dimension++;
                    elementType = listElement.ElementType;
                }
                if (dimension > 1)
                {
                    foreach (var data in list.Datas)
                    {
                        temp.WriteTag(autoId, WireFormat.WireType.LengthDelimited);

                        EnterScope(temp, cos =>
                        {
                            var array = data as DList;
                            WriteMultiDimension(array.Datas, dimension - 1, cos);
                        });
                    }
                }
                else
                {
                    WriteList(fieldType.ElementType, autoId, list.Datas, temp);
                }
                break;
            }
            case DSet set:
            {
                WriteList(fieldType.ElementType, autoId, set.Datas, temp);
                break;
            }
            case DMap map:
            {
                WriteMap(map, autoId, temp);
                break;
            }
            default:
            {
                temp.WriteTag(autoId, fieldType.Apply(ProtobufWireTypeVisitor.Ins));
                field.Apply(this, temp);
                break;
            }
        }
    }
    public void Accept(DList type, CodedOutputStream x)
    {
        throw new NotImplementedException();
    }

    public void Accept(DSet type, CodedOutputStream x)
    {
        throw new NotImplementedException();
    }
    private void WriteList(TType elementType, int fieldId, List<DType> datas, CodedOutputStream x)
    {

        if (elementType.Apply(IsProtobufPackedType.Ins))//基础类型
        {
            x.WriteTag(fieldId, WireFormat.WireType.LengthDelimited);
            var ms = AllocMemoryStream();
            var temp = new CodedOutputStream(ms);
            foreach (var data in datas)
            {
                data.Apply(this, temp);
            }
            temp.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            x.WriteBytes(ByteString.FromStream(ms));
            FreeMemoryStream(ms);
        }
        else//bean 类型
        {
            var eleWireType = elementType.Apply(ProtobufWireTypeVisitor.Ins);

            foreach (var data in datas)
            {
                x.WriteTag(fieldId, eleWireType);
                data.Apply(this, x);
            }
        }
    }
    private void WriteMap(DMap type, int fieldId, CodedOutputStream x)
    {
        var keyType = type.Type.KeyType;
        var valueType = type.Type.ValueType;
        var ms = AllocMemoryStream();
        foreach (var e in type.Datas)
        {
            x.WriteTag(fieldId, WireFormat.WireType.LengthDelimited);
            ms.Seek(0, SeekOrigin.Begin);
            var temp = new CodedOutputStream(ms);
            temp.WriteTag(1, keyType.Apply(ProtobufWireTypeVisitor.Ins));
            e.Key.Apply(this, temp);
            temp.WriteTag(2, valueType.Apply(ProtobufWireTypeVisitor.Ins));
            e.Value.Apply(this, temp);
            temp.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            x.WriteBytes(ByteString.FromStream(ms));

        }
        FreeMemoryStream(ms);
    }

    public void Accept(DMap type, CodedOutputStream x)
    {
        throw new NotSupportedException();
    }
}
