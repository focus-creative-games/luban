namespace Luban.PostProcess;

public abstract class PostProcessBase : IPostProcess
{
    public virtual void PostProcess(OutputFileManifest oldOutputFileManifest, OutputFileManifest newOutputFileManifest)
    {
        foreach (var outputFile in oldOutputFileManifest.DataFiles)
        {
            PostProcess(oldOutputFileManifest, newOutputFileManifest, outputFile);
        }
    }

    public abstract void PostProcess(OutputFileManifest oldOutputFileManifest, OutputFileManifest newOutputFileManifest,
        OutputFile outputFile);
}
