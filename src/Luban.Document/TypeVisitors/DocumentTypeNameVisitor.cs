using Luban.Document;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Document.TypeVisitors;

public class DocumentTypeNameVisitor : DecoratorFuncVisitor<string, string>
{
    public static DocumentTypeNameVisitor Ins { get; } = new();

    protected virtual ITypeFuncVisitor<string, string> UnderlyingVisitor => UnderlyingDocumentTypeNameVisitorVisitor.Ins;

    public override string DoAccept(TType type, string rootPath)
    {
        return type.IsNullable && !type.Apply(IsRawNullableTypeVisitor.Ins) ? (type.Apply(UnderlyingVisitor, rootPath) + "?") : type.Apply(UnderlyingVisitor, rootPath);
    }
}
