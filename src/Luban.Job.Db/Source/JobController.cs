using Luban.Common.Protos;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Db
{
    public class JobController : IJobController
    {
        public Task GenAsync(RemoteAgent agent, GenJob rpc)
        {
            throw new NotImplementedException();
        }
    }
}
