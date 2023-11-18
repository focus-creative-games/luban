namespace Luban.DataValidator.Builtin.Path;

interface IPathPattern
{
    bool ExistPath(string rootDir, string subFile);

    bool EmptyAble { get; set; }
}
