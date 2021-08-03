using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Defs
{
    public class DefConst : DefTypeBase
    {
        public class Item
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }

            public TType CType { get; set; }

            public string Comment { get; set; }
        }

        public List<Item> Items { get; set; } = new List<Item>();

        public DefConst(Const c)
        {
            Namespace = c.Namespace;
            Name = c.Name;
            Comment = c.Comment;

            foreach (var item in c.Items)
            {
                Items.Add(new Item { Name = item.Name, Type = item.Type, Value = item.Value, Comment = item.Comment });
            }
        }

        public override void Compile()
        {
            var FullName = this.FullName;
            HashSet<string> names = new HashSet<string>();

            foreach (var item in Items)
            {
                if (item.Name.Length == 0)
                {
                    throw new Exception($"'{FullName}' 常量字段名不能为空");
                }
                if (!names.Add(item.Name))
                {
                    throw new Exception($"'{FullName}' 字段名:'{item.Name}' 重复");
                }
                if ((item.CType = AssemblyBase.CreateType(Namespace, item.Type)) == null)
                {
                    throw new Exception($"'{FullName}' type:'{item.Type}' 类型不存在");
                }
                if (!item.CType.TryParseFrom(item.Value))
                {
                    throw new Exception($"'{FullName}' value:'{item.Value}' 不是合法的 type:'{item.Type}' 类型值");
                }
            }
        }

    }
}
