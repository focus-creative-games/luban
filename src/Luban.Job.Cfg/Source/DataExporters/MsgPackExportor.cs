using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataExporters
{

    class MsgPackExportor
    {
        public static MsgPackExportor Ins { get; } = new();


        public void WriteList(DefTable table, List<Record> records, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(records.Count);
            foreach (var record in records)
            {
                Accept(record.Data, ref writer);
            }
        }

        public void Apply(DType type, ref MessagePackWriter writer)
        {
            switch (type)
            {
                case DInt x: Accept(x, ref writer); break;
                case DString x: Accept(x, ref writer); break;
                case DFloat x: Accept(x, ref writer); break;
                case DBean x: Accept(x, ref writer); break;
                case DBool x: Accept(x, ref writer); break;
                case DEnum x: Accept(x, ref writer); break;
                case DList x: Accept(x, ref writer); break;
                case DArray x: Accept(x, ref writer); break;
                case DLong x: Accept(x, ref writer); break;
                case DDateTime x: Accept(x, ref writer); break;
                case DMap x: Accept(x, ref writer); break;
                case DText x: Accept(x, ref writer); break;
                case DVector2 x: Accept(x, ref writer); break;
                case DVector3 x: Accept(x, ref writer); break;
                case DVector4 x: Accept(x, ref writer); break;
                case DByte x: Accept(x, ref writer); break;
                case DDouble x: Accept(x, ref writer); break;
                case DFint x: Accept(x, ref writer); break;
                case DFlong x: Accept(x, ref writer); break;
                case DFshort x: Accept(x, ref writer); break;
                case DSet x: Accept(x, ref writer); break;
                case DShort x: Accept(x, ref writer); break;
                default: throw new NotSupportedException($"DType:{type.GetType().FullName} not support");
            }
        }

        public void Accept(DBool type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DByte type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DShort type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DFshort type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DInt type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DFint type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DLong type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DFlong type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DFloat type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DDouble type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DEnum type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DString type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DText type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(2);
            writer.Write(type.Key);
            writer.Write(type.TextOfCurrentAssembly);
        }

        public void Accept(DBytes type, ref MessagePackWriter writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DVector2 type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(2);
            writer.Write(type.Value.X);
            writer.Write(type.Value.Y);
        }

        public void Accept(DVector3 type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(3);
            writer.Write(type.Value.X);
            writer.Write(type.Value.Y);
            writer.Write(type.Value.Z);
        }

        public void Accept(DVector4 type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(4);
            writer.Write(type.Value.X);
            writer.Write(type.Value.Y);
            writer.Write(type.Value.Z);
            writer.Write(type.Value.W);
        }

        public void Accept(DDateTime type, ref MessagePackWriter writer)
        {
            writer.Write(type.UnixTimeOfCurrentAssembly);
        }

        public void Accept(DBean type, ref MessagePackWriter writer)
        {
            var implType = type.ImplType;
            var hierarchyFields = implType.HierarchyFields;
            int exportCount = 0;
            {
                if (type.Type.IsAbstractType)
                {
                    exportCount++;
                }
                int idx = 0;
                foreach (var field in type.Fields)
                {
                    var defField = (DefField)hierarchyFields[idx++];
                    if (field == null || !defField.NeedExport)
                    {
                        continue;
                    }
                    ++exportCount;
                }
            }

            writer.WriteMapHeader(exportCount);
            if (type.Type.IsAbstractType)
            {
                writer.Write(DefBean.JSON_TYPE_NAME_KEY);
                writer.Write(DataUtil.GetImplTypeName(type));
            }

            int index = 0;
            foreach (var field in type.Fields)
            {
                var defField = (DefField)hierarchyFields[index++];
                if (field == null || !defField.NeedExport)
                {
                    continue;
                }
                writer.Write(defField.Name);
                Apply(field, ref writer);
            }
        }

        public void Accept(DArray type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d, ref writer);
            }
        }

        public void Accept(DList type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d, ref writer);
            }
        }

        public void Accept(DSet type, ref MessagePackWriter writer)
        {
            writer.WriteArrayHeader(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d, ref writer);
            }
        }

        public void Accept(DMap type, ref MessagePackWriter writer)
        {
            writer.WriteMapHeader(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d.Key, ref writer);
                Apply(d.Value, ref writer);
            }
        }
    }
}
