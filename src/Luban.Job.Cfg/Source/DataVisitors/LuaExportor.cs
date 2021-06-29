using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class LuaExportor : IDataActionVisitor<DefAssembly, StringBuilder>
    {
        public static LuaExportor Ins { get; } = new LuaExportor();

        public void ExportTableOne(DefTable t, List<Record> records, List<string> result)
        {
            result.Add("return ");
            var s = new StringBuilder();
            Accept(records[0].Data, t.Assembly, s);
            result.Add(s.ToString());
        }

        public void ExportTableOneKeyMap(DefTable t, List<Record> records, List<string> result)
        {
            result.Add("return ");
            result.Add("{");
            var s = new StringBuilder();
            var ks = new StringBuilder();
            foreach (Record r in records)
            {
                DBean d = r.Data;
                s.Clear();
                s.Append($"[{ToLuaCodeString(d.GetField(t.Index), t.Assembly, ks)}] = ");
                Accept(d, t.Assembly, s);
                s.Append(',');
                result.Add(s.ToString());
            }
            result.Add("}");
        }

        private string ToLuaCodeString(DType data, DefAssembly ass, StringBuilder b)
        {
            b.Clear();
            data.Apply(this, ass, b);
            return b.ToString();
        }

        public void Accept(DBool type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value ? "true" : "false");
        }

        public void Accept(DByte type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DShort type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFshort type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DInt type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFint type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DLong type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFlong type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DFloat type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DDouble type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        public void Accept(DEnum type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.Value);
        }

        private string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("'", "\\'");
        }

        public void Accept(DString type, DefAssembly ass, StringBuilder line)
        {
            line.Append('\'').Append(EscapeString(type.Value)).Append('\'');
        }

        public void Accept(DBytes type, DefAssembly ass, StringBuilder line)
        {
            throw new NotImplementedException();
        }

        public void Accept(DText type, DefAssembly ass, StringBuilder line)
        {
            line.Append('\'').Append(EscapeString(type.GetText(ass.ExportTextTable, ass.NotConvertTextSet))).Append('\'');
        }

        public void Accept(DBean type, DefAssembly ass, StringBuilder line)
        {
            var bean = type.Type;
            if (bean.IsAbstractType)
            {
                line.Append($"{{ _name='{type.ImplType.Name}',");
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
                    field.Apply(this, ass, line);
                    line.Append(',');
                }
            }
            line.Append("}");
        }

        public void Accept(DArray type, DefAssembly ass, StringBuilder line)
        {
            line.Append('{');
            foreach (var d in type.Datas)
            {
                d.Apply(this, ass, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DList type, DefAssembly ass, StringBuilder line)
        {
            line.Append('{');
            foreach (var d in type.Datas)
            {
                d.Apply(this, ass, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DSet type, DefAssembly ass, StringBuilder line)
        {
            line.Append('{');
            foreach (var d in type.Datas)
            {
                d.Apply(this, ass, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DMap type, DefAssembly ass, StringBuilder line)
        {
            line.Append('{');
            foreach ((var k, var v) in type.Datas)
            {
                line.Append('[');
                k.Apply(this, ass, line);
                line.Append("]=");
                v.Apply(this, ass, line);
                line.Append(',');
            }
            line.Append('}');
        }

        public void Accept(DVector2 type, DefAssembly ass, StringBuilder line)
        {
            line.Append($"{{x={type.Value.X},y={type.Value.Y}}}");
        }

        public void Accept(DVector3 type, DefAssembly ass, StringBuilder line)
        {
            line.Append($"{{x={type.Value.X},y={type.Value.Y},z={type.Value.Z}}}");
        }

        public void Accept(DVector4 type, DefAssembly ass, StringBuilder line)
        {
            line.Append($"{{x={type.Value.X},y={type.Value.Y},z={type.Value.Z},w={type.Value.W}}}");
        }

        public void Accept(DDateTime type, DefAssembly ass, StringBuilder line)
        {
            line.Append(type.GetUnixTime(ass.TimeZone));
        }
    }
}
