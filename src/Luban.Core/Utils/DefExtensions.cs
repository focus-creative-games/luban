using Luban.CodeTarget;
using Luban.Defs;
using Luban.RawDefs;
using NLog.Targets;
using System.Text.RegularExpressions;

namespace Luban.Utils;

public static class DefExtensions
{
    public static bool NeedExport(this DefField field)
    {
        //return field.Assembly.NeedExport(field.Groups, GenerationContext.GlobalConf.Groups);
        var groupDefs = GenerationContext.GlobalConf.Groups;
        if (field.Groups.Count == 0)
        {
            return true;
        }
        var exportGroups = field.Assembly.Target.Groups;
        return field.Groups.Any(exportGroups.Contains);
    }

    public static List<DefField> GetExportFields(this DefBean bean)
    {
        return bean.Fields.Where(f => f.NeedExport()).ToList();
    }

    public static List<DefField> GetHierarchyExportFields(this DefBean bean)
    {
        return bean.HierarchyFields.Where(f => f.NeedExport()).ToList();
    }

    public static bool NeedExport(this DefTable table)
    {
        return table.Assembly.NeedExport(table.Groups, GenerationContext.GlobalConf.Groups);
    }


    public static string TypeNameWithTypeMapper(this DefTypeBase type)
    {
        if (type.TypeMappers != null)
        {
            string targetName = GenerationContext.Current.Target.Name;
            string codeTargetName = GenerationContext.CurrentCodeTarget.Name;
            foreach (var typeMapper in type.TypeMappers)
            {
                if (typeMapper.Targets.Contains(targetName) && typeMapper.CodeTargets.Contains(codeTargetName))
                {
                    return typeMapper.Options.TryGetValue(BuiltinOptionNames.TypeMapperType, out var typeName) ? typeName : throw new Exception($"option 'type' not found in type mapper of type {type.FullName} target:{targetName} codeTarget:{codeTargetName}");
                }
            }
        }
        return null;
    }

    public static string TypeConstructorWithTypeMapper(this DefTypeBase type)
    {
        if (type.TypeMappers != null)
        {
            string targetName = GenerationContext.Current.Target.Name;
            string codeTargetName = GenerationContext.CurrentCodeTarget.Name;
            foreach (var typeMapper in type.TypeMappers)
            {
                if (typeMapper.Targets.Contains(targetName) && typeMapper.CodeTargets.Contains(codeTargetName))
                {
                    return typeMapper.Options.TryGetValue(BuiltinOptionNames.TypeMapperConstructor, out var typeName) ? typeName : throw new Exception($"option 'constructor' not found in type mapper of type {type.FullName} target:{targetName} codeTarget:{codeTargetName}");
                }
            }
        }
        return null;
    }

    public static string GetTypeMapperOption(this DefTypeBase type, string option)
    {
        if (type.TypeMappers != null)
        {
            string targetName = GenerationContext.Current.Target.Name;
            string codeTargetName = GenerationContext.CurrentCodeTarget.Name;
            foreach (var typeMapper in type.TypeMappers)
            {
                if (typeMapper.Targets.Contains(targetName) && typeMapper.CodeTargets.Contains(codeTargetName))
                {
                    return typeMapper.Options.TryGetValue(option, out var typeName) ? typeName : throw new Exception($"option '{option}' not found in type mapper of type {type.FullName} target:{targetName} codeTarget:{codeTargetName}");
                }
            }
        }
        return null;
    }
}
