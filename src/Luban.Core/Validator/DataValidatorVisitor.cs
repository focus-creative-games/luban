using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Validator;

public class DataValidatorVisitor : TypeActionVisitorAdaptor<DType>
{
    private readonly Stack<object> _path = new();

    public Stack<object> Path => _path;

    public DataValidatorContext Ctx { get; }

    public Record CurrentValidateRecord { get; set; }

    public DataValidatorVisitor(DataValidatorContext ctx)
    {
        Ctx = ctx;
    }

    public void ValidateTable(DefTable table, List<Record> records)
    {
        var keyIndex = table.IndexFieldIdIndex;

        foreach (Record r in records)
        {
            if (DataUtil.IsUnchecked(r))
            {
                continue;
            }
            CurrentValidateRecord = r;
            DBean data = r.Data;
            _path.Clear();
            _path.Push(table.FullName);
            if (table.IsMapTable)
            {
                _path.Push(data.Fields[keyIndex]);
            }
            if (table.ValueTType.Validators.Count > 0)
            {
                foreach (var p in table.ValueTType.Validators)
                {
                    p.Validate(Ctx, table.ValueTType, data);
                }
            }
            table.ValueTType.Apply(this, data);
        }
    }

    private void AcceptListLike(TType elementType, List<DType> eles)
    {
        if (elementType.Validators.Count > 0)
        {
            int index = 0;
            foreach (var value in eles)
            {
                _path.Push(index++);
                foreach (var v in elementType.Validators)
                {
                    if (value == null)
                    {
                        continue;
                    }
                    v.Validate(Ctx, elementType, value);
                    elementType.Apply(this, value);
                }
                _path.Pop();
            }
        }

        if (elementType.IsBean || elementType.IsCollection)
        {
            int index = 0;
            foreach (var value in eles)
            {
                if (value == null)
                {
                    continue;
                }
                _path.Push(index++);
                elementType.Apply(this, value);
                _path.Pop();
            }
        }
    }

    public override void Accept(TBean type, DType x)
    {
        var beanData = (DBean)x;
        var defFields = ((DefBean)type.DefBean.Assembly.GetDefType(beanData.ImplType.FullName)).HierarchyFields;// beanData.ImplType.HierarchyFields;
        int i = 0;
        foreach (var fieldValue in beanData.Fields)
        {
            if (fieldValue == null)
            {
                i++;
                continue;
            }
            var defField = defFields[i++];
            _path.Push(defField.Name);

            var fieldType = defField.CType;

            if (fieldType.Validators.Count > 0)
            {
                foreach (var p in fieldType.Validators)
                {
                    p.Validate(Ctx, fieldType, fieldValue);
                }
            }
            fieldType.Apply(this, fieldValue);
            _path.Pop();
        }
    }

    public override void Accept(TArray type, DType x)
    {
        AcceptListLike(type.ElementType, ((DArray)x).Datas);
    }

    public override void Accept(TList type, DType x)
    {
        AcceptListLike(type.ElementType, ((DList)x).Datas);
    }

    public override void Accept(TSet type, DType x)
    {
        AcceptListLike(type.ElementType, ((DSet)x).Datas);
    }

    public override void Accept(TMap type, DType x)
    {
        var keyType = type.KeyType;
        var valueType = type.ValueType;
        if (keyType.Validators.Count > 0 || valueType.Validators.Count > 0)
        {
            foreach (var e in ((DMap)x).Datas)
            {
                _path.Push(e.Key);
                if (e.Key != null && keyType.Validators.Count > 0)
                {
                    foreach (var v in keyType.Validators)
                    {
                        v.Validate(Ctx, keyType, e.Key);
                        keyType.Apply(this, e.Key);
                    }
                }
                if (valueType != null && valueType.Validators.Count > 0)
                {
                    foreach (var v in valueType.Validators)
                    {
                        v.Validate(Ctx, valueType, e.Value);
                        if (e.Value != null)
                        {
                            valueType.Apply(this, e.Value);
                        }
                    }
                }
                _path.Pop();
            }
        }
        if (valueType is TBean)
        {
            foreach (var e in ((DMap)x).Datas)
            {
                _path.Push(e.Key);
                if (e.Value != null)
                {
                    valueType.Apply(this, e.Value);
                }
                _path.Pop();
            }
        }
    }
}
