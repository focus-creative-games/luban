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

using System.Text.Json;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

public class JsonDataCreator : ITypeFuncVisitor<JsonElement, DefAssembly, DType>
{
    public static JsonDataCreator Ins { get; } = new();

    public DType Accept(TBool type, JsonElement x, DefAssembly ass)
    {
        return DBool.ValueOf(x.GetBoolean());
    }

    public DType Accept(TByte type, JsonElement x, DefAssembly ass)
    {
        return DByte.ValueOf(x.GetByte());
    }

    public DType Accept(TShort type, JsonElement x, DefAssembly ass)
    {
        return DShort.ValueOf(x.GetInt16());
    }

    public DType Accept(TInt type, JsonElement x, DefAssembly ass)
    {
        return DInt.ValueOf(x.GetInt32());
    }

    public DType Accept(TLong type, JsonElement x, DefAssembly ass)
    {
        return DLong.ValueOf(x.GetInt64());
    }

    public DType Accept(TFloat type, JsonElement x, DefAssembly ass)
    {
        return DFloat.ValueOf(x.GetSingle());
    }

    public DType Accept(TDouble type, JsonElement x, DefAssembly ass)
    {
        return DDouble.ValueOf(x.GetDouble());
    }

    public DType Accept(TEnum type, JsonElement x, DefAssembly ass)
    {
        return new DEnum(type, x.ToString());
    }

    public DType Accept(TString type, JsonElement x, DefAssembly ass)
    {
        return DString.ValueOf(type, x.GetString());
    }

    private bool TryGetBeanField(JsonElement x, DefField field, out JsonElement ele)
    {
        if (!string.IsNullOrEmpty(field.CurrentVariantNameWithFieldName))
        {
            if (x.TryGetProperty(field.CurrentVariantNameWithFieldName, out ele))
            {
                return true;
            }
        }
        if (x.TryGetProperty(field.Name, out ele))
        {
            return true;
        }
        if (!string.IsNullOrEmpty(field.Alias) && x.TryGetProperty(field.Alias, out ele))
        {
            return true;
        }
        return false;
    }

    public DType Accept(TBean type, JsonElement x, DefAssembly ass)
    {
        var bean = type.DefBean;

        DefBean implBean;
        if (bean.IsAbstractType)
        {
            if (!x.TryGetProperty(FieldNames.JsonTypeNameKey, out var typeNameProp) && !x.TryGetProperty(FieldNames.FallbackTypeNameKey, out typeNameProp))
            {
                throw new Exception($"结构:'{bean.FullName}' 是多态类型，必须用 '{FieldNames.JsonTypeNameKey}' 字段指定 子类名");
            }
            string subType = typeNameProp.GetString();
            implBean = DataUtil.GetImplTypeByNameOrAlias(bean, subType);
        }
        else
        {
            implBean = bean;
        }

        var fields = new List<DType>();
        foreach (DefField f in implBean.HierarchyFields)
        {
            if (TryGetBeanField(x, f, out var ele))
            {
                if (ele.ValueKind == JsonValueKind.Null || ele.ValueKind == JsonValueKind.Undefined)
                {
                    if (f.CType.IsNullable)
                    {
                        fields.Add(null);
                    }
                    else
                    {
                        throw new Exception($"结构:'{implBean.FullName}' 字段:'{f.Name}' 不能 null or undefined ");
                    }
                }
                else
                {
                    try
                    {
                        fields.Add(f.CType.Apply(this, ele, ass));
                    }
                    catch (DataCreateException dce)
                    {
                        dce.Push(bean, f);
                        throw;
                    }
                    catch (Exception e)
                    {
                        var dce = new DataCreateException(e, "");
                        dce.Push(bean, f);
                        throw dce;
                    }
                }
            }
            else if (f.CType.IsNullable)
            {
                fields.Add(null);
            }
            else
            {
                throw new Exception($"结构:'{implBean.FullName}' 字段:'{f.CurrentVariantNameWithFieldNameOrOrigin}' 缺失");
            }
        }
        return new DBean(type, implBean, fields);
    }

    private List<DType> ReadList(TType type, JsonElement e, DefAssembly ass)
    {
        var list = new List<DType>();
        foreach (var c in e.EnumerateArray())
        {
            list.Add(type.Apply(this, c, ass));
        }
        return list;
    }

    public DType Accept(TArray type, JsonElement x, DefAssembly ass)
    {
        return new DArray(type, ReadList(type.ElementType, x, ass));
    }

    public DType Accept(TList type, JsonElement x, DefAssembly ass)
    {
        return new DList(type, ReadList(type.ElementType, x, ass));
    }

    public DType Accept(TSet type, JsonElement x, DefAssembly ass)
    {
        return new DSet(type, ReadList(type.ElementType, x, ass));
    }

    public DType Accept(TMap type, JsonElement x, DefAssembly ass)
    {
        var map = new Dictionary<DType, DType>();
        foreach (var e in x.EnumerateArray())
        {
            if (e.GetArrayLength() != 2)
            {
                throw new ArgumentException($"json map 类型的 成员数据项:{e} 必须是 [key,value] 形式的列表");
            }
            DType key = type.KeyType.Apply(this, e[0], ass);
            DType value = type.ValueType.Apply(this, e[1], ass);
            if (!map.TryAdd(key, value))
            {
                throw new Exception($"map 的 key:{key} 重复");
            }
        }
        return new DMap(type, map);
    }

    public DType Accept(TDateTime type, JsonElement x, DefAssembly ass)
    {
        return DataUtil.CreateDateTime(x.GetString());
    }
}
