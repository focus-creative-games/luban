using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser
{
    public class SteamParser : DataParserBase
    {
        private ExcelStream AsStream(TitleRow row, List<Cell> cells, string sep)
        {
            Title selfTitle = row.SelfTitle;
            if (string.IsNullOrEmpty(sep))
            {
                return new ExcelStream(cells, selfTitle.FromIndex, selfTitle.ToIndex, "", selfTitle.Default);
            }
            else
            {
                return new ExcelStream(cells, selfTitle.FromIndex, selfTitle.ToIndex, sep, selfTitle.Default);
            }
        }

        public override DType ParseAny(TType type, List<Cell> cells, TitleRow title)
        {
            ExcelStream stream = AsStream(title, cells, title.SelfTitle.Sep);
            return type.Apply(ExcelStreamDataCreator.Ins, stream);
        }

        public override DType ParseBean(TBean type, List<Cell> cells, TitleRow title)
        {
            var s = AsStream(title, cells, title.SelfTitle.Sep);
            if (type.IsNullable && s.TryReadEOF())
            {
                return null;
            }
            return type.Apply(ExcelStreamDataCreator.Ins, s);
        }


        public override DType ParseAbstractBean(TBean type, DefBean implType, List<Cell> cells, TitleRow title)
        {
            var s = AsStream(title, cells, title.SelfTitle.Sep);
            if (type.IsNullable && s.TryReadEOF())
            {
                return null;
            }
            return new DBean(type, implType, CreateBeanFields(implType, s));
        }

        private List<DType> CreateBeanFields(DefBean bean, ExcelStream stream)
        {
            var list = new List<DType>();
            foreach (DefField f in bean.HierarchyFields)
            {
                try
                {
                    list.Add(f.CType.Apply(ExcelStreamDataCreator.Ins, stream));
                }
                catch (DataCreateException dce)
                {
                    dce.Push(bean, f);
                    throw;
                }
                catch (Exception e)
                {
                    var dce = new DataCreateException(e, stream.LastReadDataInfo);
                    dce.Push(bean, f);
                    throw dce;
                }
            }
            return list;
        }

        public override List<DType> ParseCollectionElements(TType collectionType, TType elementType, List<Cell> cells, TitleRow title)
        {
            return ExcelStreamDataCreator.Ins.ReadList(collectionType, elementType, AsStream(title, cells, title.SelfTitle.Sep));
        }

        public override DMap ParseMap(TMap type, List<Cell> cells, TitleRow title)
        {
            var s = AsStream(title, cells, title.SelfTitle.Sep);
            return (DMap)type.Apply(ExcelStreamDataCreator.Ins, s);
        }

        public override KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title)
        {
            var s = AsStream(title, cells, title.SelfTitle.Sep);
            var keyData = type.KeyType.Apply(ExcelStreamDataCreator.Ins, s);
            var valueData = type.ValueType.Apply(ExcelStreamDataCreator.Ins, s);
            return new KeyValuePair<DType, DType>(keyData, valueData);
        }
    }
}
