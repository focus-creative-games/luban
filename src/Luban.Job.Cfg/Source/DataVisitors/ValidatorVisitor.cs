using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataVisitors
{
    public class ValidatorVisitor : IDataActionVisitor<DefAssembly>
    {
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
            DefAssembly ass = table.Assembly;
            var keyIndex = table.IndexFieldIdIndex;

            foreach (Record r in records)
            {
                CurrentValidateRecord = r;
                DBean data = r.Data;
                _path.Clear();
                _path.Push(table.FullName);
                if (table.IsMapTable)
                {
                    _path.Push(data.Fields[keyIndex]);
                }
                else if (table.IsTwoKeyMapTable)
                {
                    _path.Push(data.Fields[keyIndex]);
                    _path.Push(data.Fields[table.IndexFieldIdIndex2]);
                }
                Accept(data, ass);
            }
        }

        public void Accept(DBool type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DByte type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DShort type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFshort type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DInt type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFint type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DLong type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFlong type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFloat type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DDouble type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DEnum type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DString type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DBytes type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DText type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DBean record, DefAssembly assembly)
        {
            if (record.ImplType == null)
            {
                return;
            }
            var defFields = record.ImplType.HierarchyFields;
            int i = 0;
            foreach (var fieldValue in record.Fields)
            {
                var defField = (DefField)defFields[i++];
                _path.Push(defField.Name);
                switch (defField.CType)
                {
                    case TArray a:
                    {
                        if (defField.ValueValidators.Count > 0)
                        {
                            var arr = (DArray)fieldValue;
                            int index = 0;
                            foreach (var value in arr.Datas)
                            {
                                _path.Push(index++);
                                foreach (var v in defField.ValueValidators)
                                {
                                    v.Validate(Ctx, value, defField.IsNullable);
                                }
                                _path.Pop();
                            }

                        }
                        if (a.ElementType is TBean)
                        {
                            var arr = (DArray)fieldValue;
                            int index = 0;
                            foreach (var value in arr.Datas)
                            {
                                _path.Push(index++);
                                Accept((DBean)value, assembly);
                                _path.Pop();
                            }

                        }
                        break;
                    }
                    case TList b:
                    {
                        if (defField.ValueValidators.Count > 0)
                        {
                            var arr = (DList)fieldValue;
                            int index = 0;
                            foreach (var value in arr.Datas)
                            {
                                _path.Push(index++);
                                foreach (var v in defField.ValueValidators)
                                {
                                    v.Validate(Ctx, value, false);
                                }
                                _path.Pop();
                            }

                        }
                        if (b.ElementType is TBean tb)
                        {
                            var arr = (DList)fieldValue;
                            int index = 0;
                            foreach (var value in arr.Datas)
                            {
                                _path.Push(index++);
                                Accept((DBean)value, assembly);
                                _path.Pop();
                            }


                            if (defField.IndexField != null)
                            {
                                var indexSet = new HashSet<DType>();
                                if (!tb.GetBeanAs<DefBean>().TryGetField(defField.Index, out var _, out var indexOfIndexField))
                                {
                                    throw new Exception("impossible");
                                }
                                foreach (var value in arr.Datas)
                                {
                                    _path.Push(index++);
                                    DType indexValue = ((DBean)value).Fields[indexOfIndexField];
                                    if (!indexSet.Add(indexValue))
                                    {
                                        throw new Exception($"{TypeUtil.MakeFullName(_path)} index:{indexValue} 重复");
                                    }
                                    _path.Pop();
                                }
                            }
                        }
                        break;
                    }
                    case TSet c:
                    {
                        if (defField.ValueValidators.Count > 0)
                        {
                            var arr = (DSet)fieldValue;
                            foreach (var value in arr.Datas)
                            {
                                foreach (var v in defField.ValueValidators)
                                {
                                    v.Validate(Ctx, value, false);
                                }
                            }

                        }
                        break;
                    }

                    case TMap m:
                    {
                        DMap map = (DMap)fieldValue;
                        if (defField.KeyValidators.Count > 0)
                        {
                            foreach (var key in map.Datas.Keys)
                            {
                                _path.Push(key);
                                foreach (var v in defField.KeyValidators)
                                {
                                    v.Validate(Ctx, key, false);
                                }
                                _path.Pop();
                            }
                        }
                        if (defField.ValueValidators.Count > 0)
                        {
                            foreach (var value in map.Datas.Values)
                            {
                                _path.Push(value);
                                foreach (var v in defField.ValueValidators)
                                {
                                    v.Validate(Ctx, value, false);
                                }

                                if (value is DBean dv)
                                {
                                    Accept(dv, assembly);
                                }
                                _path.Pop();
                            }
                        }
                        break;
                    }
                    case TBean n:
                    {
                        Accept((DBean)fieldValue, assembly);
                        break;
                    }
                    default:
                    {
                        if (defField.Validators.Count > 0)
                        {
                            foreach (var v in defField.Validators)
                            {
                                v.Validate(Ctx, fieldValue, defField.IsNullable);
                            }
                        }
                        break;
                    }
                }
                _path.Pop();
            }
        }

        public void Accept(DArray type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DList type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DSet type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DMap type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DVector2 type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DVector3 type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DVector4 type, DefAssembly x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DDateTime type, DefAssembly x)
        {
            throw new NotImplementedException();
        }
    }
}
