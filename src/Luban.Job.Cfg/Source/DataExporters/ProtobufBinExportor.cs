﻿using Google.Protobuf;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataExporters
{
    class ProtobufBinExportor : IDataActionVisitor<CodedOutputStream>
    {
        public static ProtobufBinExportor Ins { get; } = new();

        public void WriteList(DefTable table, List<Record> datas, MemoryStream x)
        {
            var cos = new CodedOutputStream(x);
            foreach (var d in datas)
            {
                cos.WriteTag(1, WireFormat.WireType.LengthDelimited);
                d.Data.Apply(this, cos);
            }
            cos.Flush();
        }

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

        public void Accept(DFshort type, CodedOutputStream x)
        {
            x.WriteInt32(type.Value);
        }

        public void Accept(DInt type, CodedOutputStream x)
        {
            x.WriteInt32(type.Value);
        }

        public void Accept(DFint type, CodedOutputStream x)
        {
            x.WriteSFixed32(type.Value);
        }

        public void Accept(DLong type, CodedOutputStream x)
        {
            x.WriteInt64(type.Value);
        }

        public void Accept(DFlong type, CodedOutputStream x)
        {
            x.WriteSFixed64(type.Value);
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
            x.WriteInt64(type.UnixTimeOfCurrentAssembly);
        }

        public void Accept(DString type, CodedOutputStream x)
        {
            x.WriteString(type.Value);
        }

        public void Accept(DBytes type, CodedOutputStream x)
        {
            x.WriteBytes(ByteString.CopyFrom(type.Value));
        }

        public void Accept(DText type, CodedOutputStream x)
        {
            // 此处与 binary格式不同. binary格式还包含了key
            // 意味pb格式是无法支持动态本土化的。
            x.WriteString(type.TextOfCurrentAssembly);
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

        public void Accept(DVector2 type, CodedOutputStream x)
        {
            var ms = AllocMemoryStream();
            var temp = new CodedOutputStream(ms);
            temp.WriteTag(1, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.X);
            temp.WriteTag(2, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.Y);
            temp.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            x.WriteBytes(ByteString.FromStream(ms));
            FreeMemoryStream(ms);
        }

        public void Accept(DVector3 type, CodedOutputStream x)
        {
            var ms = AllocMemoryStream();
            var temp = new CodedOutputStream(ms);
            temp.WriteTag(1, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.X);
            temp.WriteTag(2, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.Y);
            temp.WriteTag(3, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.Z);
            temp.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            x.WriteBytes(ByteString.FromStream(ms));
            FreeMemoryStream(ms);
        }

        public void Accept(DVector4 type, CodedOutputStream x)
        {
            var ms = AllocMemoryStream();
            var temp = new CodedOutputStream(ms);
            temp.WriteTag(1, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.X);
            temp.WriteTag(2, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.Y);
            temp.WriteTag(3, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.Z);
            temp.WriteTag(4, WireFormat.WireType.Fixed32);
            temp.WriteFloat(type.Value.W);
            temp.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            x.WriteBytes(ByteString.FromStream(ms));
            FreeMemoryStream(ms);
        }

        private void WriteRawMessageWithoutLength(DBean type, CodedOutputStream temp)
        {
            //var ms = AllocMemoryStream();
            //var temp = new CodedOutputStream(ms);
            //if (bean.IsAbstractType)
            //{
            //    temp.WriteTag(type.ImplType.AutoId, WireFormat.WireType.LengthDelimited);
            //}

            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var field in type.Fields)
            {
                var defField = (DefField)defFields[index++];
                if (!defField.NeedExport)
                {
                    continue;
                }
                var fieldType = defField.CType;
                if (field == null)
                {
                    continue;
                }
                switch (field)
                {
                    case DArray arr:
                    {
                        WriteList(fieldType.ElementType, defField.AutoId, arr.Datas, temp);
                        break;
                    }
                    case DList list:
                    {
                        WriteList(fieldType.ElementType, defField.AutoId, list.Datas, temp);
                        break;
                    }
                    case DSet set:
                    {
                        WriteList(fieldType.ElementType, defField.AutoId, set.Datas, temp);
                        break;
                    }
                    case DMap map:
                    {
                        WriteMap(map, defField.AutoId, temp);
                        break;
                    }
                    default:
                    {
                        temp.WriteTag(defField.AutoId, defField.CType.Apply(ProtobufWireTypeVisitor.Ins));
                        field.Apply(this, temp);
                        break;
                    }
                }
            }
            //temp.Flush();
            //ms.Seek(0, SeekOrigin.Begin);
            //var bs = ByteString.FromStream(ms);
            //x.WriteBytes(bs);
            //FreeMemoryStream(ms);
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
                var bean = type.Type;

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

        private void WriteList(TType elementType, int fieldId, List<DType> datas, CodedOutputStream x)
        {
            if (elementType.Apply(IsProtobufPackedType.Ins))
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
            else
            {
                var eleWireType = elementType.Apply(ProtobufWireTypeVisitor.Ins);
                foreach (var data in datas)
                {
                    x.WriteTag(fieldId, eleWireType);
                    data.Apply(this, x);
                }
            }
        }

        public void Accept(DArray type, CodedOutputStream x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DList type, CodedOutputStream x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DSet type, CodedOutputStream x)
        {
            throw new NotImplementedException();
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
}
