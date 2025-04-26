using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser
{
    public abstract class TextParserBase : DataParserBase
    {
        protected string CreateString(List<Cell> cells, TitleRow title)
        {
            Title selfTitle = title.SelfTitle;
            var sb = new StringBuilder();
            for (int i = selfTitle.FromIndex; i <= selfTitle.ToIndex; i++)
            {
                Cell cell = cells[i];
                if (cell.Value == null)
                {
                    continue;
                }
                string s = cell.Value.ToString();
                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }
                sb.Append(s);
            }
            return sb.ToString();
        }


        protected List<string> CreateStrings(List<Cell> cells, TitleRow title)
        {
            Title selfTitle = title.SelfTitle;
            var ss = new List<string>();
            for (int i = selfTitle.FromIndex; i <= selfTitle.ToIndex; i++)
            {
                Cell cell = cells[i];
                if (cell.Value == null)
                {
                    continue;
                }
                string s = cell.Value.ToString();
                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }
                ss.Add(s);
            }
            return ss;
        }
    }
}
