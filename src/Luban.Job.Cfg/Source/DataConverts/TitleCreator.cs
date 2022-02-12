using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataConverts
{
    class TitleCreator : ITypeActionVisitor<Title, int>
    {
        public static TitleCreator Ins { get; } = new();

        public Title CreateTitle(DefTable table)
        {
            TBean type = table.ValueTType;
            //if (bean.IsDynamic)
            //{
            //    throw new Exception($"bean:{bean.Bean.FullName} 是多态bean，暂不支持");
            //}
            var title = new Title()
            {
                Root = true,
                Name = "__root__",
                Tags = new Dictionary<string, string>(),
                FromIndex = 1,
            };

            int lastColumn = 0;

            if (type.IsDynamic)
            {
                title.AddSubTitle(new Title()
                {
                    Name = DefBean.EXCEL_TYPE_NAME_KEY,
                    FromIndex = lastColumn + 1,
                    ToIndex = lastColumn + 1,
                    Tags = new Dictionary<string, string>(),
                });
                ++lastColumn;
            }

            var fields = type.IsDynamic ? type.Bean.HierarchyNotAbstractChildren.SelectMany(c => c.HierarchyFields) : type.Bean.HierarchyFields;

            foreach (var f in fields)
            {
                if (title.SubTitles.ContainsKey(f.Name))
                {
                    continue;
                }
                int startColumn = lastColumn + 1;
                var subTitle = new Title()
                {
                    Name = f.Name,
                    FromIndex = startColumn,
                    ToIndex = startColumn,
                    Tags = new Dictionary<string, string>(),
                };
                if (f.CType.Tags.TryGetValue("sep", out var sep))
                {
                    subTitle.Tags.Add("sep", sep);
                }

                f.CType.Apply(this, subTitle, startColumn);
                lastColumn = subTitle.ToIndex;
                title.AddSubTitle(subTitle);
            }
            title.ToIndex = Math.Max(lastColumn, 1);

            if (table.IsMapTable)
            {
                title.SubTitles[table.IndexField.Name].Tags.TryAdd("non_empty", "1");
            }
            title.Init();
            return title;
        }

        public void Accept(TBool type, Title title, int column)
        {

        }

        public void Accept(TByte type, Title title, int column)
        {

        }

        public void Accept(TShort type, Title title, int column)
        {

        }

        public void Accept(TFshort type, Title title, int column)
        {

        }

        public void Accept(TInt type, Title title, int column)
        {

        }

        public void Accept(TFint type, Title title, int column)
        {

        }

        public void Accept(TLong type, Title title, int column)
        {

        }

        public void Accept(TFlong type, Title title, int column)
        {

        }

        public void Accept(TFloat type, Title title, int column)
        {

        }

        public void Accept(TDouble type, Title title, int column)
        {

        }

        public void Accept(TEnum type, Title title, int column)
        {

        }

        public void Accept(TString type, Title title, int column)
        {

        }

        public void Accept(TBytes type, Title title, int column)
        {

        }

        public void Accept(TText type, Title title, int column)
        {
            // 默认使用 # 来分割
            if (!title.Tags.ContainsKey("sep"))
            {
                title.Tags.Add("sep", "#");
            }
        }

        public void Accept(TBean type, Title title, int column)
        {
            title.FromIndex = column;
            title.ToIndex = column;
            if (type.Bean is DefBean dbean && string.IsNullOrWhiteSpace(dbean.Sep))
            {
                if (!title.Tags.ContainsKey("sep"))
                {
                    title.Tags.Add("sep", "|");
                }
            }
        }

        public void Accept(TArray type, Title title, int column)
        {
            title.Tags.TryAdd("sep", "|");
        }

        public void Accept(TList type, Title title, int column)
        {
            title.Tags.TryAdd("sep", "|");
        }

        public void Accept(TSet type, Title title, int column)
        {
            title.Tags.TryAdd("sep", "|");
        }

        public void Accept(TMap type, Title title, int column)
        {
            title.Tags.TryAdd("sep", "|");
        }

        public void Accept(TVector2 type, Title title, int column)
        {

        }

        public void Accept(TVector3 type, Title title, int column)
        {

        }

        public void Accept(TVector4 type, Title title, int column)
        {

        }

        public void Accept(TDateTime type, Title title, int column)
        {

        }
    }
}
