using Luban.RawDefs;
using Luban.Schema;
using Luban.Utils;

namespace Luban.Defs;

public abstract class DefTypeBase
{
    public DefAssembly Assembly { get; set; }

    public string Name { get; set; }

    public string Namespace { get; set; }

    public string FullName => TypeUtil.MakeFullName(Namespace, Name);

    public string NamespaceWithTopModule => TypeUtil.MakeNamespace(GenerationContext.Current.TopModule, Namespace);

    public string FullNameWithTopModule => TypeUtil.MakeNamespace(GenerationContext.Current.TopModule, FullName);

    public List<string> Groups { get; set; }

    public string Comment { get; protected set; }

    public Dictionary<string, string> Tags { get; protected set; }

    public List<TypeMapper> TypeMappers { get; protected set; }

    public bool HasTag(string attrName)
    {
        return Tags != null && Tags.ContainsKey(attrName);
    }

    public string GetTag(string attrName)
    {
        return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
    }

    public virtual void PreCompile()
    {
        if (Groups != null && Groups.Count > 0)
        {
            LubanConfig c = GenerationContext.GlobalConf;
            if (Groups.Contains("*"))
            {
                Groups.Clear();
                Groups.AddRange(c.Groups.SelectMany(g => g.Names));
            }
            else
            {
                foreach (var g in Groups)
                {
                    if (c.Groups.All(gg => !gg.Names.Contains(g)))
                    {
                        throw new Exception($"type:{FullName} group:{g} not found");
                    }
                }
            }
        }
    }

    public abstract void Compile();

    public virtual void PostCompile() { }
}
