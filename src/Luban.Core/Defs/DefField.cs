using Luban.RawDefs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.Defs;

public class DefField
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public DefAssembly Assembly => HostType.Assembly;

    public DefBean HostType { get; }

    public string Name { get; }

    public string Alias { get; }

    public string Type { get; }

    public TType CType { get; private set; }

    public bool IsNullable => CType.IsNullable;

    public string Comment { get; }

    public int AutoId { get; set; }

    // public string EscapeComment => DefUtil.EscapeCommentByCurrentLanguage(Comment);

    public Dictionary<string, string> Tags { get; }

    public List<string> Variants { get; }

    public string CurrentVariantNameWithoutFieldName { get; private set; }

    public string CurrentVariantNameWithFieldName { get; private set; }

    public string CurrentVariantNameWithFieldNameOrOrigin => CurrentVariantNameWithFieldName ?? Name;

    public bool IgnoreNameValidation { get; set; }

    public bool HasTag(string attrName)
    {
        return Tags != null && Tags.ContainsKey(attrName);
    }

    public string GetTag(string attrName)
    {
        return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
    }

    public List<string> Groups { get; }

    public RawField RawField { get; }


    public DefField(DefBean host, RawField f, int idOffset)
    {
        HostType = host;
        Name = f.Name;
        Alias = f.Alias;
        Type = f.Type;
        Comment = f.Comment;
        Tags = f.Tags;
        Variants = f.Variants;
        IgnoreNameValidation = f.NotNameValidation;
        this.Groups = f.Groups;
        this.RawField = f;
    }

    public override string ToString()
    {
        return $"{HostType.FullName}.{Name}";
    }

    public void Compile()
    {
        if (Variants != null && Variants.Count > 0)
        {
            string variantKey = $"{HostType.FullName}.{Name}";
            if (HostType.Assembly.TryGetVariantNameOrDefault(variantKey, out var variantName))
            {
                if (!Variants.Contains(variantName))
                {
                    throw new Exception($"type:'{HostType.FullName}' field:'{Name}' variantKey:'{variantKey}' exists, but variantName'{variantName}' not in {string.Join(",", Variants)}");
                }
                CurrentVariantNameWithoutFieldName = variantName;
                CurrentVariantNameWithFieldName = $"{Name}@{variantName}";
            }
            else
            {
                s_logger.Warn($"type:'{HostType.FullName}' field:'{Name}' not set variant. please set variant by command line option '--variant {variantKey}=<variantName>'");
            }
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

        ValidatorManager.Ins.InitValidatorsRecursive(CType);
    }

    public void PostCompile()
    {
        CType.PostCompile(this);
        ValidatorManager.Ins.CompileValidatorsRecursive(CType, this);
    }

    public static void CompileFields<T>(DefTypeBase hostType, List<T> fields) where T : DefField
    {
        var names = new HashSet<string>();
        int nextAutoId = 1;
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

        var aliasNames = new Dictionary<string, DefField>();
        foreach (var f in fields)
        {
            if (!string.IsNullOrEmpty(f.Alias))
            {
                if (!aliasNames.TryAdd(f.Alias, f))
                {
                    throw new Exception($"type:'{hostType.FullName}' field:'{f.Name}' alias:'{f.Alias}' duplicate with field:{aliasNames[f.Alias].Name}");
                }
                if (names.Contains(f.Alias))
                {
                    throw new Exception($"type:'{hostType.FullName}' field:'{f.Name}' alias:'{f.Alias}' duplicate with other field name");
                }
            }
            f.Compile();
        }
    }
}
