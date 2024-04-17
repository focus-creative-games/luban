using Google.Protobuf;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Protobuf.DataVisitors;

public class ProtobufWireTypeVisitor : ITypeFuncVisitor<WireFormat.WireType>
{
    public static ProtobufWireTypeVisitor Ins { get; } = new();

    public WireFormat.WireType Accept(TBool type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TByte type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TShort type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TInt type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TLong type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TFloat type)
    {
        return WireFormat.WireType.Fixed32;
    }

    public WireFormat.WireType Accept(TDouble type)
    {
        return WireFormat.WireType.Fixed64;
    }

    public WireFormat.WireType Accept(TEnum type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TString type)
    {
        return WireFormat.WireType.LengthDelimited;
    }

    public WireFormat.WireType Accept(TDateTime type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TBean type)
    {
        return WireFormat.WireType.LengthDelimited;
    }

    public WireFormat.WireType Accept(TArray type)
    {
        //return WireFormat.WireType.LengthDelimited;
        throw new Exception("not support multi-dimension array wire type");
    }

    public WireFormat.WireType Accept(TList type)
    {
        throw new Exception("not support multi-dimension list wire type");
    }

    public WireFormat.WireType Accept(TSet type)
    {
        throw new Exception("not support multi-dimension set wire type");
    }

    public WireFormat.WireType Accept(TMap type)
    {
        //return WireFormat.WireType.LengthDelimited;
        throw new Exception("not support multi-dimension set wire type");
    }
}
