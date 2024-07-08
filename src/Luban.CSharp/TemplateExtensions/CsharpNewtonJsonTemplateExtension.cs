﻿using Luban.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;
namespace Luban.CSharp.TemplateExtensions;

internal class CsharpNewtonJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
       return type.Apply(NewtonJsonDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    }

    public static string DeserializeField(string fieldName, string jsonVar, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{if ({jsonVar}.TryGetValue(\"{jsonFieldName}\", out var _j)) {{ {type.Apply(NewtonJsonDeserializeVisitor.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(NewtonJsonDeserializeVisitor.Ins, $"{jsonVar}.GetValue(\"{jsonFieldName}\")", fieldName, 0);
        }
    }
}
