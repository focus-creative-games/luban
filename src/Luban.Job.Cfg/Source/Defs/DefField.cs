using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.RawDefs;
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

        public string Index { get; private set; }

        public List<string> Groups { get; }

        public DefField IndexField { get; private set; }

        public RefValidator Ref { get; private set; }

        //public RefValidator KeyRef { get; private set; }

        //public RefValidator ValueRef { get; private set; }

        //public List<IValidator> Validators { get; } = new List<IValidator>();

        //public List<IValidator> KeyValidators { get; } = new List<IValidator>();

        //public List<IValidator> ValueValidators { get; } = new List<IValidator>();

        // 如果ref了多个表，不再生成 xxx_ref之类的字段，也不会resolve
        public bool GenRef => Ref != null && Ref.Tables.Count == 1;

        public bool HasRecursiveRef => (CType.IsBean)
            || (CType is TArray ta && ta.ElementType.IsBean)
            || (CType is TList tl && tl.ElementType.IsBean)
            || (CType is TMap tm && tm.ValueType.IsBean);

#if !LUBAN_LITE
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
                return $"{table.ValueTType.Apply(CsDefineTypeName.Ins)} {RefVarName} {{ get; private set; }}";
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
                return $"{table.ValueTType.Apply(JavaDefineTypeName.Ins)} {RefVarName};";
            }
        }

        public string CppRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{table.ValueTType.Apply(CppDefineTypeName.Ins)} {RefVarName};";
            }
        }

        public string TsRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{RefVarName} : {table.ValueTType.Apply(TypescriptDefineTypeNameVisitor.Ins)}{(IsNullable ? "" : " = undefined!")}";
            }
        }
