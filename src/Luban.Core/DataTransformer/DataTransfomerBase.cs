using Luban.Datas;
using Luban.DataVisitors;
using Luban.Types;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataTransformer;

public abstract class DataTransfomerBase : IDataTransformer, IDataFuncVisitor2<DType>
{
    public DType Transform(DType originalData, TType type)
    {
        if (originalData == null)
        {
            return null;
        }
        return originalData.Apply(this, type);
    }

    DType IDataFuncVisitor2<DType>.Accept(DBool data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DByte data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DShort data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DInt data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DLong data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DFloat data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DDouble data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DEnum data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DString data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DDateTime data, TType type)
    {
        return data;
    }

    DType IDataFuncVisitor2<DType>.Accept(DBean data, TType type)
    {
        var defFields = data.ImplType.HierarchyFields;
        int i = 0;
        List<DType> newFields = null;
        foreach (var fieldValue in data.Fields)
        {
            if (fieldValue == null)
            {
                i++;
                continue;
            }
            var defField = defFields[i];
            var fieldType = defField.CType;
            DType newFieldValue = fieldValue.Apply(this, fieldType);
            if (newFieldValue != fieldValue)
            {
                if (newFields == null)
                {
                    newFields = new List<DType>(data.Fields);
                }
                newFields[i] = newFieldValue;
            }
            ++i;
        }
        return newFields == null ? data : new DBean(data.TType, data.ImplType, newFields);
    }

    DType IDataFuncVisitor2<DType>.Accept(DArray data, TType type)
    {
        TType eleType = type.ElementType;
        List<DType> newDatas = null;
        int index = 0;
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                ++index;
                continue;
            }
            DType newEle = ele.Apply(this, eleType);
            if (newEle != ele)
            {
                if (newDatas == null)
                {
                    newDatas = new List<DType>(data.Datas);
                }
                newDatas[index] = newEle;
            }
            ++index;
        }
        return newDatas == null ? data : new DArray(data.Type, newDatas);
    }

    DType IDataFuncVisitor2<DType>.Accept(DList data, TType type)
    {
        TType eleType = type.ElementType;
        List<DType> newDatas = null;
        int index = 0;
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                ++index;
                continue;
            }
            DType newEle = ele.Apply(this, eleType);
            if (newEle != ele)
            {
                if (newDatas == null)
                {
                    newDatas = new List<DType>(data.Datas);
                }
                newDatas[index] = newEle;
            }
            ++index;
        }
        return newDatas == null ? data : new DList(data.Type, newDatas);
    }

    DType IDataFuncVisitor2<DType>.Accept(DSet data, TType type)
    {
        TType eleType = type.ElementType;
        List<DType> newDatas = null;
        int index = 0;
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                ++index;
                continue;
            }
            DType newEle = ele.Apply(this, eleType);
            if (newEle != ele)
            {
                if (newDatas == null)
                {
                    newDatas = new List<DType>(data.Datas);
                }
                newDatas[index] = newEle;
            }
            ++index;
        }
        return newDatas == null ? data : new DSet(data.Type, newDatas);
    }

    DType IDataFuncVisitor2<DType>.Accept(DMap data, TType type)
    {
        TMap mapType = (TMap)type;
        bool dirty = false;
        foreach (var ele in data.Datas)
        {
            DType newKey = ele.Key.Apply(this, mapType.KeyType);
            DType newValue = ele.Value.Apply(this, mapType.ValueType);
            if (newKey != ele.Key || newValue != ele.Value)
            {
                dirty = true;
                break;
            }
        }
        if (!dirty)
        {
            return data;
        }

        var newDatas = new Dictionary<DType, DType>();
        foreach (var ele in data.Datas)
        {
            DType newKey = ele.Key.Apply(this, mapType.KeyType);
            DType newValue = ele.Value.Apply(this, mapType.ValueType);
            newDatas[newKey] = newValue;
        }
        return new DMap(data.Type, newDatas);
    }
}
