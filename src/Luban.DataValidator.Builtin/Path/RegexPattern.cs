using System.Text.RegularExpressions;

namespace Luban.DataValidator.Builtin.Path;

class RegexPattern : IPathPattern
{
    private readonly string _replacePattern;

    private readonly Regex _re;

    public bool EmptyAble { get; set; }

    public string Mode => "regex";

    public RegexPattern(string matchPattern, string replacePattern)
    {
        _re = new Regex(matchPattern);
        _replacePattern = replacePattern;
    }

    public bool ExistPath(string rootDir, string subFile)
    {
        if (!_re.IsMatch(subFile))
        {
            return false;
        }
        var replacePath = _re.Replace(subFile, _replacePattern);
        string finalPath = System.IO.Path.Combine(rootDir, replacePath);
        return File.Exists(finalPath);
    }
}
