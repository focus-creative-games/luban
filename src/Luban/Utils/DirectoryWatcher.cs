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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Utils;

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
