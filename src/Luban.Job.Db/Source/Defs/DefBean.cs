using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Db.Defs
{
    class DefBean : DefBeanBase
    {
        public DefBean(Bean b) : base(b)
        {

        }

        protected override DefFieldBase CreateField(Field f, int idOffset)
        {
            return new DefField(this, f, idOffset);
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
                if (c.Id == 0)
                {
                    throw new Exception($"bean:'{FullName}' is child of dynamic type. beanid:{Id} can't be 0!");
                }
                if (!ids.Add(c.Id))
                {
                    throw new Exception($"bean:'{c.FullName}' beanid:{c.Id} duplicate!");
                }
            }

            DefField.CompileFields(this, HierarchyFields, true);
            //if (IsValueType && HierarchyFields.Any(f => f.CType.Apply(NeedSetChildrenRootVisitor.Ins)))
            //{
            //    foreach (var f in Fields)
            //    {
            //        if (f.CType.Apply(NeedSetChildrenRootVisitor.Ins))
            //        {
            //            throw new Exception($"bean:{FullName} value type field:{f.Name} must be primitive type");
            //        }
            //    }

            //}
        }
    }
}
