using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Client.Utils
{
    class MultiFileWatcher
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

                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
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

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            lock (_watchLocker)
            {
                if (_watchDirChange)
                {
                    return;
                }
                _watchDirChange = true;

                Task.Run(async () =>
                {
                    await Task.Delay(400);

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
