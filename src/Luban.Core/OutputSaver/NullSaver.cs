namespace Luban.OutputSaver;

[OutputSaver("null")]
public class NullSaver : OutputSaverBase
{
    public override void Save(OutputFileManifest outputFileManifest)
    {

    }

    public override void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {

    }
}
