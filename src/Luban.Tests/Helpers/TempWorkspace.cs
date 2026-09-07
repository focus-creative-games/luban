// Copyright 2025 Code Philosophy

namespace Luban.Tests.Helpers;

public sealed class TempWorkspace : IDisposable
{
    public string Root { get; }

    public string CodeDir { get; }

    public string DataDir { get; }

    public TempWorkspace(string prefix = "luban-test")
    {
        Root = Path.Combine(Path.GetTempPath(), prefix, Guid.NewGuid().ToString("N"));
        CodeDir = Path.Combine(Root, "code");
        DataDir = Path.Combine(Root, "data");
        Directory.CreateDirectory(CodeDir);
        Directory.CreateDirectory(DataDir);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup for CI temp dirs.
        }
    }
}
