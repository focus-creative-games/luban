// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.RawDefs;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Defs;

public class DefBean : DefTypeBase
{
    public int Id { get; }

    public int AutoId { get; private set; } // for protobuf

    public string Parent { get; }

    public DefBean ParentDefType { get; private set; }

    public DefBean RootDefType => this.ParentDefType == null ? this : this.ParentDefType.RootDefType;

    public List<DefBean> Children { get; set; }

    public List<DefBean> HierarchyNotAbstractChildren { get; set; }

    public IEnumerable<DefBean> GetHierarchyChildren()
    {
        yield return this;
        if (Children == null)
        {
            yield break;
        }
        foreach (var child in Children)
        {
            foreach (var c2 in child.GetHierarchyChildren())
            {
                yield return c2;
            }
        }
    }

    public bool IsAbstractType => Children != null;

    public List<DefField> HierarchyFields { get; private set; } = new();

    public List<DefField> Fields { get; } = new();

    public string Alias { get; }

    public bool IsMultiRow { get; set; }

    public string Sep { get; }

    public bool IsValueType { get; }


    private List<DefField> _hierarchyExportFields;

    public List<DefField> HierarchyExportFields => _hierarchyExportFields ??= HierarchyFields.Where(f => f.NeedExport()).ToList();

    private List<DefField> _exportFields;

    public List<DefField> ExportFields => _exportFields ??= Fields.Where(f => f.NeedExport()).ToList();

    public bool IsAssignableFrom(DefBean b)
    {
        while (b != null)
        {
            if (b == this)
            {
                return true;
            }
            b = b.ParentDefType;
        }
        return false;
    }

    public bool IsDefineEquals(DefBean b)
    {
        return DeepCompareTypeDefine.Ins.Compare(this, b, new Dictionary<DefTypeBase, bool>(), new HashSet<DefTypeBase>());
    }

    public DefBean(RawBean b)
    {
        Name = b.Name;
        Namespace = b.Namespace;
        Parent = b.Parent;
        Id = TypeUtil.ComputeCfgHashIdByName(FullName);
        Comment = b.Comment;
        Tags = b.Tags;
        foreach (var field in b.Fields)
        {
            Fields.Add(CreateField(field, 0));
        }
        Alias = b.Alias;
        Sep = b.Sep;
        IsValueType = b.IsValueType;
        Groups = b.Groups;
        TypeMappers = b.TypeMappers is { Count: > 0 } ? b.TypeMappers : null;
    }

    protected DefField CreateField(RawField f, int idOffset)
    {
        return new DefField(this, f, idOffset);
    }

    public DefField GetField(string index)
    {
        return HierarchyFields.FirstOrDefault(f => f.Name == index);
    }

    public bool TryGetField(string index, out DefField field, out int fieldIndexId)
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

    public DefBean GetNotAbstractChildType(string typeNameOrAliasName)
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
        SetupParentRecursively();
        CollectHierarchyFields(HierarchyFields);
    }

    public override void Compile()
    {
        var cs = new List<DefBean>();
        if (Children != null)
        {
            CollectHierarchyNotAbstractChildren(cs);
        }
        HierarchyNotAbstractChildren = cs;
        // 检查别名是否重复
        HashSet<string> nameOrAliasName = cs.Select(b => b.Name).ToHashSet();
        foreach (DefBean c in cs)
        {
            if (!string.IsNullOrWhiteSpace(c.Alias) && !nameOrAliasName.Add(c.Alias))
            {
                throw new Exception($"bean:'{FullName}' alias:{c.Alias} 重复");
            }
        }
        DefField.CompileFields(this, HierarchyFields);
    }

    public override void PostCompile()
    {
        foreach (var field in HierarchyFields)
        {
            field.PostCompile();
        }

        if (IsAbstractType && ParentDefType == null)
        {
            int autoId = 1;
            foreach (DefBean child in HierarchyNotAbstractChildren)
            {
                child.AutoId = autoId++;
            }
        }
    }

    public void CollectHierarchyNotAbstractChildren(List<DefBean> children)
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

    protected void CollectHierarchyFields(List<DefField> fields)
    {
        if (ParentDefType != null)
        {
            ParentDefType.CollectHierarchyFields(fields);
        }
        fields.AddRange(Fields);
    }

    private void SetupParentRecursively()
    {
        if (ParentDefType == null && !string.IsNullOrEmpty(Parent))
        {
            if ((ParentDefType = (DefBean)Assembly.GetDefType(Namespace, Parent)) == null)
            {
                throw new Exception($"bean:'{FullName}' parent:'{Parent}' not exist");
            }
            if (ParentDefType.Children == null)
            {
                ParentDefType.Children = new List<DefBean>();
            }
            ParentDefType.Children.Add(this);
            ParentDefType.SetupParentRecursively();
        }
    }
}
