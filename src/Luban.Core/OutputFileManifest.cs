namespace Luban;

public enum OutputType
{
    Code,
    Data,
}

public class OutputFileManifest
{
    public string TargetName { get; }

    public OutputType OutputType { get; }

    private readonly List<OutputFile> _dataFiles = new();

    public IReadOnlyList<OutputFile> DataFiles => _dataFiles;

    public OutputFileManifest(string targetName, OutputType outputType)
    {
        TargetName = targetName;
        OutputType = outputType;
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
