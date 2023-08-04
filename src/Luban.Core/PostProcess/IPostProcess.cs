namespace Luban.Core.PostProcess;

public interface IPostProcess
{
    void PostProcess(OutputFileManifest oldOutputFileManifest, OutputFileManifest newOutputFileManifest);
    void PostProcess(OutputFileManifest oldOutputFileManifest, OutputFileManifest newOutputFileManifest, OutputFile outputFile);
}