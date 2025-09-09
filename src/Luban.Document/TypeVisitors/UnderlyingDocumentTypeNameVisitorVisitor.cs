using Luban.Document.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using System.Text;

namespace Luban.Document.TypeVisitors;

public class UnderlyingDocumentTypeNameVisitorVisitor : ITypeFuncVisitor<string, string>
{
    public static UnderlyingDocumentTypeNameVisitorVisitor Ins { get; } = new();

    public string Accept(TBool type, string rootPath)
    {
        return "bool";
    }

    public string Accept(TByte type, string rootPath)
    {
        return "byte";
    }

    public string Accept(TShort type, string rootPath)
    {
        return "short";
    }

    public string Accept(TInt type, string rootPath)
    {
        return "int";
    }

    public string Accept(TLong type, string rootPath)
    {
        return "long";
    }

    public string Accept(TFloat type, string rootPath)
    {
        return "float";
    }

    public string Accept(TDouble type, string rootPath)
    {
        return "double";
    }

    public virtual string Accept(TEnum type, string rootPath)
    {
        var link = $"{rootPath}/{DocumentTemplateExtensions.GetDefDocumentPath(type.DefEnum)}";
        link = MergeRepeatSlash(link);
        var typeName = type.DefEnum.FullName;
        return $"[{typeName}]({link})";
    }

    public string Accept(TString type, string rootPath)
    {
        return "string";
    }

    public string Accept(TBean type, string rootPath)
    {
        var link = $"{rootPath}/{DocumentTemplateExtensions.GetDefDocumentPath(type.DefBean)}";
        link = MergeRepeatSlash(link);
        var typeName = type.DefBean.FullName;
        return $"[{typeName}]({link})";
    }

    private string MergeRepeatSlash(string src)
    {
        if (string.IsNullOrEmpty(src))
        {
            return src;
        }

        var result = new StringBuilder();
        bool lastWasSlash = false;

        foreach (var ch in src)
        {
            if (ch == '/')
            {
                if (!lastWasSlash)
                {
                    result.Append(ch);
                    lastWasSlash = true;
                }
            }
            else
            {
                result.Append(ch);
                lastWasSlash = false;
            }
        }

        return result.ToString();
    }


    public string Accept(TArray type, string rootPath)
    {
        return $"{ConstStrings.ArrayTypeName}&lt;{type.ElementType.Apply(this, rootPath)}&gt;";
    }

    public string Accept(TList type, string rootPath)
    {
        return $"{ConstStrings.ListTypeName}&lt;{type.ElementType.Apply(this, rootPath)}&gt;";
    }

    public string Accept(TSet type, string rootPath)
    {
        return $"{ConstStrings.SetTypeName}&lt;{type.ElementType.Apply(this, rootPath)}&gt;";
    }

    public string Accept(TMap type, string rootPath)
    {
        return $"{ConstStrings.MapTypeName}&lt;{type.KeyType.Apply(this, rootPath)}, {type.ValueType.Apply(this, rootPath)}&gt;";
    }

    public virtual string Accept(TDateTime type, string rootPath)
    {
        return "long";
    }
}
