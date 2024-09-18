using Luban.DataLoader.Builtin.Excel;
using Luban.DataLoader.Builtin.Utils;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

class SheetDataCreator : ITypeFuncVisitor<RowColumnSheet, TitleRow, DType>
{
    public static SheetDataCreator Ins { get; } = new();

    private bool CheckNull(bool nullable, object o)
    {
        return nullable && (o == null || (o is string s && s == "null"));
    }

    private bool CheckDefault(object o)
    {
        return o == null || (o is string s && s.Length == 0);
    }

    private void ThrowIfNonEmpty(TitleRow row)
    {
        if (row.SelfTitle.NonEmpty)
        {
            throw new Exception($"字段不允许为空");
        }
    }

    public DType Accept(TBool type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
            return DBool.ValueOf(false);
        }
        if (x is bool v)
        {
            return DBool.ValueOf(v);
        }
        return DBool.ValueOf(LoadDataUtil.ParseExcelBool(x.ToString()));
    }

    public DType Accept(TByte type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            return DByte.Default;
        }
        return DByte.ValueOf(byte.Parse(x.ToString()));
    }

    public DType Accept(TShort type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
            return DShort.Default;
        }
        return DShort.ValueOf(short.Parse(x.ToString()));
    }
    public DType Accept(TInt type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
            return DInt.Default;
        }
        return DInt.ValueOf(int.Parse(x.ToString()));
    }

    public DType Accept(TLong type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
            return DLong.Default;
        }
        return DLong.ValueOf(long.Parse(x.ToString()));
    }

    public DType Accept(TFloat type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
            return DFloat.Default;
        }
        return DFloat.ValueOf(float.Parse(x.ToString()));
    }

    public DType Accept(TDouble type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckNull(type.IsNullable, x))
        {
            return null;
        }
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
            return DDouble.Default;
        }
        return DDouble.ValueOf(double.Parse(x.ToString()));
    }

    public DType Accept(TEnum type, RowColumnSheet sheet, TitleRow row)
    {
        if (row.Row != null)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                if (type.DefEnum.IsFlags || type.DefEnum.HasZeroValueItem)
                {
                    return new DEnum(type, "0");
                }

                throw new InvalidExcelDataException($"枚举类:'{type.DefEnum.FullName}' 没有value为0的枚举项, 不支持默认值");
            }
            return new DEnum(type, x.ToString());
        }

        if (row.Rows != null)
        {
            throw new Exception($"{type.DefEnum.FullName} 不支持多行格式");
        }
        if (row.Fields != null)
        {
            //throw new Exception($"array 不支持 子字段. 忘记将字段设为多行模式?  {row.SelfTitle.Name} => *{row.SelfTitle.Name}");

            var items = new List<string>();
            var sortedFields = row.Fields.Values.ToList();
            sortedFields.Sort((a, b) => a.SelfTitle.FromIndex - b.SelfTitle.FromIndex);
            foreach (var field in sortedFields)
            {
                string itemName = field.SelfTitle.Name;
                if (!type.DefEnum.TryValueByNameOrAlias(itemName, out _))
                {
                    throw new Exception($"列名:{itemName} 不是枚举类型'{type.DefEnum.FullName}'的有效枚举项");
                }
                if (field.IsBlank)
                {
                    continue;
                }
                string cur = field.Current.ToString().ToLower();
                if (cur != "0" && cur != "false")
                {
                    items.Add(itemName);
                }
            }
            if (items.Count == 0)
            {
                if (type.IsNullable)
                {
                    return null;
                }

                if (type.DefEnum.IsFlags || type.DefEnum.HasZeroValueItem)
                {
                    return new DEnum(type, "0");
                }

                throw new InvalidExcelDataException($"枚举类:'{type.DefEnum.FullName}' 没有value为0的枚举项, 不支持默认值");
            }
            return new DEnum(type, string.Join('|', items));
        }
        if (row.Elements != null)
        {
            throw new Exception($"{type.DefEnum.FullName} 不支持多行子字段格式");
        }
        throw new Exception();
    }


    public static string ParseString(object d, bool nullable)
    {
        if (d == null)
        {
            return nullable ? null : string.Empty;
        }

        string s = d is string str ? str : d.ToString();

        if (nullable && string.IsNullOrEmpty(s))
        {
            return null;
        }
        return DataUtil.UnEscapeRawString(s);
    }

    public DType Accept(TString type, RowColumnSheet sheet, TitleRow row)
    {
        object x = row.Current;
        if (CheckDefault(x))
        {
            ThrowIfNonEmpty(row);
        }
        var s = ParseString(x, type.IsNullable);
        if (s == null)
        {
            if (type.IsNullable)
            {
                return null;
            }
            throw new InvalidExcelDataException("字段不是nullable类型，不能为null");
        }
        return DString.ValueOf(type, s);
    }

    public DType Accept(TDateTime type, RowColumnSheet sheet, TitleRow row)
    {
        var d = row.Current;
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (d is System.DateTime datetime)
        {
            return new DDateTime(datetime);
        }
        return DataUtil.CreateDateTime(d.ToString());
    }

    private bool TryGetBeanField(TitleRow row, DefField field, out TitleRow ele)
    {
        if (!string.IsNullOrEmpty(field.CurrentVariantNameWithFieldName))
        {
            ele = row.GetSubTitleNamedRow(field.CurrentVariantNameWithFieldName);
            if (ele != null)
            {
                return true;
            }
        }
        ele = row.GetSubTitleNamedRow(field.Name);
        if (ele != null)
        {
            return true;
        }
        if (!string.IsNullOrEmpty(field.Alias))
        {
            ele = row.GetSubTitleNamedRow(field.Alias);
            return ele != null;
        }
        return false;
    }

    private List<DType> CreateBeanFields(DefBean bean, RowColumnSheet sheet, TitleRow row)
    {
        var list = new List<DType>();
        foreach (DefField f in bean.HierarchyFields)

        {
            string fname = f.Name;
            if (!TryGetBeanField(row, f, out var field))
            {
                throw new Exception($"bean:'{bean.FullName}' 缺失 列:'{fname}'，请检查是否写错或者遗漏");
            }
            try
            {
                list.Add(f.CType.Apply(this, sheet, field));
            }
            catch (DataCreateException dce)
            {
                dce.Push(bean, f);
                throw;
            }
            catch (Exception e)
            {
                var dce = new DataCreateException(e, $"Sheet:{sheet.SheetName} 字段:{fname} 位置:{field.Location}");
                dce.Push(bean, f);
                throw dce;
            }
        }
        return list;
    }

    public DType Accept(TBean type, RowColumnSheet sheet, TitleRow row)
    {
        string sep = row.SelfTitle.Sep;// type.GetBeanAs<DefBean>().Sep;
        if (row.Row != null)
        {
            var s = row.AsStream(sep);
            if (type.IsNullable && s.TryReadEOF())
            {
                return null;
            }
            return type.Apply(ExcelStreamDataCreator.Ins, s);
        }

        if (row.Rows != null)
        {
            var s = row.AsMultiRowConcatStream(sep);
            if (type.IsNullable && s.TryReadEOF())
            {
                return null;
            }
            return type.Apply(ExcelStreamDataCreator.Ins, s);
        }
        if (row.Fields != null)
        {
            sep += type.DefBean.Sep;
            var originBean = type.DefBean;
            if (originBean.IsAbstractType)
            {
                TitleRow typeTitle = row.GetSubTitleNamedRow(FieldNames.ExcelTypeNameKey) ?? row.GetSubTitleNamedRow(FieldNames.FallbackTypeNameKey);
                if (typeTitle == null)
                {
                    throw new Exception($"type:'{originBean.FullName}' 是多态类型,需要定义'{FieldNames.ExcelTypeNameKey}'列来指定具体子类型");
                }
                TitleRow valueTitle = row.GetSubTitleNamedRow(FieldNames.ExcelValueNameKey);
                sep += type.GetTag("sep");
                string subType = typeTitle.Current?.ToString()?.Trim();
                if (subType == null || subType == FieldNames.BeanNullType)
                {
                    if (!type.IsNullable)
                    {
                        throw new Exception($"type:'{originBean.FullName}' 不是可空类型 '{type.DefBean.FullName}?' , 不能为空");
                    }
                    return null;
                }
                DefBean implType = DataUtil.GetImplTypeByNameOrAlias(originBean, subType);
                if (valueTitle == null)
                {
                    return new DBean(type, implType, CreateBeanFields(implType, sheet, row));
                }

                sep += valueTitle.SelfTitle.Sep;
                if (valueTitle.Row != null)
                {
                    var s = valueTitle.AsStream(sep);
                    if (type.IsNullable && s.TryReadEOF())
                    {
                        return null;
                    }
                    return new DBean(type, implType, CreateBeanFields(implType, s));
                }

                if (valueTitle.Rows != null)
                {
                    var s = valueTitle.AsMultiRowConcatStream(sep);
                    if (type.IsNullable && s.TryReadEOF())
                    {
                        return null;
                    }
                    return new DBean(type, implType, CreateBeanFields(implType, s));
                }
                throw new Exception();
            }

            if (type.IsNullable)
            {
                TitleRow typeTitle = row.GetSubTitleNamedRow(FieldNames.ExcelTypeNameKey) ?? row.GetSubTitleNamedRow(FieldNames.FallbackTypeNameKey);
                if (typeTitle == null)
                {
                    throw new Exception($"type:'{originBean.FullName}' 是可空类型,需要定义'{FieldNames.ExcelTypeNameKey}'列来指明是否可空");
                }
                string subType = typeTitle.Current?.ToString()?.Trim();
                if (subType == null || subType == FieldNames.BeanNullType)
                {
                    return null;
                }

                if (subType != FieldNames.BeanNotNullType && subType != originBean.Name)
                {
                    throw new Exception($"type:'{originBean.FullName}' 可空标识:'{subType}' 不合法（只能为'{FieldNames.BeanNullType}'或'{FieldNames.BeanNotNullType}'或'{originBean.Name}')");
                }
            }

            return new DBean(type, originBean, CreateBeanFields(originBean, sheet, row));
        }
        if (row.Elements != null)
        {
            var s = row.AsMultiRowConcatElements(sep);
            return type.Apply(ExcelStreamDataCreator.Ins, s);
        }
        throw new Exception();
    }

    private static List<DType> ReadList(TType type, IEnumerable<ExcelStream> streams)
    {
        var datas = new List<DType>();
        foreach (var stream in streams)
        {
            while (!stream.TryReadEOF())
            {
                datas.Add(type.Apply(ExcelStreamDataCreator.Ins, stream));
            }
        }
        return datas;
    }

    private List<DType> ReadCollectionDatas(TType type, TType elementType, RowColumnSheet sheet, TitleRow row)
    {
        if (row.Row != null)
        {
            var s = row.AsStream(row.SelfTitle.Sep);
            return ExcelStreamDataCreator.Ins.ReadList(type, elementType, s);
        }
        if (row.Rows != null)
        {
            var s = row.AsMultiRowStream(row.SelfTitle.Sep);
            return ReadList(elementType, s);
        }
        if (row.Fields != null)
        {
            //throw new Exception($"array 不支持 子字段. 忘记将字段设为多行模式?  {row.SelfTitle.Name} => *{row.SelfTitle.Name}");

            var datas = new List<DType>(row.Fields.Count);
            var sortedFields = row.Fields.Values.ToList();
            sortedFields.Sort((a, b) => a.SelfTitle.FromIndex - b.SelfTitle.FromIndex);
            foreach (var field in sortedFields)
            {
                if (field.IsBlank)
                {
                    continue;
                }
                datas.Add(elementType.Apply(this, sheet, field));
            }
            return datas;
        }
        if (row.Elements != null)
        {
            return row.Elements.Select(e => elementType.Apply(this, sheet, e)).ToList();
        }
        throw new Exception();
    }

    public DType Accept(TArray type, RowColumnSheet sheet, TitleRow row)
    {
        //string sep = DataUtil.GetSep(type);
        return new DArray(type, ReadCollectionDatas(type, type.ElementType, sheet, row));
    }

    public DType Accept(TList type, RowColumnSheet sheet, TitleRow row)
    {
        return new DList(type, ReadCollectionDatas(type, type.ElementType, sheet, row));
    }

    public DType Accept(TSet type, RowColumnSheet sheet, TitleRow row)
    {
        return new DSet(type, ReadCollectionDatas(type, type.ElementType, sheet, row));
    }

    public DType Accept(TMap type, RowColumnSheet sheet, TitleRow row)
    {
        string sep = row.SelfTitle.Sep;

        if (row.Row != null)
        {
            var s = row.AsStream(sep);
            return type.Apply(ExcelStreamDataCreator.Ins, s);
        }

        if (row.Rows != null)
        {
            throw new Exception($"map在非多行模式下不支持多行填写，是否忘记将字段设为多行模式?  {row.SelfTitle.Name} => *{row.SelfTitle.Name}");
        }
        if (row.Fields != null)
        {
            var datas = new Dictionary<DType, DType>();
            foreach (var e in row.Fields)
            {
                var keyData = type.KeyType.Apply(StringDataCreator.Ins, e.Key);
                if (e.Value.Row != null)
                {
                    if (RowColumnSheet.IsBlankRow(e.Value.Row, e.Value.SelfTitle.FromIndex, e.Value.SelfTitle.ToIndex))
                    {
                        continue;
                    }
                    var valueData = type.ValueType.Apply(ExcelStreamDataCreator.Ins, e.Value.AsStream(""));
                    datas.Add(keyData, valueData);
                }
                else
                {
                    var valueData = type.ValueType.Apply(this, sheet, e.Value);
                    datas.Add(keyData, valueData);
                }
            }
            return new DMap(type, datas);
        }
        if (row.Elements != null)
        {
            var datas = new Dictionary<DType, DType>();
            foreach (var e in row.Elements)
            {
                if (e.SelfTitle.SubTitleList.Count > 0)
                {
                    TitleRow keyTitle = e.GetSubTitleNamedRow(FieldNames.ExcelMapKey);
                    if (keyTitle == null)
                    {
                        throw new Exception($"多行模式下map需要定义'{FieldNames.ExcelMapKey}'列来指明key");
                    }
                    var keyData = type.KeyType.Apply(this, sheet, keyTitle);
                    var valueData = type.ValueType.Apply(this, sheet, e);
                    datas.Add(keyData, valueData);
                }
                else
                {
                    var stream = e.AsStream(sep);
                    var keyData = type.KeyType.Apply(ExcelStreamDataCreator.Ins, stream);
                    var valueData = type.ValueType.Apply(ExcelStreamDataCreator.Ins, stream);
                    datas.Add(keyData, valueData);
                }
            }
            return new DMap(type, datas);
        }
        throw new Exception();
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
}
