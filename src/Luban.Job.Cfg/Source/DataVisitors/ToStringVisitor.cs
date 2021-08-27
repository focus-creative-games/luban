using Luban.Job.Cfg.Datas;
using System.Collections.Generic;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToStringVisitor : IDataActionVisitor<StringBuilder>
    {
        public static ToStringVisitor Ins { get; } = new ToStringVisitor();

        public void Accept(DBool type, StringBuilder x)
        {
            x.Append(type.Value ? "true" : "false");
        }

        public void Accept(DByte type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DShort type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DFshort type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DInt type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DFint type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DLong type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DFlong type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DFloat type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DDouble type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DEnum type, StringBuilder x)
        {
            x.Append(type.StrValue);
        }

        public void Accept(DString type, StringBuilder x)
        {
            x.Append(type.Value);
        }

        public void Accept(DBytes type, StringBuilder x)
        {
            x.Append(Bright.Common.StringUtil.ArrayToString(type.Value));
        }

        public void Accept(DText type, StringBuilder x)
        {
            x.Append(type.Key).Append('|').Append(type.RawValue);
        }

        public void Accept(DBean type, StringBuilder x)
        {
            if (type.ImplType == null)
            {
                x.Append("null");
                return;
            }
            x.Append(type.ImplType.FullName).Append('{');
            int index = 0;
            foreach (var f in type.Fields)
            {
                var defField = type.ImplType.HierarchyFields[index++];
                x.Append(defField.Name).Append(':');
                if (f != null)
                {
                    f.Apply(this, x);
                }
                else
                {
                    x.Append("null");
                }
                x.Append(',');
            }
            x.Append('}');
        }


        private void Append(List<DType> datas, StringBuilder x)
        {
            x.Append('{');
            foreach (var e in datas)
            {
                e.Apply(this, x);
                x.Append(',');
            }
            x.Append('}');
        }

        public void Accept(DArray type, StringBuilder x)
        {
            Append(type.Datas, x);
        }

        public void Accept(DList type, StringBuilder x)
        {
            Append(type.Datas, x);
        }

        public void Accept(DSet type, StringBuilder x)
        {
            Append(type.Datas, x);
        }

        public void Accept(DMap type, StringBuilder x)
        {
            x.Append('{');
            foreach (var e in type.Datas)
            {
                e.Key.Apply(this, x);
                x.Append(':');
                e.Value.Apply(this, x);
                x.Append(',');
            }
            x.Append('}');
        }

        public void Accept(DVector2 type, StringBuilder x)
        {
            var v = type.Value;
            x.Append($"vector2(x={v.X},y={v.Y})");
        }

        public void Accept(DVector3 type, StringBuilder x)
        {
            var v = type.Value;
            x.Append($"vector3(x={v.X},y={v.Y},z={v.Z})");
        }

        public void Accept(DVector4 type, StringBuilder x)
        {
            var v = type.Value;
            x.Append($"vector4(x={v.X},y={v.Y},z={v.Z},w={v.W})");
        }

        public void Accept(DDateTime type, StringBuilder x)
        {
            x.Append(type.Time.ToString());
        }
    }
}
