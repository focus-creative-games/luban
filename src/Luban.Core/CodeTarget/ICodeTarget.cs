using Luban.Defs;
using System.Text;

namespace Luban.CodeTarget;

public interface ICodeTarget
{
    string Name { get; }

    void ValidateDefinition(GenerationContext ctx);

    void Handle(GenerationContext ctx, OutputFileManifest manifest);

    string FileHeader { get; }

    Encoding FileEncoding { get; }

    string GetPathFromFullName(string fullName);

    void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer);

    void GenerateTable(GenerationContext ctx, DefTable table, CodeWriter writer);

    void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer);

    void GenerateEnum(GenerationContext ctx, DefEnum @enum, CodeWriter writer);
}
