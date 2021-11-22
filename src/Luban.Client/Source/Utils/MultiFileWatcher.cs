using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Client.Utils
{
    public class MultiFileWatcher
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        private readonly Action _onChange;

        private readonly object _watchLocker = new object();
        private bool _watchDirChange = false;

        public MultiFileWatcher(string[] files, Action onChange)
        {
            this._onChange = onChange;

            foreach (var file in files)
            {
                var watcher = new FileSystemWatcher(file);

                watcher.NotifyFilter = // NotifyFilters.Attributes
                                       //| NotifyFilters.CreationTime
                                     NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     //| NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     //| NotifyFilters.Security
                                     | NotifyFilters.Size;

                watcher.Changed += this.OnChange;
                watcher.Created += this.OnChange;
                watcher.Deleted += this.OnChange;
                watcher.Renamed += this.OnChange;

                //watcher.Filter = "*.txt";
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                this._watchers.Add(watcher);

                s_logger.Info("=== start watch. dir:{} ==", file);
            }
        }

        private static readonly List<string> _filterSuffixs = new List<string>
        {
            ".xlsx",
            ".csv",
            ".xls",
            ".json",
            ".lua",
            ".xml",
        };

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            var dirtyName = e.Name;
            if (string.IsNullOrWhiteSpace(dirtyName)
                || !_filterSuffixs.Any(s => dirtyName.EndsWith(s))
                || dirtyName.Contains('~')
                || dirtyName.Contains('$'))
            {
                return;
            }

            lock (_watchLocker)
            {
                if (_watchDirChange)
                {
                    s_logger.Trace("== has mark dirty:{}. ignore", e.FullPath);
                    return;
                }
                _watchDirChange = true;
                s_logger.Trace("== mark dirty:{}", e.FullPath);

                Task.Run(async () =>
                {
                    await Task.Delay(100);

                    lock (_watchLocker)
                    {
                        _watchDirChange = false;
                        try
                        {
                            this._onChange();
                        }
                        catch (Exception e)
                        {
                            s_logger.Error(e, "OnChange exception");
                        }
                    }
                    s_logger.Info("=== watch changes ==");
                });
            }
        }
    }
}
