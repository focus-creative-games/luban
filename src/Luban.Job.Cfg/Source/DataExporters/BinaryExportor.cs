using Bright.Serialization;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataExporters
{
    class BinaryExportor : IDataActionVisitor<ByteBuf>
    {
        public static BinaryExportor Ins { get; } = new BinaryExportor();

        public void WriteList(DefTable table, List<Record> datas, ByteBuf x)
        {
            x.WriteSize(datas.Count);
            foreach (var d in datas)
            {
                d.Data.Apply(this, x);
            }
        }

        public void Accept(DBool type, ByteBuf x)
        {
            x.WriteBool(type.Value);
        }

        public void Accept(DByte type, ByteBuf x)
        {
            x.WriteByte(type.Value);
        }

        public void Accept(DShort type, ByteBuf x)
        {
            x.WriteShort(type.Value);
        }

        public void Accept(DFshort type, ByteBuf x)
        {
            x.WriteFshort(type.Value);
        }

        public void Accept(DInt type, ByteBuf x)
        {
            x.WriteInt(type.Value);
        }

        public void Accept(DFint type, ByteBuf x)
        {
            x.WriteFint(type.Value);
        }

        public void Accept(DLong type, ByteBuf x)
        {
            x.WriteLong(type.Value);
        }

        public void Accept(DFlong type, ByteBuf x)
        {
            x.WriteFlong(type.Value);
        }

        public void Accept(DFloat type, ByteBuf x)
        {
            x.WriteFloat(type.Value);
        }

        public void Accept(DDouble type, ByteBuf x)
        {
            x.WriteDouble(type.Value);
        }

        public void Accept(DEnum type, ByteBuf x)
        {
            x.WriteInt(type.Value);
        }

        public void Accept(DString type, ByteBuf x)
        {
            x.WriteString(type.Value);
        }

        public void Accept(DBytes type, ByteBuf x)
        {
            x.WriteBytes(type.Value);
        }

        public void Accept(DText type, ByteBuf x)
        {
            x.WriteString(type.Key);
            x.WriteString(type.TextOfCurrentAssembly);
        }

        public void Accept(DBean type, ByteBuf x)
        {
            var bean = type.Type;
            if (bean.IsAbstractType)
            {
                x.WriteInt(type.ImplType.Id);
            }

            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var field in type.Fields)
            {
                var defField = (DefField)defFields[index++];
                if (!defField.NeedExport)
                {
                    continue;
                }
                if (defField.CType.IsNullable)
                {
                    if (field != null)
                    {
                        x.WriteBool(true);
                        field.Apply(this, x);
                    }
                    else
                    {
                        x.WriteBool(false);
                    }
                }
                else
                {
                    field.Apply(this, x);
                }
            }
        }

        public void WriteList(List<DType> datas, ByteBuf x)
        {
            x.WriteSize(datas.Count);
            foreach (var d in datas)
            {
                d.Apply(this, x);
            }
        }

        public void Accept(DArray type, ByteBuf x)
        {
            WriteList(type.Datas, x);
        }

        public void Accept(DList type, ByteBuf x)
        {
            WriteList(type.Datas, x);
        }

        public void Accept(DSet type, ByteBuf x)
        {
            WriteList(type.Datas, x);
        }

        public void Accept(DMap type, ByteBuf x)
        {
            Dictionary<DType, DType> datas = type.Datas;
            x.WriteSize(datas.Count);
            foreach (var e in datas)
            {
                e.Key.Apply(this, x);
                e.Value.Apply(this, x);
            }
        }

        public void Accept(DVector2 type, ByteBuf x)
        {
            x.WriteVector2(type.Value);
        }

        public void Accept(DVector3 type, ByteBuf x)
        {
            x.WriteVector3(type.Value);
        }

        public void Accept(DVector4 type, ByteBuf x)
        {
            x.WriteVector4(type.Value);
        }

        public void Accept(DDateTime type, ByteBuf x)
        {
            x.WriteLong(type.UnixTimeOfCurrentAssembly);
        }
    }
}
