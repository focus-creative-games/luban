namespace Luban.DataValidator.Builtin.Path;

class SimpleReplacePattern : IPathPattern
{
    private readonly string _prefix;
    private readonly string _suffix;

    public bool EmptyAble { get; set; }

    public string Mode => "normal";

    public SimpleReplacePattern(string prefix, string suffix)
    {
        _prefix = prefix;
        _suffix = suffix;
    }

    public bool ExistPath(string rootDir, string subFile)
    {
        string finalPath = System.IO.Path.Combine(rootDir, _prefix + subFile + _suffix);
        return File.Exists(finalPath);
    }
}
