using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System.Collections.Generic;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToJsonStringVisitor : IDataFuncVisitor<string>
    {
        public static ToJsonStringVisitor Ins { get; } = new();

        public string Accept(DBool type)
        {
            return type.Value ? "true" : "false";
        }

        public string Accept(DByte type)
        {
            return type.Value.ToString();
        }

        public string Accept(DShort type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFshort type)
        {
            return type.Value.ToString();
        }

        public string Accept(DInt type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFint type)
        {
            return type.Value.ToString();
        }

        public string Accept(DLong type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFlong type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFloat type)
        {
            return type.Value.ToString();
        }

        public string Accept(DDouble type)
        {
            return type.Value.ToString();
        }

        public string Accept(DEnum type)
        {
            return type.Value.ToString();
        }

        public string Accept(DString type)
        {
            return "\"" + DataUtil.EscapeString(type.Value) + "\"";
        }

        public string Accept(DBytes type)
        {
            throw new System.NotSupportedException();
        }

        public virtual string Accept(DText type)
        {
            var ass = DefAssembly.LocalAssebmly as DefAssembly;
            return $"{{\"{DText.KEY_NAME}\":\"{type.Key}\",\"{DText.TEXT_NAME}\":\"{DataUtil.EscapeString(type.GetText(ass.ExportTextTable, ass.NotConvertTextSet))}\"}}";
        }

        public virtual string Accept(DBean type)
        {
            var x = new StringBuilder();
            var bean = type.ImplType;
            if (bean.IsAbstractType)
            {
                x.Append($"{{ \"_name\":\"{type.ImplType.Name}\",");
            }
            else
            {
                x.Append('{');
            }

            int index = 0;
            foreach (var f in type.Fields)
            {
                if (index >= 1)
                {
                    x.Append(',');
                }
                var defField = type.ImplType.HierarchyFields[index++];
                x.Append('\"').Append(defField.Name).Append('\"').Append(':');
                if (f != null)
                {
                    x.Append(f.Apply(this));
                }
                else
                {
                    x.Append("null");
                }
            }
            x.Append('}');
            return x.ToString();
        }


        protected virtual void Append(List<DType> datas, StringBuilder x)
        {
            x.Append('[');
            int index = 0;
            foreach (var e in datas)
            {
                if (index > 0)
                {
                    x.Append(',');
                }
                ++index;
                x.Append(e.Apply(this));
            }
            x.Append(']');
        }

        public string Accept(DArray type)
        {
            var x = new StringBuilder();
            Append(type.Datas, x);
            return x.ToString();
        }

        public string Accept(DList type)
        {
            var x = new StringBuilder();
            Append(type.Datas, x);
            return x.ToString();
        }

        public string Accept(DSet type)
        {
            var x = new StringBuilder();
            Append(type.Datas, x);
            return x.ToString();
        }

        public virtual string Accept(DMap type)
        {
            var x = new StringBuilder();
            x.Append('{');
            int index = 0;
            foreach (var e in type.Datas)
            {
                if (index > 0)
                {
                    x.Append(',');
                }
                ++index;
                x.Append('"').Append(e.Key.ToString()).Append('"');
                x.Append(':');
                x.Append(e.Value.Apply(this));
            }
            x.Append('}');
            return x.ToString();
        }

        public virtual string Accept(DVector2 type)
        {
            var v = type.Value;
            return $"{{\"x\":{v.X},\"y\":{v.Y}}}";
        }

        public virtual string Accept(DVector3 type)
        {
            var v = type.Value;
            return $"{{\"x\":{v.X},\"y\":{v.Y},\"z\":{v.Z}}}";
        }

        public virtual string Accept(DVector4 type)
        {
            var v = type.Value;
            return $"{{\"x\":{v.X},\"y\":{v.Y},\"z\":{v.Z},\"w\":{v.W}}}";
        }

        public string Accept(DDateTime type)
        {
            var ass = DefAssembly.LocalAssebmly as DefAssembly;
            return type.GetUnixTime(ass.TimeZone).ToString();
        }
    }
}
