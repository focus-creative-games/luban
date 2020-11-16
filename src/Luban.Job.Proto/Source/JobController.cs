using Luban.Common.Protos;
using Luban.Server.Common;
using System;
using System.Threading.Tasks;

namespace Luban.Job.Proto
{
    [Controller("proto")]
    public class JobController : IJobController
    {
        public Task GenAsync(RemoteAgent agent, GenJob rpc)
        {
            throw new NotImplementedException();
        }
    }
}
