using Luban.Common.Utils;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Defs
{
    public class DefBean : DefBeanBase
    {
        public const string FALLBACK_TYPE_NAME_KEY = "__type__";

        public const string BEAN_NULL_STR = "null";

        public const string BEAN_NOT_NULL_STR = "{}";

        public const string JSON_TYPE_NAME_KEY = "$type";

        public const string XML_TYPE_NAME_KEY = "type";

        public const string LUA_TYPE_NAME_KEY = "_type_";

        public const string EXCEL_TYPE_NAME_KEY = "$type";
        public const string EXCEL_VALUE_NAME_KEY = "$value";

        public string JsonTypeNameKey => JSON_TYPE_NAME_KEY;

        public string LuaTypeNameKey => LUA_TYPE_NAME_KEY;

        public string Alias { get; }

        public bool IsMultiRow { get; set; }

        public string Sep { get; }

        public List<DefField> HierarchyExportFields { get; private set; }

        public List<DefField> ExportFields { get; private set; }

        public int AutoId { get; set; }

        public bool IsDefineEquals(DefBean b)
        {
            return DeepCompareTypeDefine.Ins.Compare(this, b, new Dictionary<DefTypeBase, bool>(), new HashSet<DefTypeBase>());
        }

        public override string GoBinImport
        {
            get
            {
                var imports = new HashSet<string>();
                if (IsAbstractType || this.HierarchyExportFields.Count > 0)
                {
                    imports.Add("errors");
                }
                foreach (var f in HierarchyExportFields)
                {
                    f.CType.Apply(Luban.Job.Common.TypeVisitors.GoBinImport.Ins, imports);
                }
                return string.Join('\n', imports.Select(im => $"import \"{im}\""));
            }
        }

        public string GoJsonImport
        {
            get
            {
                var imports = new HashSet<string>();
                if (IsAbstractType || this.HierarchyExportFields.Count > 0)
                {
                    imports.Add("errors");
                }
                foreach (var f in HierarchyExportFields)
                {
                    f.CType.Apply(TypeVisitors.GoJsonImport.Ins, imports);
                }
                return string.Join('\n', imports.Select(im => $"import \"{im}\""));
            }
        }

        public string UeBpIncludes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string EditorCppIncludes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string EditorCppForwards
        {
            get
            {
                var imports = new HashSet<DefTypeBase>();
                foreach (var f in Fields)
                {
                    f.CType.Apply(CollectEditorCppForwardDefineVisitor.Ins, imports);
                }
                throw new NotImplementedException();
            }
        }

        public DefBean(CfgBean b) : base(b)
        {
            Alias = b.Alias;
            Id = b.TypeId;
            Sep = b.Sep;
        }

        override protected DefFieldBase CreateField(Common.RawDefs.Field f, int idOffset)
        {
            return new DefField(this, (CfgField)f, idOffset);
        }

        public new DefField GetField(string index)
        {
            return (DefField)HierarchyFields.Where(f => f.Name == index).FirstOrDefault();
        }

        internal bool TryGetField(string index, out DefField field, out int fieldIndexId)
        {
            for (int i = 0; i < HierarchyFields.Count; i++)
            {
                if (HierarchyFields[i].Name == index)
                {
                    field = (DefField)HierarchyFields[i];
                    fieldIndexId = i;
                    return true;
                }
            }
            field = null;
            fieldIndexId = 0;
            return false;
        }

        public override DefBeanBase GetNotAbstractChildType(string typeNameOrAliasName)
        {
            if (string.IsNullOrWhiteSpace(typeNameOrAliasName))
            {
                return null;
            }
            foreach (DefBean c in HierarchyNotAbstractChildren)
            {
                if (c.Name == typeNameOrAliasName || c.Alias == typeNameOrAliasName)
                {
                    return c;
                }
            }
            return null;
        }

        public override void PreCompile()
        {
            base.PreCompile();
            this.ExportFields = this.Fields.Select(f => (DefField)f).Where(f => f.NeedExport).ToList();
            this.HierarchyExportFields = this.HierarchyFields.Select(f => (DefField)f).Where(f => f.NeedExport).ToList();
        }

        public override void Compile()
        {
            var cs = new List<DefBeanBase>();
            if (Children != null)
            {
                CollectHierarchyNotAbstractChildren(cs);
            }
            HierarchyNotAbstractChildren = cs;
            if (Id != 0)
            {
                throw new Exception($"bean:'{FullName}' beanid:{Id} should be 0!");
            }
            else
            {
                Id = TypeUtil.ComputCfgHashIdByName(FullName);
            }
            // 检查别名是否重复
            HashSet<string> nameOrAliasName = cs.Select(b => b.Name).ToHashSet();
            foreach (DefBean c in cs)
            {
                if (!string.IsNullOrWhiteSpace(c.Alias) && !nameOrAliasName.Add(c.Alias))
                {
                    throw new Exception($"bean:'{FullName}' alias:{c.Alias} 重复");
                }
            }
            DefField.CompileFields(this, HierarchyFields, false);

            var allocAutoIds = this.HierarchyFields.Select(f => f.Id).ToHashSet();

            int nextAutoId = 1;
            foreach (var f in this.HierarchyFields)
            {
                while (!allocAutoIds.Add(nextAutoId))
                {
                    ++nextAutoId;
                }
                f.AutoId = nextAutoId;
            }
        }

        public override void PostCompile()
        {
            foreach (var field in HierarchyFields)
            {
                field.PostCompile();
            }
            if (this.IsAbstractType && this.ParentDefType == null)
            {
                int nextAutoId = 0;
                foreach (DefBean c in this.HierarchyNotAbstractChildren)
                {
                    c.AutoId = ++nextAutoId;
                }
            }
        }
    }
}
