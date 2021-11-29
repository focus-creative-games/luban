using Google.Protobuf;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.TypeVisitors
{
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

        public WireFormat.WireType Accept(TFshort type)
        {
            return WireFormat.WireType.Varint;
        }

        public WireFormat.WireType Accept(TInt type)
        {
            return WireFormat.WireType.Varint;
        }

        public WireFormat.WireType Accept(TFint type)
        {
            return WireFormat.WireType.Fixed32;
        }

        public WireFormat.WireType Accept(TLong type)
        {
            return WireFormat.WireType.Varint;
        }

        public WireFormat.WireType Accept(TFlong type)
        {
            return WireFormat.WireType.Fixed64;
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

        public WireFormat.WireType Accept(TText type)
        {
            return WireFormat.WireType.LengthDelimited;
        }

        public WireFormat.WireType Accept(TBytes type)
        {
            return WireFormat.WireType.LengthDelimited;
        }

        public WireFormat.WireType Accept(TVector2 type)
        {
            return WireFormat.WireType.LengthDelimited;
        }

        public WireFormat.WireType Accept(TVector3 type)
        {
            return WireFormat.WireType.LengthDelimited;
        }

        public WireFormat.WireType Accept(TVector4 type)
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
            throw new System.NotSupportedException();
        }

        public WireFormat.WireType Accept(TList type)
        {
            throw new System.NotSupportedException();
        }

        public WireFormat.WireType Accept(TSet type)
        {
            throw new System.NotSupportedException();
        }

        public WireFormat.WireType Accept(TMap type)
        {
            return WireFormat.WireType.LengthDelimited;
        }
    }
}
