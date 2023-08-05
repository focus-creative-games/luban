using Luban.RawDefs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.Defs;

public class DefField
{
    public DefAssembly Assembly => HostType.Assembly;

    public DefBean HostType { get; set; }

    public string Name { get; protected set; }

    public string Type { get; }

    public TType CType { get; protected set; }

    public bool IsNullable => CType.IsNullable;

    public string Comment { get; }
    
    public int AutoId { get; set; }

    // public string EscapeComment => DefUtil.EscapeCommentByCurrentLanguage(Comment);

    public Dictionary<string, string> Tags { get; }

    public bool IgnoreNameValidation { get; set; }

    public bool HasTag(string attrName)
    {
        return Tags != null && Tags.ContainsKey(attrName);
    }

    public string GetTag(string attrName)
    {
        return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
    }
    
    public string Index { get; private set; }

    public List<string> Groups { get; }

    public DefField IndexField { get; private set; }

    public RawField RawField { get; }


    public DefField(DefBean host, RawField f, int idOffset)
    {
        HostType = host;
        Name = f.Name;
        Type = f.Type;
        Comment = f.Comment;
        Tags = DefUtil.ParseAttrs(f.Tags);
        IgnoreNameValidation = f.IgnoreNameValidation;
        this.Groups = f.Groups;
        this.RawField = f;
    }

    public void Compile()
    {
        if (!IgnoreNameValidation && !TypeUtil.IsValidName(Name))
        {
            throw new Exception($"type:'{HostType.FullName}' field name:'{Name}' is reserved");
        }

        try
        {
            CType = Assembly.CreateType(HostType.Namespace, Type, false);
        }
        catch (Exception e)
        {
            throw new Exception($"type:'{HostType.FullName}' field:'{Name}' type:'{Type}' is invalid", e);
        }

        //if (IsNullable && (CType.IsCollection || (CType is TBean)))
        //{
        //    throw new Exception($"type:{HostType.FullName} field:{Name} type:{Type} is collection or bean. not support nullable");
        //}

        switch (CType)
        {
            case TArray t:
            {
                if (t.ElementType is TBean e && !e.IsDynamic && e.DefBean.HierarchyFields.Count == 0)
                {
                    throw new Exception($"container element type:'{e.DefBean.FullName}' can't be empty bean");
                }
                break;
            }
            case TList t:
            {
                if (t.ElementType is TBean e && !e.IsDynamic && e.DefBean.HierarchyFields.Count == 0)
                {
                    throw new Exception($"container element type:'{e.DefBean.FullName}' can't be empty bean");
                }
                break;
            }
        }

        foreach (var (tagName, tagValue) in CType.Tags)
        {
            if (ValidatorManager.Ins.TryCreateDataValidator(tagName, tagValue, out var validator))
            {
                CType.Validators.Add(validator);
            }
        }
    }

    private void ValidateIndex()
    {
        Index = CType.GetTag("index");

        if (!string.IsNullOrEmpty(Index))
        {
            if ((CType is TArray tarray) && (tarray.ElementType is TBean b))
            {
                if ((IndexField = b.DefBean.GetField(Index)) == null)
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' index:'{Index}'. index not exist");
                }
            }
            else if ((CType is TList tlist) && (tlist.ElementType is TBean tb))
            {
                if ((IndexField = tb.DefBean.GetField(Index)) == null)
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

    public void PostCompile()
    {
        CType.PostCompile(this);
        ValidateIndex();
    }

    public static void CompileFields<T>(DefTypeBase hostType, List<T> fields) where T : DefField
    {
        var names = new HashSet<string>();
        int nextAutoId = 0;
        foreach (var f in fields)
        {
            var name = f.Name;
            if (name.Length == 0)
            {
                throw new Exception($"type:'{hostType.FullName}' field name can't be empty");
            }
            if (!names.Add(name))
            {
                throw new Exception($"type:'{hostType.FullName}' 'field:{name}' duplicate");
            }
            if (TypeUtil.ToCsStyleName(name) == hostType.Name)
            {
                throw new Exception($"type:'{hostType.FullName}' field:'{name}' 生成的c#字段名与类型名相同，会引起编译错误");
            }
            f.AutoId = nextAutoId++;
        }

        foreach (var f in fields)
        {
            f.Compile();
        }
    }
}