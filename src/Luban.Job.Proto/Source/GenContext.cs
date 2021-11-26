using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Proto.Defs;
using Luban.Job.Proto.Generate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileInfo = Luban.Common.Protos.FileInfo;

namespace Luban.Job.Proto
{
    class GenContext
    {
        public GenArgs GenArgs { get; init; }
        public DefAssembly Assembly { get; init; }
        public string GenType { get; set; }
        public IRender Render { get; set; }
        public ELanguage Lan { get; set; }

        public List<DefTypeBase> ExportTypes { get; init; }

        public ConcurrentBag<FileInfo> GenCodeFilesInOutputCodeDir { get; init; }
        public ConcurrentBag<FileInfo> GenScatteredFiles { get; init; }
        public List<Task> Tasks { get; init; }
    }
}
