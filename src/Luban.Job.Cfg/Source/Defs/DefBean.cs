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
        public const string TYPE_NAME_KEY = "__type__";

        public const string BEAN_NULL_STR = "{null}";

        public const string BEAN_NOT_NULL_STR = "{}";

        public string Alias { get; }

        public bool IsMultiRow { get; set; }

        public String Sep { get; }

        public List<DefField> HierarchyExportFields { get; private set; }

        public List<DefField> ExportFields { get; private set; }

        public bool IsDefineEquals(DefBean b)
        {
            return DeepCompareTypeDefine.Ins.Compare(this, b, new Dictionary<DefTypeBase, bool>(), new HashSet<DefTypeBase>());
        }

        public string GoImport
        {
            get
            {
                var imports = new HashSet<string>();
                foreach (var f in Fields)
                {
                    f.CType.Apply(CollectGoImport.Ins, imports);
                }
                return string.Join('\n', imports.Select(im => $"import \"{im}\""));
            }
        }

        public string UeBpIncludes
        {
            get
            {
                //var imports = new HashSet<CfgDefTypeBase>();
                //if (ParentDefType != null)
                //{
                //    imports.Add(ParentDefType);
                //}
                //foreach (var f in Fields)
                //{
                //    f.CType.Apply(CollectEditorCppHeaderIncludeVisitor.Ins, imports);
                //}
                //imports.Remove(this);
                //return string.Join('\n', imports.Select(im => $"#include \"{im.UeBpHeaderFileName}\""));
                throw new NotImplementedException();
            }
        }

        //public string UeForwards
        //{
        //    get
        //    {
        //        var imports = new HashSet<IDefType>();
        //        foreach (var f in Fields)
        //        {
        //            f.CType.Apply(CollectUeCppForwardDefineVisitor.Ins, imports);
        //        }
        //        return string.Join('\n', imports.Select(im => $"{im.CppNamespaceBegin} class {im.UeBpName}; {im.CppNamespaceEnd}"));
        //    }
        //}

        public string EditorCppIncludes
        {
            get
            {
                //var imports = new HashSet<CfgDefTypeBase>();
                //if (ParentDefType != null)
                //{
                //    imports.Add(ParentDefType);
                //}
                //foreach (var f in Fields)
                //{
                //    f.CType.Apply(CollectEditorCppHeaderIncludeVisitor.Ins, imports);
                //}
                //imports.Remove(this);
                //return string.Join('\n', imports.Select(im => $"#include \"{im.UeEditorHeaderFileName}\""));
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
                //return string.Join('\n', imports.Select(im => $"{im.CppNamespaceBegin} struct {im.UeFname}; {im.CppNamespaceEnd}"));
                throw new NotImplementedException();
            }
        }

        public DefBean(CfgBean b) : base(b)
        {
            Alias = b.Alias;
            Id = b.TypeId;
            Sep = b.Sep;
        }

        protected override DefFieldBase CreateField(Common.RawDefs.Field f, int idOffset)
        {
            return new DefField(this, (CfgField)f, idOffset);
        }

        internal DefField GetField(string index)
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
            if (!string.IsNullOrEmpty(Parent))
            {
                if ((ParentDefType = (DefBean)AssemblyBase.GetDefType(Namespace, Parent)) == null)
                {
                    throw new Exception($"bean:{FullName} parent:{Parent} not exist");
                }
                if (ParentDefType.Children == null)
                {
                    ParentDefType.Children = new List<DefBeanBase>();
                }
                ParentDefType.Children.Add(this);
            }

            CollectHierarchyFields(HierarchyFields);

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
                throw new Exception($"bean:{FullName} beanid:{Id} should be 0!");
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
                    throw new Exception($"bean:{FullName} alias:{c.Alias} 重复");
                }
            }
            DefField.CompileFields(this, HierarchyFields, false);
        }

        public override void PostCompile()
        {
            foreach (var field in HierarchyFields)
            {
                field.PostCompile();
            }
        }
    }
}
