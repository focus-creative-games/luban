namespace Luban.DataValidator.Builtin.Path;

class GodotResourcePattern: IPathPattern
{
    public bool EmptyAble { get; set; }
    public bool ExistPath(string rootDir, string subFile)
    {
        return subFile.StartsWith("res://") && File.Exists(System.IO.Path.Combine(rootDir, subFile.Substring(6)));
    }
}
