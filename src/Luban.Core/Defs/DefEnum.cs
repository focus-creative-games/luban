using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Defs;

public class DefEnum : DefTypeBase
{
    public class Item
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Alias { get; set; }

        public string AliasOrName => string.IsNullOrWhiteSpace(Alias) ? Name : Alias;

        public int IntValue { get; set; }

        public string Comment { get; set; }

        public string CommentOrAlias => string.IsNullOrEmpty(Comment) ? Alias : Comment;

        public Dictionary<string, string> Tags { get; set; }

        public bool HasTag(string attrName)
        {
            return Tags != null && Tags.ContainsKey(attrName);
        }

        public string GetTag(string attrName)
        {
            return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
        }
    }

    public bool IsFlags { get; }

    public bool IsUniqueItemId { get; }

    public List<Item> Items { get; } = new();

    private readonly Dictionary<string, int> _nameOrAlias2Value = new();

    private readonly Dictionary<int, string> _vaule2Name = new();

    public bool HasZeroValueItem => this.Items.Any(item => item.IntValue == 0);

    public bool TryValueByNameOrAlias(string name, out int value)
    {
        return _nameOrAlias2Value.TryGetValue(name, out value);
    }

    public int GetValueByNameOrAlias(string name)
    {
        // TODO flags ?
        if (!name.Contains('|'))
        {
            return GetBasicValueByNameOrAlias(name);
        }
        int combindValue = 0;
        foreach (var s in name.Split('|'))
        {
            combindValue |= GetBasicValueByNameOrAlias(s.Trim());
        }
        return combindValue;
    }

    private int GetBasicValueByNameOrAlias(string name)
    {
        if (_nameOrAlias2Value.TryGetValue(name, out var value))
        {
            return value;
        }
        else if (int.TryParse(name, out value))
        {
            if (!_vaule2Name.ContainsKey(value) && !IsFlags)
            {
                throw new Exception($"{value} 不是 enum:'{FullName}'的有效枚举值");
            }
            return value;
        }
        else
        {
            throw new Exception($"'{name}' 不是enum:'{FullName}'的有效枚举值");
        }
    }

    public DefEnum(RawEnum e)
    {
        Name = e.Name;
        Namespace = e.Namespace;
        IsFlags = e.IsFlags;
        IsUniqueItemId = e.IsUniqueItemId;
        Comment = e.Comment;
        Tags = e.Tags;
        Groups = e.Groups;
        TypeMappers = e.TypeMappers is { Count: > 0 } ? e.TypeMappers : null;
        foreach (var item in e.Items)
        {
            Items.Add(new Item
            {
                Name =
                    item.Name,
                Alias = item.Alias,
                Value = item.Value,
                Comment = string.IsNullOrWhiteSpace(item.Comment) ? item.Alias : item.Comment,
                Tags = item.Tags,
            });
        }
    }

    public override void Compile()
    {
        var fullName = FullName;

        int lastEnumValue = -1;
        var names = new HashSet<string>();
        foreach (var item in Items)
        {
            string value = item.Value.ToLower();
            if (!names.Add(item.Name))
            {
                throw new Exception($"enum:'{fullName}' 字段:'{item.Name}' 重复");
            }
            if (string.IsNullOrEmpty(value))
            {
                //  A,
                item.IntValue = ++lastEnumValue;
                item.Value = item.IntValue.ToString();
            }
            else if (int.TryParse(item.Value, out var v))
            {
                //  A = 5,
                item.IntValue = v;
                lastEnumValue = v;
            }
            else if (value.StartsWith("0x"))
            {

                if (int.TryParse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out var x))
                {
                    item.IntValue = x;
                    lastEnumValue = x;
                }
                else
                {
                    throw new Exception($"enum:'{fullName}' 枚举名:'{item.Name}' value:'{item.Value}' 非法");
                }
            }
            else if (IsFlags)
            {
                //  D = A | B | C,
                string[] itemNames = item.Value.Split('|').Select(s => s.Trim()).ToArray();
                foreach (var n in itemNames)
                {
                    var index = Items.FindIndex(i => i.Name == n);
                    if (index < 0)
                    {
                        throw new Exception($"enum:'{fullName}' 枚举名:'{item.Name}' 值:'{item.Value}' 非法");
                    }
                    item.IntValue |= Items[index].IntValue;
                }
            }
            else
            {
                throw new Exception($"enum:'{fullName}' 枚举名:'{item.Name}' value:'{item.Value}' 非法");
            }

            if (!string.IsNullOrWhiteSpace(item.Name) && !_nameOrAlias2Value.TryAdd(item.Name, item.IntValue))
            {
                throw new Exception($"enum:'{fullName}' 枚举名:'{Name}' 重复");
            }

            if (!string.IsNullOrWhiteSpace(item.Alias) && !_nameOrAlias2Value.TryAdd(item.Alias, item.IntValue))
            {
                throw new Exception($"enum:'{fullName}' 枚举名:'{Name}' alias:'{item.Alias}' 重复");
            }
            if (_vaule2Name.TryGetValue(item.IntValue, out var itemName))
            {
                if (IsUniqueItemId)
                {
                    throw new Exception($"enum:'{fullName}' 枚举值:{item.IntValue} 重复. 枚举名:'{itemName}' <=> '{item.Name}'");
                }
            }
            else
            {
                _vaule2Name.Add(item.IntValue, item.Name);
            }
        }


    }

}
