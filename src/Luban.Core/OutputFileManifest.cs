namespace Luban;

public class OutputFileManifest
{
    public string TargetName { get; init; }
    
    private readonly List<OutputFile> _dataFiles = new();

    public IReadOnlyList<OutputFile> DataFiles => _dataFiles;
    
    public OutputFileManifest(string targetName)
    {
        TargetName = targetName;
    }
    

    public void AddFile(string file, object content)
    {
        AddFile(new OutputFile { File = file, Content = content });
    }
    
    public void AddFile(OutputFile file)
    {
        lock (this)
        {
            _dataFiles.Add(file);
        }
    }
}