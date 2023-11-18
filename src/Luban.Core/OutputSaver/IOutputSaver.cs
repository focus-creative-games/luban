namespace Luban.OutputSaver;

public interface IOutputSaver
{
    string Name { get; }

    void Save(OutputFileManifest outputFileManifest);

    void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile);
}
