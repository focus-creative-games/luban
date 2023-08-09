namespace Luban.OutputSaver;

[OutputSaver("none")]
public class NoneSaver : IOutputSaver
{
    public void Save(OutputFileManifest outputFileManifest, string outputDir)
    {

    }

    public void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {

    }
}