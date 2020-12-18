using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class LuaExportor : IDataActionVisitor<StringBuilder>
    {
        public static LuaExportor Ins { get; } = new LuaExportor();

        public void ExportTableOne(DefTable t, List<DType> records, List<string> result)
        {
            result.Add("return ");
            var s = new StringBuilder();
            Accept((DBean)records[0], s);
            result.Add(s.ToString());
        }

        public void ExportTableOneKeyMap(DefTable t, List<DType> records, List<string> result)
        {
            result.Add("return ");
            result.Add("{");
            var s = new StringBuilder();
            var ks = new StringBuilder();
            foreach (DBean r in records)
            {
                s.Clear();
                s.Append($"[{ToLuaCodeString(r.GetField(t.Index), ks)}] = ");
                Accept(r, s);
                s.Append(',');
                result.Add(s.ToString());
            }
            result.Add("}");
        }

        public void ExportTableTwoKeyMap(DefTable t, List<DType> records, List<string> result)
        {
            result.Add("return ");
            result.Add("{");

            var s = new StringBuilder();
            var ks = new StringBuilder();
            foreach (var g in records.GroupBy(r => ((DBean)r).GetField(t.Index1)))
            {
                result.Add($"[{ToLuaCodeString(g.Key, ks)}] =");
                result.Add("{");

                foreach (DBean r in g)
                {
                    s.Clear();
                    s.Append($"[{ToLuaCodeString(r.GetField(t.Index2), ks)}] = ");
                    Accept(r, s);
                    s.Append(',');
                    result.Add(s.ToString());
                }

                result.Add("},");
            }

            result.Add("}");
        }

        private string ToLuaCodeString(DType data, StringBuilder b)
        {
            b.Clear();
            data.Apply(this, b);
            return b.ToString();
        }

        public void Accept(DBool type, StringBuilder line)
        {
            line.Append(type.Value ? "true" : "false");
        }

        public void Accept(DByte type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DShort type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFshort type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DInt type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFint type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DLong type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFlong type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFloat type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DDouble type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DEnum type, StringBuilder line)
        {
            line.Append(type.Value);
        }

        private string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("'", "\\'");
        }

        public void Accept(DString type, StringBuilder line)
        {
            line.Append('\'').Append(EscapeString(type.Value)).Append('\'');
        }

        public void Accept(DBytes type, StringBuilder line)
        {
            throw new NotImplementedException();
        }

        public void Accept(DText type, StringBuilder line)
        {
            line.Append('\'').Append(EscapeString(type.Value)).Append('\'');
        }

        public void Accept(DBean type, StringBuilder line)
        {
            var bean = type.Type;
            if (bean.IsAbstractType)
            {
                // null 时特殊处理
                if (type.ImplType == null)
                {
                    line.Append("nil");
                    return;
                }
                line.Append($"{{ _name='{type.ImplType.FullName}',");
            }
            else
            {
                line.Append('{');
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
                if (field != null)
                {
                    line.Append(defField.Name).Append('=');
                    field.Apply(this, line);
                    line.Append(',');
                }
            }
            line.Append("}");
        }

        public void Accept(DArray type, StringBuilder line)
        {
            line.Append('{');
            foreach (var d in type.Datas)
            {
                d.Apply(this, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DList type, StringBuilder line)
        {
            line.Append('{');
            foreach (var d in type.Datas)
            {
                d.Apply(this, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DSet type, StringBuilder line)
        {
            line.Append('{');
            foreach (var d in type.Datas)
            {
                d.Apply(this, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DMap type, StringBuilder line)
        {
            line.Append('{');
            foreach ((var k, var v) in type.Datas)
            {
                line.Append('[');
                k.Apply(this, line);
                line.Append("]=");
                v.Apply(this, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DVector2 type, StringBuilder line)
        {
            line.Append($"{{x={type.Value.X},y={type.Value.Y}}}");
        }

        public void Accept(DVector3 type, StringBuilder line)
        {
            line.Append($"{{x={type.Value.X},y={type.Value.Y},z={type.Value.Z}}}");
        }

        public void Accept(DVector4 type, StringBuilder line)
        {
            line.Append($"{{x={type.Value.X},y={type.Value.Y},z={type.Value.Z},w={type.Value.W}}}");
        }

        public void Accept(DDateTime type, StringBuilder line)
        {
            line.Append(type.UnixTime);
        }
    }
}
