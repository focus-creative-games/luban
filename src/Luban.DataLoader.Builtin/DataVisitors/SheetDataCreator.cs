// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.DataLoader.Builtin.Excel;
using Luban.DataLoader.Builtin.Excel.DataParser;
using Luban.DataLoader.Builtin.Utils;
using Luban.Diagnostics;
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
            throw new LubanException("error.data.field_not_empty");
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
        if (!LoadDataUtil.TryParseExcelByteFromNumberOrConstAlias(x.ToString(), out byte v))
        {
            throw new LubanException("error.data.invalid_byte", x);
        }
        return DByte.ValueOf(v);
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
        if (!LoadDataUtil.TryParseExcelShortFromNumberOrConstAlias(x.ToString(), out short v))
        {
            throw new LubanException("error.data.invalid_short", x);
        }
        return DShort.ValueOf(v);
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
        if (!LoadDataUtil.TryParseExcelIntFromNumberOrConstAlias(x.ToString(), out var v))
        {
            throw new LubanException("error.data.invalid_int", x);
        }
        return DInt.ValueOf(v);
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
        if (!LoadDataUtil.TryParseExcelLongFromNumberOrConstAlias(x.ToString(), out var v))
        {
            throw new LubanException("error.data.invalid_long", x);
        }
        return DLong.ValueOf(v);
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
        if (!LoadDataUtil.TryParseExcelFloatFromNumberOrConstAlias(x.ToString(), out var v))
        {
            throw new LubanException("error.data.invalid_float", x);
        }
        return DFloat.ValueOf(v);
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
        if (!LoadDataUtil.TryParseExcelDoubleFromNumberOrConstAlias(x.ToString(), out var v))
        {
            throw new LubanException("error.data.invalid_double", x);
        }
        return DDouble.ValueOf(v);
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

                throw new LubanException("error.data.enum_no_zero_default", type.DefEnum.FullName);
            }
            return new DEnum(type, x.ToString());
        }

        if (row.Rows != null)
        {
            throw new LubanException("error.excel.enum_no_multirow", type.DefEnum.FullName);
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
                    throw new LubanException("error.excel.enum_invalid_item", itemName, type.DefEnum.FullName);
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

                throw new LubanException("error.data.enum_no_zero_default", type.DefEnum.FullName);
            }
            return new DEnum(type, string.Join(type.GetTagOrDefault("sep", "|"), items));
        }
        if (row.Elements != null)
        {
            throw new LubanException("error.excel.enum_no_multirow_sub");
        }
        throw new LubanException("error.internal.unexpected");
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
            throw new LubanException("error.data.not_nullable");
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
                throw new LubanException("error.excel.missing_column", bean.FullName, fname);
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
        IDataParser dataParser = row.GetDataParser();
        string sep = row.SelfTitle.Sep;// type.GetBeanAs<DefBean>().Sep;
        if (row.Row != null)
        {
            return dataParser.ParseBean(type, row.Row, row);
        }

        if (row.Rows != null)
        {
            //var s = row.AsMultiRowConcatStream(sep);
            //if (type.IsNullable && s.TryReadEOF())
            //{
            //    return null;
            //}
            //return type.Apply(ExcelStreamDataCreator.Ins, s);
            throw new LubanException("error.excel.bean_no_multirow", type.DefBean.FullName);
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
                    throw new LubanException("error.excel.polymorphic_need_type_column", originBean.FullName, FieldNames.ExcelTypeNameKey);
                }
                TitleRow valueTitle = row.GetSubTitleNamedRow(FieldNames.ExcelValueNameKey);
                sep += type.GetTag("sep");
                string subType = typeTitle.Current?.ToString()?.Trim();
                if (subType == null || subType == FieldNames.BeanNullType)
                {
                    if (!type.IsNullable)
                    {
                        throw new LubanException("error.excel.not_nullable_bean", originBean.FullName, type.DefBean.FullName);
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
                    TBean implBeanType = TBean.Create(type.IsNullable, implType, null);
                    DBean implData = dataParser.ParseBean(implBeanType, valueTitle.Row, valueTitle);
                    return new DBean(type, implType, implData.Fields);
                }

                if (valueTitle.Rows != null)
                {
                    throw new LubanException("error.excel.bean_no_multirow", type.DefBean.FullName);
                }
                throw new LubanException("error.internal.unexpected");
            }

            if (type.IsNullable)
            {
                TitleRow typeTitle = row.GetSubTitleNamedRow(FieldNames.ExcelTypeNameKey) ?? row.GetSubTitleNamedRow(FieldNames.FallbackTypeNameKey);
                if (typeTitle == null)
                {
                    throw new LubanException("error.excel.nullable_need_type_column", originBean.FullName, FieldNames.ExcelTypeNameKey);
                }
                string subType = typeTitle.Current?.ToString()?.Trim();
                if (subType == null || subType == FieldNames.BeanNullType)
                {
                    return null;
                }

                if (subType != FieldNames.BeanNotNullType && subType != originBean.Name)
                {
                    throw new LubanException("error.excel.invalid_null_flag", originBean.FullName, subType, FieldNames.BeanNullType, FieldNames.BeanNotNullType, originBean.Name);
                }
            }

            return new DBean(type, originBean, CreateBeanFields(originBean, sheet, row));
        }
        if (row.Elements != null)
        {
            return ReadMultiRowBeanData(type, sheet, row);
        }
        throw new LubanException("error.internal.unexpected");
    }

    private DType ReadMultiRowBeanData(TBean type, RowColumnSheet sheet, TitleRow row)
    {
        var elements = row.Elements;
        if (elements.Count == 0)
        {
            if (type.IsNullable)
            {
                return null;
            }
            throw new LubanException("error.excel.bean_missing_data", row.SelfTitle.Name, type.DefBean.FullName);
        }
        if (elements[0].HasSubFields)
        {
            // 带子列名的多行格式：单 bean 只能对应一行子字段数据
            if (elements.Count > 1)
            {
                throw new LubanException("error.excel.bean_multirow_as_list", row.SelfTitle.Name, type.DefBean.FullName, elements.Count);
            }
            return Accept(type, sheet, elements[0]);
        }
        // 平铺格式（无子列名）：多行单元格按列顺序摊平成流解析，兼容旧版本表格式
        var s = row.AsMultiRowConcatElements(row.SelfTitle.Sep);
        var result = type.Apply(ExcelStreamDataCreator.Ins, s);
        if (!s.TryReadEOF())
        {
            throw new LubanException("error.excel.bean_extra_multirow", row.SelfTitle.Name, type.DefBean.FullName);
        }
        return result;
    }

    private List<DType> ReadCollectionDatas(TType type, TType elementType, RowColumnSheet sheet, TitleRow row)
    {
        IDataParser dataParser = row.GetDataParser();
        if (row.Row != null)
        {
            return dataParser.ParseCollectionElements(type, row.Row, row);
        }
        if (row.Rows != null)
        {
            throw new LubanException("error.excel.array_need_multirow", row.SelfTitle.Name);
        }
        if (row.Fields != null)
        {
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
        throw new LubanException("error.internal.unexpected");
    }

    public DType Accept(TArray type, RowColumnSheet sheet, TitleRow row)
    {
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
        IDataParser dataParser = row.GetDataParser();
        string sep = row.SelfTitle.Sep;

        if (row.Row != null)
        {
            return dataParser.ParseMap(type, row.Row, row);
        }

        if (row.Rows != null)
        {
            throw new LubanException("error.excel.map_need_multirow", row.SelfTitle.Name);
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
                    var valueData = dataParser.ParseAny(type.ValueType, e.Value.Row, e.Value);
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
                        throw new LubanException("error.excel.map_need_key_column", FieldNames.ExcelMapKey);
                    }
                    var keyData = type.KeyType.Apply(this, sheet, keyTitle);
                    var valueData = type.ValueType.Apply(this, sheet, e);
                    datas.Add(keyData, valueData);
                }
                else
                {
                    var (keyData, valueData) = dataParser.ParseMapEntry(type, e.Row, e);
                    datas.Add(keyData, valueData);
                }
            }
            return new DMap(type, datas);
        }
        throw new LubanException("error.internal.unexpected");
    }
}
