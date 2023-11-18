namespace Luban.DataValidator.Builtin.Path;

class UnityAddressablePattern : IPathPattern
{
    public bool EmptyAble { get; set; }

    public UnityAddressablePattern()
    {
    }

    public bool ExistPath(string rootDir, string subFile)
    {
        return File.Exists(System.IO.Path.Combine(rootDir, subFile));
    }
}