#endif

        public string RefVarName => $"{ConventionName}_Ref";


        public string ConventionGetterName => TypeUtil.ToJavaGetterName(Name);

        //public string JavaGetterName => TypeUtil.ToJavaGetterName(Name);

        //public string CppGetterName => JavaGetterName;

        public bool NeedExport => Assembly.NeedExport(this.Groups);

        public TEnum Remapper { get; private set; }

        public CfgField RawDefine { get; }

        public string GetTextKeyName(string name) => name + TText.L10N_FIELD_SUFFIX;

        public bool GenTextKey => this.CType is TText;

        public bool HasRecursiveText => HasRecursiveRef;


        public DefField(DefTypeBase host, CfgField f, int idOffset) : base(host, f, idOffset)
        {
            this.Groups = f.Groups;
            this.RawDefine = f;
        }


        private void CompileValidatorsForType(TType type)
        {
            foreach (var valName in ValidatorFactory.ValidatorNames)
            {
                if (type.Tags != null && type.Tags.TryGetValue(valName, out var valValue))
                {
                    type.Processors.Add(ValidatorFactory.Create(valName, valValue));
                }
            }
        }

        private void CompileValidatorsForArrayLink(TType elementType)
        {
            CompileValidatorsForType(elementType);

            var valueRef = this.CType.Processors.Find(v => v is RefValidator);
            if (valueRef != null)
            {
                this.CType.Processors.Remove(valueRef);
                elementType.Processors.Add(valueRef);
            }
            var valuePath = this.CType.Processors.Find(v => v is PathValidator);
            if (valuePath != null)
            {
                this.CType.Processors.Remove(valuePath);
                elementType.Processors.Add(valuePath);
            }
        }

        public override void Compile()
        {
            base.Compile();

            CompileValidatorsForType(CType);

            switch (this.CType)
            {
                case TArray ta:
                {
                    CompileValidatorsForArrayLink(ta.ElementType);
                    break;
                }
                case TList ta:
                {
                    CompileValidatorsForArrayLink(ta.ElementType);
                    break;
                }
                case TSet ta:
                {
                    CompileValidatorsForArrayLink(ta.ElementType);
                    break;
                }
                case TMap ta:
                {
                    CompileValidatorsForType(ta.KeyType);
                    CompileValidatorsForType(ta.ValueType);
                    break;
                }
                default:
                {
                    var selfRef = this.CType.Processors.Find(v => v is RefValidator);
                    if (selfRef != null)
                    {
                        this.Ref = (RefValidator)selfRef;
                    }
                    break;
                }
            }

            switch (CType)
            {
                case TArray t:
                {
                    if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                    {
                        throw new Exception($"container element type:'{e.Bean.FullName}' can't be empty bean");
                    }
                    if (t.ElementType is TText)
                    {
                        throw new Exception($"bean:{HostType.FullName} field:{Name} container element type can't text");
                    }
                    break;
                }
                case TList t:
                {
                    if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                    {
                        throw new Exception($"container element type:'{e.Bean.FullName}' can't be empty bean");
                    }
                    if (t.ElementType is TText)
                    {
                        throw new Exception($"bean:{HostType.FullName} field:{Name} container element type can't text");
                    }
                    break;
                }
                case TSet t:
                {
                    if (t.ElementType is TText)
                    {
                        throw new Exception($"bean:{HostType.FullName} field:{Name} container element type can't text");
                    }
                    break;
                }
                case TMap t:
                {
                    if (t.KeyType is TText)
                    {
                        throw new Exception($"bean:{HostType.FullName} field:{Name} container key type can't text");
                    }
                    if (t.ValueType is TText)
                    {
                        throw new Exception($"bean:{HostType.FullName} field:{Name} container value type can't text");
                    }
                    break;
                }
            }
        }

        private void ValidateRef(RefValidator val, TType refVarType)
        {
            foreach (var table in val.Tables)
            {
                var cfgTable = Assembly.GetCfgTable(RefValidator.GetActualTableName(table));
                if (cfgTable == null)
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' ref 引用的表:'{table}' 不存在");
                }
                if (!cfgTable.NeedExport)
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' ref 引用的表:'{table}' 没有导出");
                }
                if (!cfgTable.IsMapTable)
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' ref 引用的表:'{table}'不是普通表，无法进行引用检查");
                }
                var keyType = cfgTable.KeyTType;
                if (keyType.GetType() != refVarType.GetType())
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' 类型:'{refVarType}' 与 被引用的表:'{cfgTable.FullName}' key类型:'{keyType}' 不一致");
                }
            }
        }

        private void CompileValidator(TType type)
        {
            foreach (var p in CType.Processors)
            {
                if (p is IValidator val)
                {
                    val.Compile(this);
                    if (val is RefValidator refVal)
                    {
                        ValidateRef(refVal, type);
                    }
                }
            }
        }

        public override void PostCompile()
        {
            base.PostCompile();

            CompileValidator(CType);


            switch (this.CType)
            {
                case TArray ta:
                {
                    CompileValidator(ta.ElementType);
                    break;
                }
                case TList ta:
                {
                    CompileValidator(ta.ElementType);
                    break;
                }
                case TSet ta:
                {
                    CompileValidator(ta.ElementType);
                    break;
                }
                case TMap ta:
                {
                    CompileValidator(ta.KeyType);
                    CompileValidator(ta.ValueType);
                    break;
                }
            }

            Index = CType.GetTag("index");

            if (!string.IsNullOrEmpty(Index))
            {
                if ((CType is TArray tarray) && (tarray.ElementType is TBean b))
                {
                    if ((IndexField = b.GetBeanAs<DefBean>().GetField(Index)) == null)
                    {
                        throw new Exception($"type:'{HostType.FullName}' field:'{Name}' index:'{Index}'. index not exist");
                    }
                }
                else if ((CType is TList tlist) && (tlist.ElementType is TBean tb))
                {
                    if ((IndexField = tb.GetBeanAs<DefBean>().GetField(Index)) == null)
                    {
                        throw new Exception($"type:'{HostType.FullName}' field:'{Name}' index:'{Index}'. index not exist");
                    }
                }
                else
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' index:'{Index}'. only array:bean or list:bean support index");
                }
            }
        }
    }
}
