using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;
class UnityEditorRender : ITypeFuncVisitor<string, int, string>
{
    public static UnityEditorRender Ins { get; } = new UnityEditorRender();
    public string Accept(TBool type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = UnityEditor.EditorGUILayout.Toggle({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TByte type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = (byte)UnityEditor.EditorGUILayout.IntField({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TShort type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = (short)UnityEditor.EditorGUILayout.IntField({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TInt type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = UnityEditor.EditorGUILayout.IntField({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TLong type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = UnityEditor.EditorGUILayout.LongField({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TFloat type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = UnityEditor.EditorGUILayout.FloatField({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TDouble type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = UnityEditor.EditorGUILayout.DoubleField({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TEnum type, string fieldName, int depth)
    {
        return $$"""
        {{fieldName}} = ({{type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}})UnityEditor.EditorGUILayout.EnumPopup({{fieldName}}, GUILayout.Width(150));
        """;
    }

    public string Accept(TString type, string fieldName, int depth)
    {
        if (type.HasTag("obj"))
        {
            var objType = type.GetTag("obj");
            if (objType == "sprite")
            {
                return $$"""
                {{fieldName}}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({{fieldName}}_UnityObject, typeof(UnityEngine.Sprite), false, GUILayout.Width(150)) as UnityEngine.Sprite;if ({{fieldName}}_UnityObject != null)
                {
                    UnityEngine.GUILayout.Label(((UnityEngine.Sprite){{fieldName}}_UnityObject).texture);
                }
                """;
            }
            else if (objType == "texture")
            {
                return $$"""
                {{fieldName}}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({{fieldName}}_UnityObject, typeof(UnityEngine.Texture2D), false, GUILayout.Width(150)) as UnityEngine.Texture2D;if ({{fieldName}}_UnityObject != null)
                {
                    UnityEngine.GUILayout.Label((UnityEngine.Texture2D){{fieldName}}_UnityObject);
                }
                """;
            }
            else if (objType == "audioClip")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.AudioClip), false, GUILayout.Width(150)) as UnityEngine.AudioClip;";
            }
            else if (objType == "animationClip")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.AnimationClip), false, GUILayout.Width(150)) as UnityEngine.AnimationClip;";
            }
            else if (objType == "material")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.Material), false, GUILayout.Width(150)) as UnityEngine.Material;";
            }
            else if (objType == "gameObject")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.GameObject), false, GUILayout.Width(150)) as UnityEngine.GameObject;";
            }
            else if (objType == "prefab")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.GameObject), false, GUILayout.Width(150)) as UnityEngine.GameObject;";
            }
            else if (objType == "font")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.Font), false, GUILayout.Width(150)) as UnityEngine.Font;";
            }
            else if (objType == "textAsset")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.TextAsset), false, GUILayout.Width(150)) as UnityEngine.TextAsset;";
            }
            else if (objType == "shader")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.Shader), false, GUILayout.Width(150)) as UnityEngine.Shader;";
            }
            else if (objType == "scriptableObject")
            {
                return $"{fieldName}_UnityObject = UnityEditor.EditorGUILayout.ObjectField({fieldName}_UnityObject, typeof(UnityEngine.ScriptableObject), false, GUILayout.Width(150)) as UnityEngine.ScriptableObject;";
            }
        }

        return $"{fieldName} = UnityEditor.EditorGUILayout.TextField({fieldName}, GUILayout.Width(150));";
    }

    public string Accept(TDateTime type, string fieldName, int depth)
    {
        return $"{fieldName} = UnityEditor.EditorGUILayout.LongField({fieldName}, GUILayout.Width(150));";
    }

    public string Accept(TBean type, string fieldName, int depth)
    {
        if (type.DefBean.IsAbstractType)
        {
            var __list = $"__list{depth}";
            var __newIndex = $"__newIndex{depth}";
            var __type = $"__type{depth}";
            var __impl = $"__impl{depth}";

            return $$"""
            {
                var {{__list}} = {{type.DefBean.FullName}}.Types.Select(t => new GUIContent(t)).ToArray();
                UnityEditor.EditorGUILayout.BeginVertical("Box");
                if ({{fieldName}} == null)
                {
                    {{fieldName}} = new {{type.DefBean.HierarchyNotAbstractChildren[0].FullName}}();
                    {{fieldName}}.TypeIndex = 0;
                }
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.LabelField("类型", GUILayout.Width(100));
                {{fieldName}}.TypeIndex = UnityEditor.EditorGUILayout.Popup({{fieldName}}.TypeIndex, {{__list}}, GUILayout.Width(200));
                UnityEditor.EditorGUILayout.EndHorizontal();
                {{fieldName}}.Instance.Render();
                UnityEditor.EditorGUILayout.EndVertical();
            }
            """;
        }
        else
        {
            var sb = $$"""
            {
                UnityEditor.EditorGUILayout.BeginVertical("Box");
            """;
            foreach (var f in type.DefBean.HierarchyExportFields)
            {
                sb += $$"""
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.LabelField("{{f.Name}}");
                {{f.CType.Apply(this, $"{fieldName}.{f.Name}", depth + 1)}}
                UnityEditor.EditorGUILayout.EndHorizontal();
                """;
            }
            sb += $$"""
                UnityEditor.EditorGUILayout.EndVertical();
            }
            """;
            return sb;
        }
    }

    public string Accept(TArray type, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __i = $"__i{depth}";
        var __e = $"__e{depth}";
        var __list = $"__list{depth}";

        return $$"""
        {
            UnityEditor.EditorGUILayout.BeginVertical("Box");
            int {{__n}} = {{fieldName}}.Length;
            for (int {{__i}} = 0; {{__i}} < {{__n}}; {{__i}}++)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    var {{__list}} = new System.Collections.Generic.List<{{type.ElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}}>({{fieldName}});
                    {{__list}}.RemoveAt({{__i}});
                    {{fieldName}} = {{__list}}.ToArray();
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    break;
                }
                UnityEditor.EditorGUILayout.LabelField({{__i}}.ToString(), GUILayout.Width(20));
                {{type.ElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}} {{__e}} = {{fieldName}}[{{__i}}];
                {{type.ElementType.Apply(this, __e, depth + 1)}};
                {{fieldName}}[{{__i}}] = {{__e}};
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                var {{__list}} = new System.Collections.Generic.List<{{type.ElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}}>({{fieldName}});
                {{__list}}.Add({{type.ElementType.Apply(CtorDefaultValueVisitor.Ins)}});
                {{fieldName}} = {{__list}}.ToArray();
            }
            UnityEditor.EditorGUILayout.EndVertical();
        }
        """;
    }

    public string Accept(TList type, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __i = $"__i{depth}";
        var __e = $"__e{depth}";

        return $$"""
        {
            UnityEditor.EditorGUILayout.BeginVertical("Box");
            int {{__n}} = {{fieldName}}.Count;
            for (int {{__i}} = 0; {{__i}} < {{__n}}; {{__i}}++)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    {{fieldName}}.RemoveAt({{__i}});
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    break;
                }
                UnityEditor.EditorGUILayout.LabelField({{__i}}.ToString(), GUILayout.Width(20));
                {{type.ElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}} {{__e}} = {{fieldName}}[{{__i}}];
                {{type.ElementType.Apply(this, __e, depth + 1)}};
                {{fieldName}}[{{__i}}] = {{__e}};
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                {{fieldName}}.Add({{type.ElementType.Apply(CtorDefaultValueVisitor.Ins)}});
            }
            UnityEditor.EditorGUILayout.EndVertical();
        }
        """;
    }

    public string Accept(TSet type, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __i = $"__i{depth}";
        var __e = $"__e{depth}";

        return $$"""
        {
            UnityEditor.EditorGUILayout.BeginVertical("Box");
            int {{__n}} = {{fieldName}}.Count;
            for (int {{__i}} = 0; {{__i}} < {{__n}}; {{__i}}++)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    {{fieldName}}.RemoveAt({{__i}});
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    break;
                }
                UnityEditor.EditorGUILayout.LabelField({{__i}}.ToString(), GUILayout.Width(20));
                {{type.ElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}} {{__e}} = {{fieldName}}[{{__i}}];
                {{type.ElementType.Apply(this, __e, depth + 1)}};
                {{fieldName}}[{{__i}}] = {{__e}};
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                {{fieldName}}.Add({{type.ElementType.Apply(CtorDefaultValueVisitor.Ins)}});
            }
            UnityEditor.EditorGUILayout.EndVertical();
        }
        """;
    }

    public string Accept(TMap type, string fieldName, int depth)
    {
        // Map在编辑器里是List<object[]>类型，其中object的程度固定为2
        var __n = $"__n{depth}";
        var __i = $"__i{depth}";
        var __e = $"__e{depth}";
        var __key = $"__key{depth}";
        var __value = $"__value{depth}";

        return $$"""
        {
            UnityEditor.EditorGUILayout.BeginVertical("Box");
            int {{__n}} = {{fieldName}}.Count;
            for (int {{__i}} = 0; {{__i}} < {{__n}}; {{__i}}++)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    {{fieldName}}.RemoveAt({{__i}});
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    break;
                }
                UnityEditor.EditorGUILayout.LabelField({{__i}}.ToString(), GUILayout.Width(20));
                var {{__e}} = {{fieldName}}[{{__i}}];
                {{type.KeyType.Apply(EditorDeclaringTypeNameVisitor.Ins)}} {{__key}} = {{type.KeyType.Apply(CtorDefaultValueVisitor.Ins)}};
                {{type.ValueType.Apply(EditorDeclaringTypeNameVisitor.Ins)}} {{__value}} = {{type.ValueType.Apply(CtorDefaultValueVisitor.Ins)}};
                if ({{__e}} == null)
                {
                    {{__e}} = new object[2] { {{__key}}, {{__value}} };
                    {{fieldName}}[{{__i}}] = {{__e}};
                }
                else
                {
                    {{__key}} = ({{type.KeyType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}}){{__e}}[0];
                    {{__value}} = ({{type.ValueType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}}){{__e}}[1];
                }
                {{type.KeyType.Apply(this, __key, depth + 1)}};
                {{type.ValueType.Apply(this, __value, depth + 1)}};
                {{__e}}[0] = {{__key}};
                {{__e}}[1] = {{__value}};
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                {{fieldName}}.Add(new object[2]);
            }
            UnityEditor.EditorGUILayout.EndVertical();
        }
        """;
    }
}
