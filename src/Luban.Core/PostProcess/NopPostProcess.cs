namespace Luban.Core.PostProcess;

[PostProcess("nop", TargetFileType.Code)]
public class NopPostProcess : PostProcessBase
{
    public override void PostProcess(OutputFileManifest oldOutputFileManifest, OutputFileManifest newOutputFileManifest,
        OutputFile outputFile)
    {
        newOutputFileManifest.AddFile(outputFile);
    }
}