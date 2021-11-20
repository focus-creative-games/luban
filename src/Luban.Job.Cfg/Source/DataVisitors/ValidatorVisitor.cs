using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Validators;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataVisitors
{
    public class ValidatorVisitor : ITypeActionVisitor<DType>
    {
        public const string TAG_UNCHECKED = "unchecked";

        private readonly Stack<object> _path = new Stack<object>();

        public Stack<object> Path => _path;

        public ValidatorContext Ctx { get; }

        public Record CurrentValidateRecord { get; set; }

        public ValidatorVisitor(ValidatorContext ctx)
        {
            Ctx = ctx;
        }

        public void ValidateTable(DefTable table, List<Record> records)
        {
            var keyIndex = table.IndexFieldIdIndex;

            foreach (Record r in records)
            {
                if (r.Tags != null && r.Tags.Count > 0 && r.Tags.Contains(TAG_UNCHECKED))
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
                if (table.ValueTType.Processors.Count > 0)
                {
                    foreach (var p in table.ValueTType.Processors)
                    {
                        if (p is IValidator v)
                        {
                            v.Validate(Ctx, table.ValueTType, data);
                        }
                    }
                }
                table.ValueTType.Apply(this, data);
            }
        }

        private void AcceptListLike(TType elementType, List<DType> eles)
        {
            if (elementType.Processors.Count > 0)
            {
                int index = 0;
                foreach (var value in eles)
                {
                    _path.Push(index++);
                    foreach (var v in elementType.Processors)
                    {
                        if (v is IValidator eleVal)
                        {
                            eleVal.Validate(Ctx, elementType, value);
                            if (value != null)
                            {
                                elementType.Apply(this, value);
                            }
                        }
                    }
                    _path.Pop();
                }
            }

            if (elementType is TBean)
            {
                int index = 0;
                foreach (var value in eles)
                {
                    _path.Push(index++);
                    if (value != null)
                    {
                        elementType.Apply(this, value);
                    }
                    _path.Pop();
                }
            }
        }

        public void Accept(TBool type, DType x)
        {

        }

        public void Accept(TByte type, DType x)
        {

        }

        public void Accept(TShort type, DType x)
        {

        }

        public void Accept(TFshort type, DType x)
        {

        }

        public void Accept(TInt type, DType x)
        {

        }

        public void Accept(TFint type, DType x)
        {

        }

        public void Accept(TLong type, DType x)
        {

        }

        public void Accept(TFlong type, DType x)
        {

        }

        public void Accept(TFloat type, DType x)
        {

        }

        public void Accept(TDouble type, DType x)
        {

        }

        public void Accept(TEnum type, DType x)
        {

        }

        public void Accept(TString type, DType x)
        {

        }

        public void Accept(TBytes type, DType x)
        {

        }

        public void Accept(TText type, DType x)
        {

        }

        public void Accept(TBean type, DType x)
        {
            var beanData = (DBean)x;
            var defFields = ((DefBean)type.Bean.AssemblyBase.GetDefType(beanData.ImplType.FullName)).HierarchyFields;// beanData.ImplType.HierarchyFields;
            int i = 0;
            foreach (var fieldValue in beanData.Fields)
            {
                var defField = (DefField)defFields[i++];
                _path.Push(defField.Name);

                var fieldType = defField.CType;

                if (fieldType.Processors.Count > 0)
                {
                    foreach (var p in fieldType.Processors)
                    {
                        if (p is IValidator val)
                        {
                            val.Validate(Ctx, fieldType, fieldValue);
                        }
                    }
                }
                if (fieldValue != null)
                {
                    fieldType.Apply(this, fieldValue);
                }
                _path.Pop();
            }
        }

        public void Accept(TArray type, DType x)
        {
            AcceptListLike(type.ElementType, ((DArray)x).Datas);
        }

        public void Accept(TList type, DType x)
        {
            AcceptListLike(type.ElementType, ((DList)x).Datas);
        }

        public void Accept(TSet type, DType x)
        {
            AcceptListLike(type.ElementType, ((DSet)x).Datas);
        }

        public void Accept(TMap type, DType x)
        {
            var keyType = type.KeyType;
            var valueType = type.ValueType;
            if (keyType.Processors.Count > 0 || valueType.Processors.Count > 0)
            {
                foreach (var e in ((DMap)x).Datas)
                {
                    _path.Push(e.Key);
                    if (keyType.Processors.Count > 0)
                    {
                        foreach (var v in keyType.Processors)
                        {
                            if (v is IValidator eleVal)
                            {
                                eleVal.Validate(Ctx, keyType, e.Key);
                                keyType.Apply(this, e.Key);
                            }
                        }
                    }
                    if (valueType.Processors.Count > 0)
                    {
                        foreach (var v in valueType.Processors)
                        {
                            if (v is IValidator eleVal)
                            {
                                eleVal.Validate(Ctx, valueType, e.Value);
                                if (e.Value != null)
                                {
                                    valueType.Apply(this, e.Value);
                                }
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

        public void Accept(TVector2 type, DType x)
        {

        }

        public void Accept(TVector3 type, DType x)
        {

        }

        public void Accept(TVector4 type, DType x)
        {

        }

        public void Accept(TDateTime type, DType x)
        {

        }
    }
}
