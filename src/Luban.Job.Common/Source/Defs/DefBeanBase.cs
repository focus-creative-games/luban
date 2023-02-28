using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Common.Defs
{
    public abstract class DefBeanBase : DefTypeBase
    {
        public bool IsBean => true;

        public string Parent { get; }

        public bool IsValueType { get; }

        public DefBeanBase ParentDefType { get; protected set; }

        public DefBeanBase RootDefType => this.ParentDefType == null ? this : this.ParentDefType.RootDefType;

        public bool IsSerializeCompatible { get; }

        public List<DefBeanBase> Children { get; set; }

        public List<DefBeanBase> HierarchyNotAbstractChildren { get; set; }

        public IEnumerable<DefBeanBase> GetHierarchyChildren()
        {
            yield return this;
            if (Children == null)
            {
                yield break;
            }
            foreach (var child in Children)
            {
                foreach(var c2 in  child.GetHierarchyChildren())
                {
                    yield return c2;
                }
            }
        }

        public bool IsNotAbstractType => Children == null;

        public bool IsAbstractType => Children != null;

        public List<DefFieldBase> HierarchyFields { get; private set; } = new List<DefFieldBase>();

        public List<DefFieldBase> Fields { get; } = new List<DefFieldBase>();

        public string CsClassModifier => IsAbstractType ? "abstract" : "sealed";

        public string CsMethodModifier => ParentDefType != null ? "override" : (IsAbstractType ? "virtual" : "");


        public string JavaClassModifier => IsAbstractType ? "abstract" : "final";

        public string JavaMethodModifier => ParentDefType != null ? "override" : (IsAbstractType ? "virtual" : "");

        public string TsClassModifier => IsAbstractType ? "abstract" : "";

        public DefBeanBase(Bean b)
        {
            Name = b.Name;
            Namespace = b.Namespace;
            Parent = b.Parent;
            Id = b.TypeId;
            IsValueType = b.IsValueType;
            Comment = b.Comment;
            Tags = DefUtil.ParseAttrs(b.Tags);
            foreach (var field in b.Fields)
            {
                Fields.Add(CreateField(field, 0));
            }
        }

        public virtual string GoBinImport
        {
            get
            {
                var imports = new HashSet<string>();
                if (IsAbstractType || this.HierarchyFields.Count > 0)
                {
                    imports.Add("errors");
                }
                foreach (var f in HierarchyFields)
                {
                    f.CType.Apply(Luban.Job.Common.TypeVisitors.GoBinImport.Ins, imports);
                }
                return string.Join('\n', imports.Select(im => $"import \"{im}\""));
            }
        }

        protected abstract DefFieldBase CreateField(Field f, int idOffset);

        public void CollectHierarchyNotAbstractChildren(List<DefBeanBase> children)
        {
            if (IsAbstractType)
            {
                foreach (var c in Children)
                {
                    c.CollectHierarchyNotAbstractChildren(children);
                }
            }
            else
            {
                children.Add(this);
            }
        }

        public virtual DefBeanBase GetNotAbstractChildType(string typeNameOrAliasName)
        {
            if (string.IsNullOrWhiteSpace(typeNameOrAliasName))
            {
                return null;
            }
            foreach (var c in HierarchyNotAbstractChildren)
            {
                if (c.Name == typeNameOrAliasName)
                {
                    return c;
                }
            }
            return null;
        }

        public DefFieldBase GetField(string index)
        {
            return HierarchyFields.Where(f => f.Name == index).FirstOrDefault();
        }

        public bool TryGetField(string index, out DefFieldBase field, out int fieldIndexId)
        {
            for (int i = 0; i < HierarchyFields.Count; i++)
            {
                if (HierarchyFields[i].Name == index)
                {
                    field = HierarchyFields[i];
                    fieldIndexId = i;
                    return true;
                }
            }
            field = null;
            fieldIndexId = 0;
            return false;
        }

        protected void CollectHierarchyFields(List<DefFieldBase> fields)
        {
            if (ParentDefType != null)
            {
                ParentDefType.CollectHierarchyFields(fields);
            }
            fields.AddRange(Fields);
        }

        private void SetUpParentRecursively()
        {
            if (ParentDefType == null && !string.IsNullOrEmpty(Parent))
            {
                if ((ParentDefType = (DefBeanBase)AssemblyBase.GetDefType(Namespace, Parent)) == null)
                {
                    throw new Exception($"bean:'{FullName}' parent:'{Parent}' not exist");
                }
                if (ParentDefType.Children == null)
                {
                    ParentDefType.Children = new List<DefBeanBase>();
                }
                ParentDefType.Children.Add(this);
                ParentDefType.SetUpParentRecursively();
            }
        }

        public override void PreCompile()
        {
            SetUpParentRecursively();
            CollectHierarchyFields(HierarchyFields);
        }

        public override void Compile()
        {
            var cs = new List<DefBeanBase>();
            if (Children != null)
            {
                CollectHierarchyNotAbstractChildren(cs);
            }
            HierarchyNotAbstractChildren = cs;


            var ids = new HashSet<int>();
            foreach (var c in cs)
            {
                if (c.Id <= 0)
                {
                    throw new Exception($"bean:'{FullName}' is child of dynamic type. beanid:{Id} can't less then 0!");
                }
                if (!ids.Add(c.Id))
                {
                    throw new Exception($"bean:'{c.FullName}' beanid:{c.Id} duplicate!");
                }
            }
            DefFieldBase.CompileFields(this, HierarchyFields, IsSerializeCompatible);
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
