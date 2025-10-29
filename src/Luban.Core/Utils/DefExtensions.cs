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
