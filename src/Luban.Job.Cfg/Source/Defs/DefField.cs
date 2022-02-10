using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Utils;
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

        // 如果ref了多个表，不再生成 xxx_ref之类的字段，也不会resolve
        public bool GenRef => Ref != null && Ref.GenRef;

        public bool HasRecursiveRef => (CType is TBean tb && HostType.AssemblyBase.GetExternalTypeMapper(tb) == null)
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

        public override void Compile()
        {
            base.Compile();

            ValidatorUtil.CreateValidators(CType);
            var selfRef = this.CType.Processors.Find(v => v is RefValidator);
            if (selfRef != null)
            {
                this.Ref = (RefValidator)selfRef;
            }
        }

        private void ValidateIndex()
        {
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

        public override void PostCompile()
        {
            base.PostCompile();
            ValidateIndex();
        }
    }
}
