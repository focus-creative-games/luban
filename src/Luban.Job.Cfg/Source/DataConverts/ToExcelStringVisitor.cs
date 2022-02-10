using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataConverts
{
    public class ToExcelStringVisitor : IDataFuncVisitor<string, string>
    {
        public static ToExcelStringVisitor Ins { get; } = new();

        public string Accept(DBool type, string sep)
        {
            return type.Value ? "true" : "false";
        }

        public string Accept(DByte type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DShort type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DFshort type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DInt type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DFint type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DLong type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DFlong type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DFloat type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DDouble type, string sep)
        {
            return type.Value.ToString();
        }

        public string Accept(DEnum type, string sep)
        {
            return type.StrValue;
        }

        private string Enscape(string s)
        {
            return string.IsNullOrEmpty(s) ? "\"\"" : s;
        }

        public string Accept(DString type, string sep)
        {
            return Enscape(type.Value.ToString());
        }

        public string Accept(DBytes type, string sep)
        {
            throw new NotImplementedException();
        }

        public string Accept(DText type, string sep)
        {
            return $"{Enscape(type.Key)}{sep}{Enscape(type.RawValue)}";
        }

        public string Accept(DBean type, string sep)
        {
            if (!string.IsNullOrWhiteSpace(type.Type.Sep))
            {
                sep = type.Type.Sep;
            }
            else if (string.IsNullOrWhiteSpace(sep))
            {
                sep = "|";
            }
            var sb = new List<string>();
            if (type.Type.IsAbstractType)
            {
                sb.Add(type.ImplType != null ? type.ImplType.Name : DefBean.BEAN_NULL_STR);
            }
            else if (type.TType.IsNullable)
            {
                sb.Add(type.ImplType != null ? type.ImplType.Name : DefBean.BEAN_NULL_STR);
            }

            foreach (var field in type.Fields)
            {
                if (field == null)
                {
                    sb.Add("null");
                    continue;
                }
                sb.Add(field.Apply(this, sep));
                // 对于数目不定的数据类型，需要加分割符
                switch (field)
                {
                    case DArray:
                    case DList:
                    case DSet:
                    case DMap:
                    {
                        sb.Add(ExcelStream.END_OF_LIST);
                        break;
                    }
                }
            }
            return string.Join(sep, sb);
        }

        public string Accept(DArray type, string sep)
        {
            if (string.IsNullOrEmpty(sep))
            {
                sep = "|";
            }
            return string.Join(sep, type.Datas.Select(d => d.Apply(this, sep)));
        }

        public string Accept(DList type, string sep)
        {
            if (string.IsNullOrEmpty(sep))
            {
                sep = "|";
            }
            return string.Join(sep, type.Datas.Select(d => d.Apply(this, sep)));
        }

        public string Accept(DSet type, string sep)
        {
            if (string.IsNullOrEmpty(sep))
            {
                sep = "|";
            }
            return string.Join(sep, type.Datas.Select(d => d.Apply(this, sep)));
        }

        public string Accept(DMap type, string sep)
        {
            if (string.IsNullOrEmpty(sep))
            {
                sep = "|";
            }
            return string.Join(sep, type.Datas.Select(d => $"{d.Key.Apply(this, sep)}{sep}{d.Value.Apply(this, sep)}"));
        }

        public string Accept(DVector2 type, string sep)
        {
            var v = type.Value;
            return $"{v.X},{v.Y}";
        }

        public string Accept(DVector3 type, string sep)
        {
            var v = type.Value;
            return $"{v.X},{v.Y},{v.Z}";
        }

        public string Accept(DVector4 type, string sep)
        {
            var v = type.Value;
            return $"{v.X},{v.Y},{v.Z},{v.W}";
        }

        public string Accept(DDateTime type, string sep)
        {
            return DataUtil.FormatDateTime(type.Time);
        }
    }
}
