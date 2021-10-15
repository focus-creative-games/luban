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

        public string Index { get; }

        public List<string> Groups { get; }

        public DefField IndexField { get; private set; }

        public RefValidator Ref { get; private set; }

        public RefValidator KeyRef { get; private set; }

        public RefValidator ValueRef { get; private set; }

        public List<IValidator> Validators { get; } = new List<IValidator>();

        public List<IValidator> KeyValidators { get; } = new List<IValidator>();

        public List<IValidator> ValueValidators { get; } = new List<IValidator>();

        // 如果ref了多个表，不再生成 xxx_ref之类的字段，也不会resolve
        public bool GenRef => Ref != null && Ref.Tables.Count == 1;

        public bool HasRecursiveRef => (CType.IsBean)
            || (CType is TArray ta && ta.ElementType.IsBean)
            || (CType is TList tl && tl.ElementType.IsBean)
            || (CType is TMap tm && tm.ValueType.IsBean);

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
                return $"{table.ValueTType.Apply(CsDefineTypeName.Ins)} {CsRefVarName} {{ get; private set; }}";
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

        public string CppRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{table.ValueTType.Apply(CppDefineTypeName.Ins)} {CppRefVarName};";
            }
        }

        public string TsRefValidatorDefine
        {
            get
            {
                var table = Assembly.GetCfgTable(Ref.FirstTable);
                return $"{TsRefVarName} : {table.ValueTType.Apply(TypescriptDefineTypeNameVisitor.Ins)}{(IsNullable ? "" : " = undefined!")}";
            }
        }

        public string CsRefVarName => $"{CsStyleName}_Ref";

        public string JavaRefVarName => $"{JavaStyleName}_Ref";

        public string CppRefVarName => $"{CsStyleName}_Ref";

        public string TsRefVarName => $"{TsStyleName}_Ref";

        public string JavaGetterName => TypeUtil.ToJavaGetterName(Name);

        public string CppGetterName => JavaGetterName;

        public bool NeedExport => Assembly.NeedExport(this.Groups);

        public TEnum Remapper { get; private set; }

        public CfgField RawDefine { get; }

        public string GetTextKeyName(string name) => name + TText.L10N_FIELD_SUFFIX;

        public bool GenTextKey => this.CType is TText;

        public bool HasRecursiveText => HasRecursiveRef;


        public DefField(DefTypeBase host, CfgField f, int idOffset) : base(host, f, idOffset)
        {
            Index = f.Index;
            this.Groups = f.Groups;
            this.RawDefine = f;
        }

        public override void Compile()
        {
            base.Compile();

            switch (this.CType)
            {
                case TArray ta:
                {
                    if (ta.ElementType.Tags.TryGetValue("ref", out string refStr))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", refStr));
                    }
                    if (CType.Tags.TryGetValue("ref", out string refStr2))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", refStr2));
                    }
                    if (ta.ElementType.Tags.TryGetValue("path", out string PathStr))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", PathStr));
                    }
                    if (CType.Tags.TryGetValue("path", out string pathStr2))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", pathStr2));
                    }
                    break;
                }
                case TList ta:
                {
                    if (ta.ElementType.Tags.TryGetValue("ref", out string refStr))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", refStr));
                    }
                    if (CType.Tags.TryGetValue("ref", out string refStr2))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", refStr2));
                    }
                    if (ta.ElementType.Tags.TryGetValue("path", out string PathStr))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", PathStr));
                    }
                    if (CType.Tags.TryGetValue("path", out string pathStr2))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", pathStr2));
                    }
                    break;
                }
                case TSet ta:
                {
                    if (ta.ElementType.Tags.TryGetValue("ref", out string refStr))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", refStr));
                    }
                    if (CType.Tags.TryGetValue("ref", out string refStr2))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", refStr2));
                    }
                    if (ta.ElementType.Tags.TryGetValue("path", out string PathStr))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", PathStr));
                    }
                    if (CType.Tags.TryGetValue("path", out string pathStr2))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", pathStr2));
                    }
                    break;
                }
                case TMap ta:
                {
                    if (ta.KeyType.Tags.TryGetValue("ref", out string keyRefStr))
                    {
                        this.KeyValidators.Add(this.KeyRef = (RefValidator)ValidatorFactory.Create("ref", keyRefStr));
                    }
                    if (ta.ValueType.Tags.TryGetValue("ref", out string valueRefStr))
                    {
                        this.ValueValidators.Add(this.ValueRef = (RefValidator)ValidatorFactory.Create("ref", valueRefStr));
                    }
                    if (ta.KeyType.Tags.TryGetValue("path", out string PathStr))
                    {
                        this.KeyValidators.Add(ValidatorFactory.Create("path", PathStr));
                    }
                    if (ta.ValueType.Tags.TryGetValue("path", out string pathStr2))
                    {
                        this.ValueValidators.Add(ValidatorFactory.Create("path", pathStr2));
                    }
                    break;
                }
                default:
                {
                    if (CType.Tags.TryGetValue("ref", out string refStr2))
                    {
                        this.Ref = (RefValidator)ValidatorFactory.Create("ref", refStr2);
                    }
                    if (CType.Tags.TryGetValue("path", out string pathStr2))
                    {
                        this.Validators.Add(ValidatorFactory.Create("path", pathStr2));
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

        public override void PostCompile()
        {
            base.PostCompile();

            foreach (var val in KeyValidators.Concat(ValueValidators).Concat(Validators))
            {
                val.Compile(this);
            }

            if (Ref != null)
            {
                ValidateRef(Ref, CType);
            }
            if (KeyRef != null)
            {
                ValidateRef(KeyRef, (CType as TMap).KeyType);
            }
            if (ValueRef != null)
            {
                switch (this.CType)
                {
                    case TArray ta:
                    {
                        ValidateRef(ValueRef, ta.ElementType);
                        break;
                    }
                    case TList ta:
                    {
                        ValidateRef(ValueRef, ta.ElementType);
                        break;
                    }
                    case TSet ta:
                    {
                        ValidateRef(ValueRef, ta.ElementType);
                        break;
                    }
                    case TMap ta:
                    {
                        ValidateRef(ValueRef, ta.ValueType);
                        break;
                    }
                }
            }
        }
    }
}
