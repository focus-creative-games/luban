using Luban.DataLoader;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

class EditorJsonLoad : ITypeFuncVisitor<string, string, int, string>
{
    public static EditorJsonLoad Ins { get; } = new();

    public string Accept(TBool type, string json, string x, int depth)
    {
        return $"if(!{json}.IsBoolean) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TByte type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TShort type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TInt type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TLong type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TFloat type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TDouble type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TEnum type, string json, string x, int depth)
    {
        return $"if({json}.IsString) {{ {x} = ({type.Apply(EditorDeclaringTypeNameVisitor.Ins)})System.Enum.Parse(typeof({type.Apply(EditorDeclaringTypeNameVisitor.Ins)}), {json}); }} else if({json}.IsNumber) {{ {x} = ({type.Apply(EditorDeclaringTypeNameVisitor.Ins)})(int){json}; }} else {{ throw new SerializationException(); }}  ";
    }

    public string Accept(TString type, string json, string x, int depth)
    {
        var ret = $"if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json};";
        if (type.HasTag("obj"))
        {

            var objType = type.GetTag("obj");
            if (objType == "sprite")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>({x});";
            }
            else if (objType == "texture")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>({x});";
            }
            else if (objType == "audioClip")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AudioClip>({x});";
            }
            else if (objType == "animationClip")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>({x});";
            }
            else if (objType == "material")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>({x});";
            }
            else if (objType == "gameObject")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>({x});";
            }
            else if (objType == "prefab")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>({x});";
            }
            else if (objType == "font")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Font>({x});";
            }
            else if (objType == "textAsset")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>({x});";
            }
            else if (objType == "shader")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Shader>({x});";
            }
            else if (objType == "scriptableObject")
            {
                ret += $"{x}_UnityObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.ScriptableObject>({x});";
            }
        }
        return ret;
    }

    public string Accept(TBean type, string json, string x, int depth)
    {
        if (type.DefBean.IsAbstractType)
        {
            var __index = $"__index{depth}";
            return
            $$"""

            if (!{{json}}.IsObject)
            {
                throw new SerializationException();
            }
            {{x}} = {{type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}}.LoadJson{{type.DefBean.Name}}({{json}});
            var {{__index}} = {{type.Apply(EditorDeclaringTypeNameVisitor.Ins)}}.Types.IndexOf({{x}}.GetTypeStr());
            if ({{__index}} == -1)
            {
                throw new SerializationException();
            }
            {{x}}.TypeIndex = {{__index}};
            {{x}}.Instance = {{type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}}.LoadJson{{type.DefBean.Name}}({{json}});
            """;
        }
        else
        {
            return $"if(!{json}.IsObject) {{ throw new SerializationException(); }}  {x} = {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.LoadJson{type.DefBean.Name}({json});";
        }
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        var __e = $"__e{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        var __n = $"__n{depth}";
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} int {__n} = {json}.Count; {x} = new {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}[{__n}]; int {__i}=0; foreach(SimpleJSON.JSONNode {__e} in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth++)}  {x}[{__i}++] = {__v}; }}  ";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        var __e = $"__e{depth}";
        var __v = $"__v{depth}";
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(SimpleJSON.JSONNode {__e} in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth++)}  {x}.Add({__v}); }}  ";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        var __e = $"__e{depth}";
        var __v = $"__v{depth}";
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(SimpleJSON.JSONNode {__e} in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth++)}  {x}.Add({__v}); }}  ";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        var __e = $"__e{depth}";
        var __k = $"__k{depth}";
        var __v = $"__v{depth}";
        return @$"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(SimpleJSON.JSONNode {__e} in {json}.Children) {{ {type.KeyType.Apply(EditorDeclaringTypeNameVisitor.Ins)} {__k};  {type.KeyType.Apply(this, $"{__e}[0]", __k, depth++)} {type.ValueType.Apply(EditorDeclaringTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth++)}  {x}.Add(new object[] {{ {__k}, {__v} }}); }}  ";
    }

    public string Accept(TDateTime type, string json, string x, int depth)
    {
        return $"if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json};";
    }
}
