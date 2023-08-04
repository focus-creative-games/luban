namespace Luban.OutputSaver;

public interface IOutputSaver
{
    void Save(OutputFileManifest outputFileManifest, string outputDir);

    void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile);
}