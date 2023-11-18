using System.Text.RegularExpressions;

namespace Luban.DataValidator.Builtin.Path;

class Ue4ResourcePattern : IPathPattern
{
    private readonly Regex _pat1;
    private readonly Regex _pat2;

    public bool EmptyAble { get; set; }

    public Ue4ResourcePattern()
    {
        _pat1 = new Regex(@"^/Game/(.+?)(\..+)?$");
        _pat2 = new Regex(@"^\w+'/Game/(.+?)(\..+)?'$");
    }

    private bool CheckMatch(Match match)
    {
        var groups = match.Groups;
        if (!groups[1].Success)
        {
            return false;
        }
        if (groups[2].Success)
        {
            // 如果是  /Game/../xxx.yyy 的情形
            // 要求 yyy == xxx 或者 yyy == xxx_C
            string path = groups[1].Value;
            string suffix = groups[2].Value.Substring(1);
            if (suffix.EndsWith("_C"))
            {
                suffix = suffix[0..^2];
            }
            return path.EndsWith(suffix);
        }
        return true;
    }

    private bool AlternativePaths(string rawPath)
    {
        return File.Exists($"{rawPath}.uasset") || File.Exists($"{rawPath}.umap");
    }

    public bool ExistPath(string rootDir, string subFile)
    {
        var match1 = _pat1.Match(subFile);
        if (match1.Success)
        {
            return CheckMatch(match1) && AlternativePaths(System.IO.Path.Combine(rootDir, match1.Groups[1].Value));
        }
        var match2 = _pat2.Match(subFile);
        if (match2.Success)
        {
            return CheckMatch(match2) && AlternativePaths(System.IO.Path.Combine(rootDir, match2.Groups[1].Value));
        }
        return false;
    }
}
