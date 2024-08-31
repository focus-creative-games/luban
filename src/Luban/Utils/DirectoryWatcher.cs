using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Utils
{
    internal class DirectoryWatcher
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        private readonly Action _onChange;

        private readonly object _watchLocker = new object();

        public DirectoryWatcher(string[] files, Action onChange)
        {
            this._onChange = onChange;

            s_logger.Info("watching dirs: {0}", string.Join(",", files));
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
            }
        }

        private static readonly List<string> _filterSuffixs = new List<string>
        {
            ".xlsx",
            ".csv",
            ".xls",
            ".xlsm",
            ".json",
            ".lua",
            ".xml",
            ".yml",
        };

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            var dirtyName = e.Name;
            if (string.IsNullOrWhiteSpace(dirtyName)
                || !_filterSuffixs.Any(dirtyName.EndsWith)
                || dirtyName.Contains('~')
                || dirtyName.Contains('$'))
            {
                return;
            }

            lock (_watchLocker)
            {
                s_logger.Trace("== mark dirty:{}", e.FullPath);

                try
                {
                    this._onChange();
                }
                catch (Exception e2)
                {
                    s_logger.Error(e2, "OnChange exception");
                }
            }
        }
    }
}
