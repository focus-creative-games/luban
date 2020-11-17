using Luban.Common.Utils;
using Luban.Config.Common.RawDefs;
using Luban.Job.Cfg.Validators;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Defs
{
    public class DefField : DefFieldBase
    {
        public DefAssembly Assembly => (DefAssembly)HostType.AssemblyBase;


        public bool RawIsMultiRow { get; }

        public bool IsMultiRow { get; private set; }

        public bool ComputeIsMultiRow()
        {
            if (IsMultiRow)
            {
                return true;
            }

            switch (CType)
            {
                case TBean b: { return IsMultiRow = ((DefBean)b.Bean).IsMultiRow; }
                case TList b: { return IsMultiRow = b.ElementType is TBean b2 && ((DefBean)b2.Bean).IsMultiRow; }
                case TArray b: { return IsMultiRow = b.ElementType is TBean b2 && ((DefBean)b2.Bean).IsMultiRow; }
                case TMap b: { return IsMultiRow = b.ValueType is TBean b2 && ((DefBean)b2.Bean).IsMultiRow; }
                default: return false;
            }
        }


        public string Index { get; }

        public List<string> Groups { get; }

        public DefField IndexField { get; private set; }

        public RefValidator Ref { get; private set; }

        // 对于 two key map, 需要检查 ref,但不为它生成 ref 代码.故只有map类型表才要生成代码
        public bool GenRef => Ref != null && Assembly.GetCfgTable(Ref.FirstTable).IsMapTable;

        public bool HasRecursiveRef => (CType is TBean)
            || (CType is TArray ta && ta.ElementType is TBean)
            || (CType is TList tl && tl.ElementType is TBean)
            || (CType is TMap tm && tm.ValueType is TBean);

        public string CsRefTypeName
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return table.ValueTType.Apply(CsDefineTypeName.Ins);
            }
        }

        public string CsRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{table.ValueTType.Apply(CsDefineTypeName.Ins)} {CsRefVarName};";
            }
        }

        public string JavaRefTypeName
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return table.ValueTType.Apply(JavaDefineTypeName.Ins);
            }
        }

        public string JavaRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{table.ValueTType.Apply(JavaDefineTypeName.Ins)} {JavaRefVarName};";
            }
        }

        public string TsRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{TsRefVarName} : {table.ValueTType.Apply(TsDefineTypeName.Ins)};";
            }
        }

        public string Sep { get; set; }

        // 如果没有指定sep
        // 如果是bean,且指定了sep，则使用此值
        // 如果是vectorN,使用 ,
        public string ActualSep => string.IsNullOrWhiteSpace(Sep) ? (CType is TBean bean ? ((DefBean)bean.Bean).Sep : "") : Sep;

        public List<IValidator> KeyValidators { get; } = new List<IValidator>();

        public List<IValidator> ValueValidators { get; } = new List<IValidator>();

        public List<IValidator> Validators { get; } = new List<IValidator>();

        public string ResourceTag { get; }

        public bool IsResource => !string.IsNullOrEmpty(ResourceTag);

        public string CsRefVarName => $"{CsStyleName}_Ref";

        public string JavaRefVarName => $"{JavaStyleName}_Ref";

        public string TsRefVarName => $"{TsStyleName}_Ref";

        public string JavaGetterName => TypeUtil.ToJavaGetterName(Name);

        public string CppGetterName => JavaGetterName;

        public bool NeedExport => Assembly.NeedExport(this.Groups);

        public TEnum Remapper { get; private set; }

        public CfgField RawDefine { get; }

        public DefField(DefTypeBase host, CfgField f, int idOffset) : base(host, f, idOffset)
        {
            Index = f.Index;
            Sep = f.Sep;
            this.IsMultiRow = this.RawIsMultiRow = f.IsMultiRow;
            ResourceTag = f.Resource;
            this.Validators.AddRange(f.Validators.Select(v => ValidatorFactory.Create(v)));
            this.KeyValidators.AddRange(f.KeyValidators.Select(v => ValidatorFactory.Create(v)));
            this.ValueValidators.AddRange(f.ValueValidators.Select(v => ValidatorFactory.Create(v)));
            this.Groups = f.Groups;
            this.RawDefine = f;
        }

        public override void Compile()
        {
            base.Compile();
            foreach (var v in this.Validators)
            {
                v.Compile(this);
            }

            foreach (var v in this.KeyValidators)
            {
                v.Compile(this);
            }

            foreach (var v in this.ValueValidators)
            {
                v.Compile(this);
            }

            switch (CType)
            {
                case TArray t:
                {
                    if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                    {
                        throw new Exception($"container element type can't be empty bean");
                    }
                    break;
                }
                case TList t:
                {
                    if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                    {
                        throw new Exception($"container element type can't be empty bean");
                    }
                    break;
                }
            }

            if (IsMultiRow && !CType.IsCollection)
            {
                throw new Exception($"只有容器类型才支持 multi_line 属性");
            }

            if (string.IsNullOrEmpty(Sep) && CType is TBean bean)
            {
                Sep = bean.GetBeanAs<DefBean>().Sep;
            }
            if (!string.IsNullOrEmpty(Index))
            {
                if ((CType is TArray tarray) && (tarray.ElementType is TBean b))
                {
                    if ((IndexField = b.GetBeanAs<DefBean>().GetField(Index)) == null)
                    {
                        throw new Exception($"type:{HostType.FullName} field:{Name} index:{Index}. index not exist");
                    }
                }
                else if ((CType is TList tlist) && (tlist.ElementType is TBean tb))
                {
                    if ((IndexField = tb.GetBeanAs<DefBean>().GetField(Index)) == null)
                    {
                        throw new Exception($"type:{HostType.FullName} field:{Name} index:{Index}. index not exist");
                    }
                }
                else
                {
                    throw new Exception($"type:{HostType.FullName} field:{Name} index:{Index}. only array:bean or list:bean support index");
                }
            }

            if (!CType.IsCollection && !(CType is TBean))
            {
                this.Ref = (RefValidator)this.Validators.FirstOrDefault(v => v is RefValidator);
            }

            if (!string.IsNullOrEmpty(this.RawDefine.Converter))
            {
                this.Remapper = AssemblyBase.GetDefTType(HostType.Namespace, this.RawDefine.Converter, this.IsNullable) as TEnum;
                if (this.Remapper == null)
                {
                    throw new Exception($"type:{HostType.FullName} field:{Name} converter:{this.RawDefine.Converter} not exists");
                }
            }

            // 检查所引用的表是否导出了
            if (NeedExport)
            {
                var allValidators = new List<IValidator>(this.Validators);
                allValidators.AddRange(this.KeyValidators);
                allValidators.AddRange(this.ValueValidators);

                foreach (var val in allValidators)
                {
                    if (val is RefValidator refValidator && !Assembly.GetCfgTable(refValidator.FirstTable).NeedExport)
                    {
                        throw new Exception($"type:{HostType.FullName} field:{Name} ref 引用的表:{refValidator.FirstTable} 没有导出");
                    }
                }
            }
        }

        public override void PostCompile()
        {
            base.PostCompile();

            // 检查 字段类型 与 所引用的表的key是否一致

            foreach (var val in KeyValidators)
            {
                if (val is RefValidator refValidator)
                {
                    var cfgTable = Assembly.GetCfgTable(refValidator.FirstTable);
                    if (CType is TMap mapType)
                    {
                        if (mapType.KeyType.GetType() != cfgTable.KeyTType.GetType())
                        {
                            throw new Exception($"type:{HostType.FullName} field:{Name} key类型:{mapType.KeyType.GetType()} 与 被引用的表:{cfgTable.FullName} key类型:{cfgTable.KeyTType.GetType()} 不一致");
                        }
                    }
                    else
                    {
                        throw new Exception($"type:{HostType.FullName} field:{Name} 不是 map类型. 不能指定 key_validator 引用");
                    }
                }
            }

            var remainValidators = new List<IValidator>(this.Validators);
            remainValidators.AddRange(this.ValueValidators);
            foreach (var val in remainValidators)
            {
                if (val is RefValidator refValidator)
                {
                    var cfgTable = Assembly.GetCfgTable(refValidator.FirstTable);
                    TType valueType;
                    switch (CType)
                    {
                        case TArray ta: valueType = ta.ElementType; break;
                        case TList tl: valueType = tl.ElementType; break;
                        case TSet ts: valueType = ts.ElementType; break;
                        case TMap tm: valueType = tm.ValueType; break;
                        default: valueType = CType; break;
                    }

                    if (valueType.GetType() != cfgTable.KeyTType.GetType())
                    {
                        throw new Exception($"type:{HostType.FullName} field:{Name} 类型:{valueType.GetType()} 与 被引用的表:{cfgTable.FullName} key类型:{cfgTable.KeyTType.GetType()} 不一致");
                    }

                }
            }
        }
    }
}
