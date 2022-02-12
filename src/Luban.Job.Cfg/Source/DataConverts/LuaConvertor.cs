using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataConverts
{
    class LuaConvertor : DataVisitors.ToLuaLiteralVisitor
    {
        //public static new LuaConvertor Ins { get; } = new();

        private string _indentStr = "";

        private const string INDENT_STEP = "    ";

        public string ExportRecord(DefTable t, Record record)
        {
            return "return " + record.Data.Apply(this);
        }

        public override string Accept(DText type)
        {
            return $"{{{DText.KEY_NAME}='{type.Key}',{DText.TEXT_NAME}=\"{DataUtil.EscapeString(type.RawValue)}\"}}";
        }

        public override string Accept(DEnum type)
        {
            return $"'{type.StrValue}'";
        }

        public override string Accept(DBean type)
        {
            string curIndent = _indentStr;
            string subIndent = _indentStr + INDENT_STEP;
            _indentStr = subIndent;

            var x = new StringBuilder();
            x.AppendLine("{");
            if (type.Type.IsAbstractType)
            {
                x.Append(subIndent).AppendLine($"{DefBean.LUA_TYPE_NAME_KEY} = '{type.ImplType.Name}',");
            }

            int index = 0;
            foreach (var f in type.Fields)
            {
                var defField = (DefField)type.ImplType.HierarchyFields[index++];
                if (f == null)
                {
                    continue;
                }
                x.Append(subIndent);
                x.Append(defField.Name).Append(" = ");
                x.Append(f.Apply(this));
                x.AppendLine(",");
            }
            x.Append(curIndent).Append("}");
            _indentStr = curIndent;
            return x.ToString();
        }

        protected void AppendToString(List<DType> datas, StringBuilder x)
        {
            string curIndent = _indentStr;
            _indentStr += INDENT_STEP;
            x.Append("{");
            foreach (var e in datas)
            {
                x.AppendLine();
                x.Append(_indentStr).Append(e.Apply(this));
                x.Append(",");
            }

            x.AppendLine().Append(curIndent).Append("}");
            _indentStr = curIndent;
        }

        public override string Accept(DArray type)
        {
            var x = new StringBuilder();
            AppendToString(type.Datas, x);
            return x.ToString();
        }

        public override string Accept(DList type)
        {
            var x = new StringBuilder();
            AppendToString(type.Datas, x);
            return x.ToString();
        }

        public override string Accept(DSet type)
        {
            var x = new StringBuilder();
            AppendToString(type.Datas, x);
            return x.ToString();
        }

        public override string Accept(DMap type)
        {
            string curIndent = _indentStr;
            _indentStr += INDENT_STEP;
            var x = new StringBuilder();
            x.AppendLine("{");
            foreach (var e in type.Datas)
            {
                x.Append(_indentStr).Append('[');
                x.Append(e.Key.Apply(this));
                x.Append(']');
                x.Append(" = ");
                x.Append(e.Value.Apply(this));
                x.AppendLine(",");
            }
            x.Append(curIndent).AppendLine("}");
            _indentStr = curIndent;
            return x.ToString();
        }

        public override string Accept(DDateTime type)
        {
            return type.ToFormatString();
        }
    }
}
