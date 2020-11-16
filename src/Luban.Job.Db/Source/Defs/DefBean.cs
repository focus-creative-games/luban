using Luban.Job.Common.RawDefs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Db.Defs
{
    class DefBean : DbDefTypeBase
    {
        public const string TYPE_NAME_KEY = "__type__";

        public bool IsBean => true;

        public string Parent { get; }

        public bool IsValueType { get; }

        public bool IsMultiRow { get; set; }

        public DefBean ParentDefType { get; protected set; }

        public DefBean RootDefType => this.ParentDefType == null ? this : this.ParentDefType.RootDefType;

        public bool IsSerializeCompatible { get; }

        public List<DefBean> Children { get; private set; }

        public List<DefBean> HierarchyNotAbstractChildren { get; private set; }

        public bool IsNotAbstractType => Children == null;

        public bool IsAbstractType => Children != null;

        public List<DefField> HierarchyFields { get; private set; } = new List<DefField>();

        public List<DefField> Fields { get; } = new List<DefField>();

        public string CsClassModifier => IsAbstractType ? "abstract" : "sealed";

        public string CsMethodModifier => ParentDefType != null ? "override" : (IsAbstractType ? "virtual" : "");

        public DefBean(Bean b)
        {
            Name = b.Name;
            Namespace = b.Namespace;
            Parent = b.Parent;
            Id = b.TypeId;
            IsValueType = b.IsValueType;
            foreach (var field in b.Fields)
            {
                Fields.Add(new DefField(this, field, 0));
            }
        }

        private void CollectHierarchyNotAbstractChildren(List<DefBean> children)
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

        internal DefBean GetNotAbstractChildType(string typeNameOrAliasName)
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

        internal DefField GetField(string index)
        {
            return HierarchyFields.Where(f => f.Name == index).FirstOrDefault();
        }

        internal bool TryGetField(string index, out DefField field, out int fieldIndexId)
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

        private void CollectHierarchyFields(List<DefField> fields)
        {
            if (ParentDefType != null)
            {
                ParentDefType.CollectHierarchyFields(fields);
            }
            fields.AddRange(Fields);
        }

        public override void PreCompile()
        {
            var agent = AssemblyBase.Agent;
            agent.Trace("compile bean  module:{0} name:{1} begin", Namespace, Name);
            if (!string.IsNullOrEmpty(Parent))
            {
                if ((ParentDefType = (DefBean)Assembly.GetDefType(Namespace, Parent)) == null)
                {
                    throw new Exception($"bean:{FullName} parent:{Parent} not exist");
                }
                if (ParentDefType.Children == null)
                {
                    ParentDefType.Children = new List<DefBean>();
                }
                ParentDefType.Children.Add(this);
            }

            CollectHierarchyFields(HierarchyFields);
        }

        public override void Compile()
        {
            var agent = AssemblyBase.Agent;
            var cs = new List<DefBean>();
            if (Children != null)
            {
                CollectHierarchyNotAbstractChildren(cs);
            }
            HierarchyNotAbstractChildren = cs;


            var ids = new HashSet<int>();
            foreach (var c in cs)
            {
                if (c.Id == 0)
                {
                    throw new Exception($"bean:{FullName} is child of dynamic type. beanid:{Id} can't be 0!");
                }
                if (!ids.Add(c.Id))
                {
                    throw new Exception($"bean:{c.FullName} beanid:{c.Id} duplicate!");
                }
            }

            DefField.CompileFields(this, HierarchyFields, true);
            if (IsValueType && HierarchyFields.Any(f => f.CType.NeedSetChildrenRoot()))
            {
                foreach (var f in Fields)
                {
                    if (f.CType.NeedSetChildrenRoot())
                    {
                        throw new Exception($"bean:{FullName} value type field:{f.Name} must be primitive type");
                    }
                }

            }
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
