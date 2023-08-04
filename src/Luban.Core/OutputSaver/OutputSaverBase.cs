namespace Luban.Core.OutputSaver;

public abstract class OutputSaverBase : IOutputSaver
{

    protected virtual void BeforeSave(OutputFileManifest outputFileManifest, string outputDir)
    {

    }

    protected virtual void PostSave(OutputFileManifest outputFileManifest, string outputDir)
    {

    }
    
    public virtual void Save(OutputFileManifest outputFileManifest, string outputDir)
    {
        BeforeSave(outputFileManifest, outputDir);
        var tasks = new List<Task>();
        foreach (var outputFile in outputFileManifest.DataFiles)
        {
            tasks.Add(Task.Run(() =>
            {
                SaveFile(outputFileManifest, outputDir, outputFile);
            }));
        }
        Task.WaitAll(tasks.ToArray());
        PostSave(outputFileManifest, outputDir);
    }

    public abstract void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile);
}