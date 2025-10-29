// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Luban.Utils;

public class FileCleaner
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly HashSet<string> _outputDirs = new();
    private readonly HashSet<string> _savedFileOrDirs = new();
    private readonly HashSet<string> _ignoreFileExtensions = new();


    public void AddIgnoreExtension(string ext)
    {
        _ignoreFileExtensions.Add(ext);
    }

    public void AddOutputDir(string dir)
    {
        dir = Path.TrimEndingDirectorySeparator(dir);
        _outputDirs.Add(dir);
    }

    public void AddSavedFile(string file)
    {
        file = file.Replace('\\', '/');
        while (true)
        {
            _savedFileOrDirs.Add(file);
            s_logger.Trace("add save file:{file}", file);
            int sepIndex = file.LastIndexOf('/');
            if (sepIndex < 0)
            {
                break;
            }

            file = file[..sepIndex];
        }
    }


    public void RemoveUnusedFiles()
    {
        foreach (var dir in _outputDirs)
        {
            RemoveUnusedFileInDir(dir);
        }
    }

    private void RemoveUnusedFileInDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        var fullRootPath = Path.GetFullPath(dir);
        s_logger.Trace("full path:{path}", fullRootPath);

        foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
        {
            s_logger.Trace("file:{file}", file);
            string fullSubFilePath = Path.GetFullPath(file);
            var relateFile = fullSubFilePath[(fullRootPath.Length + 1)..].Replace('\\', '/');
            if (_savedFileOrDirs.Contains(relateFile))
            {
                s_logger.Trace("remain file:{file}", file);
            }
            else if (!_ignoreFileExtensions.Contains(FileUtil.GetFileExtension(file)))
            {
                s_logger.Info("[remove] {0}", file);
                File.Delete(file);
            }
        }

        // 清除空目录
        var subDirs = new List<string>(Directory.GetDirectories(dir, "*", SearchOption.AllDirectories));
        subDirs.Sort((a, b) => -string.Compare(a, b, StringComparison.Ordinal));
        foreach (var subDir in subDirs)
        {
            string fullSubDirPath = Path.GetFullPath(subDir);
            var relateDir = fullSubDirPath[(fullRootPath.Length + 1)..].Replace('\\', '/');
            if (_savedFileOrDirs.Contains(relateDir))
            {
                s_logger.Trace("remain directory:{dir}", relateDir);
            }
            else
            {
                s_logger.Info("[remove] dir: {dir}", subDir);
                FileUtil.DeleteDirectoryRecursive(subDir);
            }
        }
    }

    public static void Clean(string outputDir, List<string> savedFiles)
    {
        var cleaner = new FileCleaner();
        cleaner.AddOutputDir(outputDir);
        cleaner.AddIgnoreExtension("meta"); // for unity
        cleaner.AddIgnoreExtension("uid"); // for godot
        foreach (var file in savedFiles)
        {
            cleaner.AddSavedFile(file);
        }

        cleaner.RemoveUnusedFiles();
    }
}
