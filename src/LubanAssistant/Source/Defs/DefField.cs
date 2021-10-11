using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.RawDefs;
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


        public string Sep { get; set; }

        // 如果没有指定sep
        // 如果是bean,且指定了sep，则使用此值
        // 如果是vectorN,使用 ,
        public string ActualSep => string.IsNullOrWhiteSpace(Sep) ? (CType is TBean bean ? ((DefBean)bean.Bean).Sep : "") : Sep;

        public bool NeedExport => Assembly.NeedExport(this.Groups);

        public TEnum Remapper { get; private set; }

        public CfgField RawDefine { get; }

        public string GetTextKeyName(string name) => name + TText.L10N_FIELD_SUFFIX;

        public bool GenTextKey => this.CType is TText;


        public string DefaultValue { get; }

        public DType DefalutDtypeValue { get; private set; }

        public bool IsRowOrient { get; }

        public DefField(DefTypeBase host, CfgField f, int idOffset) : base(host, f, idOffset)
        {
            Index = f.Index;
            Sep = f.Sep;
            this.IsMultiRow = this.RawIsMultiRow = f.IsMultiRow;
            this.Groups = f.Groups;
            this.RawDefine = f;
            this.DefaultValue = f.DefaultValue;
            this.IsRowOrient = f.IsRowOrient;
        }

        public override void Compile()
        {
            base.Compile();

            if (!string.IsNullOrWhiteSpace(this.DefaultValue))
            {
                this.DefalutDtypeValue = CType.Apply(StringDataCreator.Ins, this.DefaultValue);
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

            if (IsMultiRow && !CType.IsCollection && !CType.IsBean)
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

            if (!string.IsNullOrEmpty(this.RawDefine.Converter))
            {
                this.Remapper = AssemblyBase.GetDefTType(HostType.Namespace, this.RawDefine.Converter, this.IsNullable) as TEnum;
                if (this.Remapper == null)
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' converter:'{this.RawDefine.Converter}' not exists");
                }
            }
        }
    }
}
