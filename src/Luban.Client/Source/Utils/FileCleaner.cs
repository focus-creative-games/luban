using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Luban.Client.Common.Utils
{
    class FileCleaner
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly HashSet<string> _outputDirs = new HashSet<string>();
        private readonly HashSet<string> _savedFileOrDirs = new HashSet<string>();
        private readonly HashSet<string> _ignoreFileExtensions = new HashSet<string>();


        public void AddIgnoreExtension(string ext)
        {
            _ignoreFileExtensions.Add(ext);
        }

        public void AddOutputDir(string dir)
        {
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
                    s_logger.Info("[remove] file: {file}", file);
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
                    Directory.Delete(subDir);
                }
            }
        }


        public static void Clean(string outputDir, List<Luban.Common.Protos.FileInfo> savedFiles)
        {
            var cleaner = new FileCleaner();
            cleaner.AddOutputDir(outputDir);
            cleaner.AddIgnoreExtension("meta"); // for unity
            foreach (var file in savedFiles)
            {
                cleaner.AddSavedFile(file.FilePath);
            }
            cleaner.RemoveUnusedFiles();
        }

    }
}
