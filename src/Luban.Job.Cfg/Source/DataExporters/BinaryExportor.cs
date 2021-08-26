using Bright.Serialization;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.TypeVisitors;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataExporters
{
    class BinaryExportor : IDataActionVisitor<DefAssembly, ByteBuf>
    {
        public static BinaryExportor Ins { get; } = new BinaryExportor();

        public void WriteList(List<Record> datas, DefAssembly ass, ByteBuf x)
        {
            x.WriteSize(datas.Count);
            foreach (var d in datas)
            {
                d.Data.Apply(this, ass, x);
            }
        }

        public void Accept(DBool type, DefAssembly ass, ByteBuf x)
        {
            x.WriteBool(type.Value);
        }

        public void Accept(DByte type, DefAssembly ass, ByteBuf x)
        {
            x.WriteByte(type.Value);
        }

        public void Accept(DShort type, DefAssembly ass, ByteBuf x)
        {
            x.WriteShort(type.Value);
        }

        public void Accept(DFshort type, DefAssembly ass, ByteBuf x)
        {
            x.WriteFshort(type.Value);
        }

        public void Accept(DInt type, DefAssembly ass, ByteBuf x)
        {
            x.WriteInt(type.Value);
        }

        public void Accept(DFint type, DefAssembly ass, ByteBuf x)
        {
            x.WriteFint(type.Value);
        }

        public void Accept(DLong type, DefAssembly ass, ByteBuf x)
        {
            x.WriteLong(type.Value);
        }

        public void Accept(DFlong type, DefAssembly ass, ByteBuf x)
        {
            x.WriteFlong(type.Value);
        }

        public void Accept(DFloat type, DefAssembly ass, ByteBuf x)
        {
            x.WriteFloat(type.Value);
        }

        public void Accept(DDouble type, DefAssembly ass, ByteBuf x)
        {
            x.WriteDouble(type.Value);
        }

        public void Accept(DEnum type, DefAssembly ass, ByteBuf x)
        {
            x.WriteInt(type.Value);
        }

        public void Accept(DString type, DefAssembly ass, ByteBuf x)
        {
            x.WriteString(type.Value);
        }

        public void Accept(DBytes type, DefAssembly ass, ByteBuf x)
        {
            x.WriteBytes(type.Value);
        }

        public void Accept(DText type, DefAssembly ass, ByteBuf x)
        {
            x.WriteString(type.Key);
            x.WriteString(type.GetText(ass.ExportTextTable, ass.NotConvertTextSet));
        }

        public void Accept(DBean type, DefAssembly ass, ByteBuf x)
        {
            var bean = type.Type;
            if (bean.IsAbstractType)
            {
                // 调整设计后，多态bean不会为空
                //if (type.ImplType == null)
                //{
                //    x.WriteInt(0);
                //    return;
                //}
                x.WriteInt(type.ImplType.Id);
            }
            int index = -1;
            foreach (var field in type.Fields)
            {
                ++index;
                var defField = (DefField)type.ImplType.HierarchyFields[index];
                if (!defField.NeedExport)
                {
                    continue;
                }
                if (defField.CType.IsNullable)
                {
                    if (field != null)
                    {
                        x.WriteBool(true);
                        field.Apply(this, ass, x);
                    }
                    else
                    {
                        x.WriteBool(false);
                    }
                }
                else
                {
                    field.Apply(this, ass, x);
                }
            }
        }

        public void WriteList(List<DType> datas, DefAssembly ass, ByteBuf x)
        {
            x.WriteSize(datas.Count);
            foreach (var d in datas)
            {
                d.Apply(this, ass, x);
            }
        }

        public void Accept(DArray type, DefAssembly ass, ByteBuf x)
        {
            WriteList(type.Datas, ass, x);
        }

        public void Accept(DList type, DefAssembly ass, ByteBuf x)
        {
            WriteList(type.Datas, ass, x);
        }

        public void Accept(DSet type, DefAssembly ass, ByteBuf x)
        {
            WriteList(type.Datas, ass, x);
        }

        public void Accept(DMap type, DefAssembly ass, ByteBuf x)
        {
            Dictionary<DType, DType> datas = type.Datas;
            x.WriteSize(datas.Count);
            foreach (var e in datas)
            {
                e.Key.Apply(this, ass, x);
                e.Value.Apply(this, ass, x);
            }
        }

        public void Accept(DVector2 type, DefAssembly ass, ByteBuf x)
        {
            x.WriteVector2(type.Value);
        }

        public void Accept(DVector3 type, DefAssembly ass, ByteBuf x)
        {
            x.WriteVector3(type.Value);
        }

        public void Accept(DVector4 type, DefAssembly ass, ByteBuf x)
        {
            x.WriteVector4(type.Value);
        }

        public void Accept(DDateTime type, DefAssembly ass, ByteBuf x)
        {
            x.WriteInt(type.GetUnixTime(ass.TimeZone));
        }
    }
}
