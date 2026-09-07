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

using Luban.DataLoader.Builtin.Lite;
using Luban.DataLoader.Builtin.Utils;
using Luban.Diagnostics;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

class LiteStreamDataCreator : ITypeFuncVisitor<LiteStream, DType>
{
    public static LiteStreamDataCreator Ins { get; } = new();

    private bool CheckNull(bool nullable, string s)
    {
        return nullable && s == "null";
    }

    private static bool CreateBool(string x)
    {
        var s = x.Trim();
        return LoadDataUtil.ParseExcelBool(s);
    }

    public DType Accept(TBool type, LiteStream x)
    {

        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        return DBool.ValueOf(CreateBool(d));
    }

    public DType Accept(TByte type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!LoadDataUtil.TryParseExcelByteFromNumberOrConstAlias(d, out byte v))
        {
            throw new LubanException("error.data.invalid_byte", d);
        }
        return DByte.ValueOf(v);
    }

    public DType Accept(TShort type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!LoadDataUtil.TryParseExcelShortFromNumberOrConstAlias(d, out short v))
        {
            throw new LubanException("error.data.invalid_short", d);
        }
        return DShort.ValueOf(v);
    }

    public DType Accept(TInt type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!LoadDataUtil.TryParseExcelIntFromNumberOrConstAlias(d, out var v))
        {
            throw new LubanException("error.data.invalid_int", d);
        }
        return DInt.ValueOf(v);
    }

    public DType Accept(TLong type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        //}
        if (!LoadDataUtil.TryParseExcelLongFromNumberOrConstAlias(d, out var v))
        {
            throw new LubanException("error.data.invalid_long", d);
        }
        return DLong.ValueOf(v);
    }

    public DType Accept(TFloat type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!LoadDataUtil.TryParseExcelFloatFromNumberOrConstAlias(d, out var v))
        {
            throw new LubanException("error.data.invalid_float", d);
        }
        return DFloat.ValueOf(v);
    }

    public DType Accept(TDouble type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!LoadDataUtil.TryParseExcelDoubleFromNumberOrConstAlias(d, out var v))
        {
            throw new LubanException("error.data.invalid_double", d);
        }
        return DDouble.ValueOf(v);
    }

    public DType Accept(TEnum type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        return new DEnum(type, d.Trim());
    }

    private static string ParseString(string s, bool nullable)
    {
        if (nullable && (string.IsNullOrEmpty(s) || s == "null"))
        {
            return null;
        }
        return DataUtil.RemoveStringQuote(s);
    }

    public DType Accept(TString type, LiteStream x)
    {
        string d = x.ReadData();
        var s = ParseString(d, type.IsNullable);
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

    public DType Accept(TDateTime type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        return DataUtil.CreateDateTime(d);
    }

    private List<DType> CreateBeanFields(DefBean bean, LiteStream stream)
    {
        var list = new List<DType>();
        foreach (DefField f in bean.HierarchyFields)
        {
            try
            {
                list.Add(f.CType.Apply(this, stream));
            }
            catch (DataCreateException dce)
            {
                dce.Push(bean, f);
                throw;
            }
        }
        return list;
    }

    public DType Accept(TBean type, LiteStream x)
    {
        var originBean = type.DefBean;
        if (type.IsNullable)
        {
            if (!x.IsBeginOfStructOrCollection())
            {
                string subType = x.ReadData().ToLower().Trim();
                if (subType == FieldNames.BeanNullType)
                {
                    return null;
                }
                else
                {
                    throw new LubanException("error.data.lite_bean_format", originBean.FullName);
                }
            }
        }
        x.ReadStructOrCollectionBegin();

        if (originBean.IsAbstractType)
        {
            string subType = x.ReadData();
            if (subType.ToLower().Trim() == FieldNames.BeanNullType)
            {
                if (!type.IsNullable)
                {
                    throw new LubanException("error.data.bean_not_nullable", originBean.FullName);
                }
                return null;
            }
            DefBean implType = DataUtil.GetImplTypeByNameOrAlias(originBean, subType);
            var fields = CreateBeanFields(implType, x);
            x.ReadStructOrCollectionEnd();
            return new DBean(type, implType, fields);

        }
        else
        {
            var fields = CreateBeanFields(originBean, x);
            x.ReadStructOrCollectionEnd();
            return new DBean(type, originBean, fields);
        }
    }

    // 容器类统统不支持 type.IsNullable
    // 因为貌似没意义？
    public List<DType> ReadList(TType type, TType eleType, LiteStream stream)
    {
        stream.ReadStructOrCollectionBegin();
        var datas = new List<DType>();
        while (!stream.IsEndOfStructOrCollection())
        {
            datas.Add(eleType.Apply(this, stream));
        }
        stream.ReadStructOrCollectionEnd();
        return datas;
    }

    public DType Accept(TArray type, LiteStream x)
    {
        return new DArray(type, ReadList(type, type.ElementType, x));
    }

    public DType Accept(TList type, LiteStream x)
    {
        return new DList(type, ReadList(type, type.ElementType, x));
    }

    public DType Accept(TSet type, LiteStream x)
    {
        return new DSet(type, ReadList(type, type.ElementType, x));
    }

    public DType Accept(TMap type, LiteStream stream)
    {
        stream.ReadStructOrCollectionBegin();
        var datas = new Dictionary<DType, DType>();
        while (!stream.IsEndOfStructOrCollection())
        {
            stream.ReadStructOrCollectionBegin();
            var key = type.KeyType.Apply(this, stream);
            var value = type.ValueType.Apply(this, stream);
            stream.ReadStructOrCollectionEnd();
            if (!datas.TryAdd(key, value))
            {
                throw new LubanException("error.data.map_duplicate_key", key);
            }
        }
        stream.ReadStructOrCollectionEnd();
        return new DMap(type, datas);
    }
}
